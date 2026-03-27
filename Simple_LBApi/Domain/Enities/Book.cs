namespace Simple_LBApi.Domain.Enities
{
    public class Book : BaseEntity
    {
        public string Title { get; set; }
        public string Author { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }

        public ICollection<Loan> Loans { get; set; }
    }
}
