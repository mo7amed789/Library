using Simple_LBApi.DTOs;

namespace Simple_LBApi.Services.Interfaces
{
    public interface ILoanService
    {
        Task BorrowAsync(int userId, BorrowDto dto);
        Task ReturnAsync(int loanId);
        Task RenewAsync(int loanId);
        Task<List<LoanResponseDto>> GetUserLoans(int userId);
    }
}
