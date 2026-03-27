namespace Simple_LBApi.DTOs
{
    public class BookResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int AvailableCopies { get; set; }
    }
}
