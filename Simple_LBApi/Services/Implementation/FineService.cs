using Microsoft.EntityFrameworkCore;
using Simple_LBApi.Common;
using Simple_LBApi.Data;
using Simple_LBApi.Domain.Enities;
using Simple_LBApi.Services.Interfaces;

namespace Simple_LBApi.Services.Implementation
{
    public class FineService : IFineService
    {
        private readonly AppDbContext _context;

        public FineService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Fine>> GetMyUnpaidAsync(int userId)
        {
            return await _context.Fines
                .Include(f => f.Loan)
                .Where(f => !f.IsPaid && f.Loan.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkPaidAsync(int fineId, int userId, bool isAdmin)
        {
            var fine = await _context.Fines
                .Include(f => f.Loan)
                .FirstOrDefaultAsync(f => f.Id == fineId)
                ?? throw new ApiException("Fine not found", StatusCodes.Status404NotFound);

            if (!isAdmin && fine.Loan.UserId != userId)
            {
                throw new ApiException("Forbidden", StatusCodes.Status403Forbidden);
            }

            if (fine.IsPaid)
            {
                throw new ApiException("Fine already paid", StatusCodes.Status400BadRequest);
            }

            fine.IsPaid = true;
            fine.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
