namespace Simple_LBApi.DTOs
{
    public class LoanResponseDto
    {
        public int Id { get; set; }
        public string BookTitle { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
    }
}
