using Simple_LBApi.Data;
using Simple_LBApi.Domain.Enities;
using Simple_LBApi.Domain.Enums;
using Simple_LBApi.DTOs;
using Simple_LBApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class LoanService : ILoanService
{
    private readonly AppDbContext _context;

    private const int MaxLoans = 3;
    private const int LoanDays = 14;
    private const int MaxRenewals = 2;
    private const int FinePerDay = 5;

    public LoanService(AppDbContext context)
    {
        _context = context;
    }

    public async Task BorrowAsync(int userId, BorrowDto dto)
    {
        var activeLoans = await _context.Loans
            .CountAsync(l => l.UserId == userId && l.Status == LoanStatus.Active);

        if (activeLoans >= MaxLoans)
            throw new Exception("Max loans reached");

        var book = await _context.Books.FindAsync(dto.BookId);

        if (book == null || book.IsDeleted)
            throw new Exception("Book not found");

        if (book.AvailableCopies <= 0)
            throw new Exception("Book not available");

        var loan = new Loan
        {
            UserId = userId,
            BookId = dto.BookId,
            LoanDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(LoanDays),
            Status = LoanStatus.Active
        };

        book.AvailableCopies--;

        _context.Loans.Add(loan);
        await _context.SaveChangesAsync();
    }

    public async Task ReturnAsync(int loanId)
    {
        var loan = await _context.Loans
            .Include(l => l.Book)
            .FirstOrDefaultAsync(l => l.Id == loanId);

        if (loan == null)
            throw new Exception("Loan not found");

        if (loan.Status == LoanStatus.Returned)
            throw new Exception("Already returned");

        loan.ReturnDate = DateTime.UtcNow;
        loan.Status = LoanStatus.Returned;

        loan.Book.AvailableCopies++;

        // 🔥 Fine Logic
        if (loan.ReturnDate > loan.DueDate)
        {
            var daysLate = (loan.ReturnDate.Value - loan.DueDate).Days;

            var fine = new Fine
            {
                LoanId = loan.Id,
                Amount = daysLate * FinePerDay
            };

            _context.Fines.Add(fine);
        }

        await _context.SaveChangesAsync();
    }

    public async Task RenewAsync(int loanId)
    {
        var loan = await _context.Loans.FindAsync(loanId);

        if (loan == null)
            throw new Exception("Loan not found");

        if (loan.RenewCount >= MaxRenewals)
            throw new Exception("Max renewals reached");

        if (loan.Status != LoanStatus.Active)
            throw new Exception("Cannot renew");

        loan.DueDate = loan.DueDate.AddDays(LoanDays);
        loan.RenewCount++;

        await _context.SaveChangesAsync();
    }

    public async Task<List<LoanResponseDto>> GetUserLoans(int userId)
    {
        return await _context.Loans
            .Include(l => l.Book)
            .Where(l => l.UserId == userId)
            .Select(l => new LoanResponseDto
            {
                Id = l.Id,
                BookTitle = l.Book.Title,
                LoanDate = l.LoanDate,
                DueDate = l.DueDate,
                Status = l.Status.ToString()
            })
            .ToListAsync();
    }
}