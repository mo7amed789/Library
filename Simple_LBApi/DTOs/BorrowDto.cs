using System.ComponentModel.DataAnnotations;

namespace Simple_LBApi.DTOs
{
    public class BorrowDto
    {
        [Range(1, int.MaxValue)]
        public int BookId { get; set; }
    }
}
