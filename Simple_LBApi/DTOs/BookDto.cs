using System.ComponentModel.DataAnnotations;

namespace Simple_LBApi.DTOs
{
    public class BookDto
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Author { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }

        [Range(1, 10000)]
        public int TotalCopies { get; set; }
    }
}
