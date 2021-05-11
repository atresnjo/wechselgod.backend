using System.Collections.Generic;
using wechselGod.Domain;

namespace wechselGod.Api.Services.Models
{
    public record FinGetAllBankAccountsResponse(List<BankAccount> Accounts);
}
