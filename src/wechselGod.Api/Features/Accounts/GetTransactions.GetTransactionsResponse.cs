using System.Collections.Generic;
using wechselGod.Domain;

namespace wechselGod.Api.Features.Accounts
{
    public record GetBankAccountTransactionsResponse(IReadOnlyList<BankAccountTransactionItem> Transactions);
}
