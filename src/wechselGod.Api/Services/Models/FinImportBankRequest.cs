using System.Collections.Generic;
using Newtonsoft.Json;

namespace wechselGod.Api.Services.Models
{
    public class FinImportBankRequest
    {
        public FinImportBankRequest()
        {
            
        }

        [JsonProperty("bankId")]
        public long BankId { get; set; }

        public FinImportBankRequest(long bankId, string name, string interfaceData, IReadOnlyList<string> accountTypes)
        {
            BankId = bankId;
            Name = name;
            InterfaceData = interfaceData;
            AccountTypes = accountTypes;
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("interface")]
        public string InterfaceData { get; set; }

        [JsonProperty("accountTypes")]
        public IReadOnlyList<string> AccountTypes { get; set; }
    }
}
