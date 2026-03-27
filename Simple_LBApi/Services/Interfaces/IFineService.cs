using Simple_LBApi.Domain.Enities;

namespace Simple_LBApi.Services.Interfaces
{
    public interface IFineService
    {
        Task<List<Fine>> GetMyUnpaidAsync(int userId);
        Task MarkPaidAsync(int fineId, int userId, bool isAdmin);
    }
}
