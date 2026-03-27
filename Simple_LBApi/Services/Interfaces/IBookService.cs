using Simple_LBApi.DTOs;

namespace Simple_LBApi.Services.Interfaces
{
    public interface IBookService
    {
        Task<List<BookResponseDto>> GetAllAsync();
        Task<BookResponseDto> GetByIdAsync(int id);
        Task CreateAsync(BookDto dto);   // ✅ changed
        Task UpdateAsync(int id, BookDto dto); // ✅ changed
        Task DeleteAsync(int id);
    }
}
