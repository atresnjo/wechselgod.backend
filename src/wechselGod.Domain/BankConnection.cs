using System;
using System.Collections.Generic;

namespace wechselGod.Domain
{
    public record BankConnection(long BankId, long BankConnectionId, string DisplayName, DateTime LastUpdated,
        int AccountType, IReadOnlyList<long> AccountIds);
}
