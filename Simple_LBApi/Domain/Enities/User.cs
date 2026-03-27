namespace Simple_LBApi.Domain.Enities
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public string Role { get; set; } // Admin / User

        public bool IsEmailVerified { get; set; } = false;

        public ICollection<Loan> Loans { get; set; }
    }
}
