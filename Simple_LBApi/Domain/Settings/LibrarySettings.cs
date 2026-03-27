namespace Simple_LBApi.Domain.Settings
{
    public class LibrarySettings
    {
        public int MaxLoansPerUser { get; set; }
        public int LoanDurationDays { get; set; }
        public decimal FinePerDay { get; set; }
    }
}
