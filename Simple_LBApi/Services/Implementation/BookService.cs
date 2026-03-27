using Microsoft.EntityFrameworkCore;
using Simple_LBApi.Common;
using Simple_LBApi.Data;
using Simple_LBApi.Domain.Enities;
using Simple_LBApi.DTOs;
using Simple_LBApi.Services.Interfaces;

namespace Simple_LBApi.Services.Implementation
{
    public class BookService : IBookService
    {
        private readonly AppDbContext _context;

        public BookService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BookResponseDto>> GetAllAsync()
        {
            return await _context.Books
                .AsNoTracking()
                .Include(b => b.Category)
                .Where(b => !b.IsDeleted)
                .Select(b => new BookResponseDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    CategoryName = b.Category.Name,
                    AvailableCopies = b.AvailableCopies
                })
                .ToListAsync();
        }

        public async Task<BookResponseDto> GetByIdAsync(int id)
        {
            var book = await _context.Books
                .AsNoTracking()
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

            if (book is null)
            {
                throw new ApiException("Book not found", StatusCodes.Status404NotFound);
            }

            return new BookResponseDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                CategoryName = book.Category.Name,
                AvailableCopies = book.AvailableCopies
            };
        }

        public async Task CreateAsync(BookDto dto)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
            {
                throw new ApiException("Invalid category", StatusCodes.Status400BadRequest);
            }

            var book = new Book
            {
                Title = dto.Title.Trim(),
                Author = dto.Author.Trim(),
                CategoryId = dto.CategoryId,
                TotalCopies = dto.TotalCopies,
                AvailableCopies = dto.TotalCopies
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, BookDto dto)
        {
            var book = await _context.Books.FindAsync(id);

            if (book is null || book.IsDeleted)
            {
                throw new ApiException("Book not found", StatusCodes.Status404NotFound);
            }

            book.Title = dto.Title.Trim();
            book.Author = dto.Author.Trim();
            book.CategoryId = dto.CategoryId;

            var diff = dto.TotalCopies - book.TotalCopies;
            book.TotalCopies = dto.TotalCopies;
            book.AvailableCopies += diff;

            if (book.AvailableCopies < 0)
            {
                throw new ApiException("Invalid total copies", StatusCodes.Status400BadRequest);
            }

            book.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book is null)
            {
                throw new ApiException("Book not found", StatusCodes.Status404NotFound);
            }

            book.IsDeleted = true;
            book.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
