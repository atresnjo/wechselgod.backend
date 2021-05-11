using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using wechselGod.Api.Services;

namespace wechselGod.Api.Features.Accounts
{
    public class ImportBank : BaseAsyncEndpoint
        .WithRequest<AddBankRequest>
        .WithResponse<AddBankResponse>
    {
        private readonly FinApiService _finApiService;

        public ImportBank(FinApiService finApiService)
        {
            _finApiService = finApiService;
        }

        [Authorize]
        [HttpPost("/accounts/banks")]
        [SwaggerOperation(
            Summary = "Imports a new bank connection",
            Description = "Imports a new bank connection",
            OperationId = "Accounts.ImportBank",
            Tags = new[] {"AccountsEndpoint"})
        ]

        public override async Task<ActionResult<AddBankResponse>> HandleAsync([FromQuery] AddBankRequest request,
            CancellationToken cancellationToken = default)
        {
            var redirectUrl = await _finApiService.ImportBank(request.Id, cancellationToken);
            if (string.IsNullOrEmpty(redirectUrl))
            {
                // handle and log properly
                return BadRequest();
            }

            return Ok(new AddBankResponse(redirectUrl));
        }
    }
}
