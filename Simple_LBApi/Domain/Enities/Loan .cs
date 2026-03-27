using static System.Net.WebRequestMethods;
using Simple_LBApi.Domain.Enums;

namespace Simple_LBApi.Domain.Enities
{
    public class Loan : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }

        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public int RenewCount { get; set; }

        public LoanStatus Status { get; set; }

        public Fine Fine { get; set; }
    }
}
