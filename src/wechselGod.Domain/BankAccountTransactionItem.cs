using System;
using System.Collections.Generic;

namespace wechselGod.Domain
{
    public record BankAccountTransactionItem (DateTime BankBookingDate, decimal AmountSpent,
        IReadOnlyList<BankAccountTransaction> Transactions);
}
