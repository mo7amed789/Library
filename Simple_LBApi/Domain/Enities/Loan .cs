using Simple_LBApi.Domain.Enums;

namespace Simple_LBApi.Domain.Enities
{
    public class Loan : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int RenewCount { get; set; }
        public LoanStatus Status { get; set; }

        public Fine? Fine { get; set; }
    }
}
