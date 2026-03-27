namespace Simple_LBApi.Domain.Enities
{
    public class Fine : BaseEntity
    {
        public int Id { get; set; }

        public int LoanId { get; set; }

        public decimal Amount { get; set; }

        public bool IsPaid { get; set; } = false;

        public DateTime CreatedAt { get; set; }

        // Navigation
        public Loan Loan { get; set; }
    }
}
