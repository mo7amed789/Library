using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Simple_LBApi.Common;
using Simple_LBApi.Data;
using Simple_LBApi.Domain.Enities;
using Simple_LBApi.Domain.Enums;
using Simple_LBApi.Domain.Settings;
using Simple_LBApi.DTOs;
using Simple_LBApi.Services.Interfaces;

namespace Simple_LBApi.Services.Implementation
{
    public class LoanService : ILoanService
    {
        private readonly AppDbContext _context;
        private readonly LibrarySettings _settings;

        private const int MaxRenewals = 2;

        public LoanService(AppDbContext context, IOptions<LibrarySettings> options)
        {
            _context = context;
            _settings = options.Value;
        }

        public async Task BorrowAsync(int userId, BorrowDto dto)
        {
            var activeLoans = await _context.Loans
                .CountAsync(l => l.UserId == userId && l.Status == LoanStatus.Active);

            if (activeLoans >= _settings.MaxLoansPerUser)
            {
                throw new ApiException("Max loans reached", StatusCodes.Status400BadRequest);
            }

            var book = await _context.Books.FindAsync(dto.BookId);
            if (book is null || book.IsDeleted)
            {
                throw new ApiException("Book not found", StatusCodes.Status404NotFound);
            }

            if (book.AvailableCopies <= 0)
            {
                throw new ApiException("Book not available", StatusCodes.Status409Conflict);
            }

            var loan = new Loan
            {
                UserId = userId,
                BookId = dto.BookId,
                LoanDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(_settings.LoanDurationDays),
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

            if (loan is null)
            {
                throw new ApiException("Loan not found", StatusCodes.Status404NotFound);
            }

            if (loan.Status == LoanStatus.Returned)
            {
                throw new ApiException("Already returned", StatusCodes.Status400BadRequest);
            }

            loan.ReturnDate = DateTime.UtcNow;
            loan.Status = LoanStatus.Returned;
            loan.Book.AvailableCopies++;

            if (loan.ReturnDate > loan.DueDate)
            {
                var daysLate = (loan.ReturnDate.Value - loan.DueDate).Days;
                var fine = new Fine
                {
                    LoanId = loan.Id,
                    Amount = daysLate * _settings.FinePerDay
                };
                _context.Fines.Add(fine);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RenewAsync(int loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan is null)
            {
                throw new ApiException("Loan not found", StatusCodes.Status404NotFound);
            }

            if (loan.RenewCount >= MaxRenewals)
            {
                throw new ApiException("Max renewals reached", StatusCodes.Status400BadRequest);
            }

            if (loan.Status != LoanStatus.Active)
            {
                throw new ApiException("Cannot renew", StatusCodes.Status400BadRequest);
            }

            loan.DueDate = loan.DueDate.AddDays(_settings.LoanDurationDays);
            loan.RenewCount++;

            await _context.SaveChangesAsync();
        }

        public async Task<List<LoanResponseDto>> GetUserLoans(int userId)
        {
            return await _context.Loans
                .AsNoTracking()
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
}
