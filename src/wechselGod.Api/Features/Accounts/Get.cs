using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;
using Org.OpenAPITools.Api;
using Swashbuckle.AspNetCore.Annotations;
using wechselGod.Api.Extensions;
using wechselGod.Api.Services;

namespace wechselGod.Api.Features.Accounts
{
    public class Get : BaseAsyncEndpoint
        .WithoutRequest
        .WithResponse<AccountDetailsResponse>
    {
        private readonly FinApiService _finApiService;

        public Get(FinApiService finApiService)
        {
            _finApiService = finApiService;
        }

        [Authorize]
        [HttpGet("/accounts/details")]
        [SwaggerOperation(
            Summary = "Gets account details",
            Description = "Gets detailed information about the logged in account (bank accounts, last transactions)",
            OperationId = "Accounts.Get",
            Tags = new[] {"BankEndpoint"})
        ]

        public override async Task<ActionResult<AccountDetailsResponse>> HandleAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await _finApiService.GetAllBankAccounts(cancellationToken);
            if (response == null)
                return Ok();

            foreach (var responseAccount in response.Accounts)
            {
                responseAccount.FormattedBalance = long.Parse(responseAccount.Balance.FormatBalance());
            }

            var session = await _finApiService.BuildUserSessionConfiguration(cancellationToken);
            var client = new BankConnectionsApi(session);
            var allBankConnections = await client.GetAllBankConnectionsAsync(cancellationToken: cancellationToken);

            var allBankConnectionIds = allBankConnections.Connections.SelectMany(x => x.AccountIds).Shuffle();
            var transactionsPayload =
                await _finApiService.GetAllBankTransactions(allBankConnectionIds.Shuffle().Take(1).FirstOrDefault(), cancellationToken);

            var finalTransactions = transactionsPayload.Transactions.Shuffle().OrderByDescending(x => x.BankBookingDate).Take(5).ToList();
            foreach (var finalTransaction in finalTransactions)
            {
                finalTransaction.FormattedBalance = long.Parse(finalTransaction.Amount.FormatBalance());
            }

            return Ok(new AccountDetailsResponse(response.Accounts, finalTransactions));
        }
    }
}
