using System.Collections.Generic;
using wechselGod.Domain;

namespace wechselGod.Api.Features.Accounts
{
    public record AccountDetailsResponse(List<BankAccount> BankAccounts, List<BankAccountTransaction> Transactions);
}
