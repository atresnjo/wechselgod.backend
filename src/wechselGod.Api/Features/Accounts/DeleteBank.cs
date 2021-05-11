using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.OpenAPITools.Api;
using Swashbuckle.AspNetCore.Annotations;
using wechselGod.Api.Services;

namespace wechselGod.Api.Features.Accounts
{
    public class DeleteBank : BaseAsyncEndpoint
        .WithRequest<DeleteBankRequest>
        .WithoutResponse
    {
        private readonly FinApiService _finApiService;

        public DeleteBank(FinApiService finApiService)
        {
            _finApiService = finApiService;
        }

        [Authorize]
        [HttpDelete("/accounts/banks/{id}")]
        [SwaggerOperation(
            Summary = "Deletes a bank connection",
            Description = "Deletes a bank connection",
            OperationId = "Accounts.DeleteBank",
            Tags = new[] {"AccountsEndpoint"})
        ]

        public override async Task<ActionResult> HandleAsync([FromRoute] DeleteBankRequest request,
            CancellationToken cancellationToken = default)
        {
            var session = await _finApiService.BuildUserSessionConfiguration(cancellationToken);
            var api = new BankConnectionsApi(session);
            await api.DeleteBankConnectionAsync(request.Id, cancellationToken);
            return Ok();
        }
    }
}