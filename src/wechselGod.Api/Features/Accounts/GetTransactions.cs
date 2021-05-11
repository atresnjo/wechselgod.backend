using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using wechselGod.Api.Services;
using wechselGod.Domain;

namespace wechselGod.Api.Features.Accounts
{
    public class GetTransactions : BaseAsyncEndpoint
        .WithRequest<GetTransactionsRequest>
        .WithResponse<GetBankAccountTransactionsResponse>
    {
        private readonly FinApiService _finApiService;

        public GetTransactions(FinApiService finApiService)
        {
            _finApiService = finApiService;
        }

        [Authorize]
        [HttpGet("/accounts/banks/{id}/transactions")]
        [SwaggerOperation(
            Summary = "Gets transactions for the requested bank account",
            Description = "Gets all transactions for the requested bank account",
            OperationId = "Accounts.GetTransactions",
            Tags = new[] {"AccountsEndpoint"})
        ]

        public override async Task<ActionResult<GetBankAccountTransactionsResponse>> HandleAsync(
            [FromRoute] GetTransactionsRequest request, CancellationToken cancellationToken = default)
        {
            var transactionsPayload = await _finApiService.GetAllBankTransactions(request.Id, cancellationToken);
            var groupedTransactions = transactionsPayload.Transactions.GroupBy(x => x.BankBookingDate);
            var transactions = groupedTransactions.Select(bankAccountTransactions =>
                new BankAccountTransactionItem(bankAccountTransactions.Key, bankAccountTransactions.Sum(x => x.Amount),
                    bankAccountTransactions.ToList())).ToList();

            return Ok(new GetBankAccountTransactionsResponse(transactions));
        }
    }
}
