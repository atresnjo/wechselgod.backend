using System.Collections.Generic;
using wechselGod.Domain;

namespace wechselGod.Api.Services.Models
{
    public class FinGetAllBankTransactionsResponse
    {
        public IReadOnlyList<BankAccountTransaction> Transactions { get; set; }
    }
}
