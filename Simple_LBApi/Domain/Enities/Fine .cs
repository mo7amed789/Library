namespace Simple_LBApi.Domain.Enities
{
    public class Fine : BaseEntity
    {
        public int LoanId { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }

        public Loan Loan { get; set; } = null!;
    }
}
