using System;

namespace wechselGod.Domain
{
    public class BankAccountTransaction
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public DateTime ValueDate { get; set; }
        public DateTime BankBookingDate { get; set; }
        public decimal Amount { get; set; }
        public string Purpose { get; set; }
        public string CounterpartName { get; set; }
        public string CounterpartAccountNumber { get; set; }
        public string CounterpartIban { get; set; }
        public string CounterpartBic { get; set; }
        public long FormattedBalance { get; set; }
        public bool IsNegative => Amount < 0;
    }
}
