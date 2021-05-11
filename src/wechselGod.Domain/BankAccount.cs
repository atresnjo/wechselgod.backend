namespace wechselGod.Domain
{
    public class BankAccount
    {
        public long Id { get; set; }
        public string AccountName { get; set; }
        public string Iban { get; set; }
        public string AccountHolderName { get; set; }
        public string AccountCurrency { get; set; }
        public decimal Balance { get; set; }
        public long FormattedBalance { get; set; }
        public string AccountNumber { get; set; }
        public long BankId { get; set; }
        public long BankConnectionId { get; set; }
        public string SvgPath { get; set; }
        public bool IsNegative => Balance < 0;
    }
}
