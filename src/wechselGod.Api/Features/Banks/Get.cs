using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Org.OpenAPITools.Api;
using Swashbuckle.AspNetCore.Annotations;
using wechselGod.Api.Services;

namespace wechselGod.Api.Features.Banks
{
    public class Get : BaseAsyncEndpoint
        .WithRequest<GetBankDetailsRequest>
        .WithResponse<GetBankDetailsResponse>
    {
        private readonly FinApiService _finApiService;

        public Get(FinApiService finApiService)
        {
            _finApiService = finApiService;
        }

        [HttpGet("/banks/{id}")]
        [SwaggerOperation(
            Summary = "Gets bank details",
            Description = "Get detailed information about the bank",
            OperationId = "Banks.Get",
            Tags = new[] {"BankEndpoint"})
        ]
        public override async Task<ActionResult<GetBankDetailsResponse>> HandleAsync(
            [FromRoute] GetBankDetailsRequest request, CancellationToken cancellationToken = default)
        {
            var sessionConfiguration = await _finApiService.BuildClientSessionConfiguration(cancellationToken);
            var client = new BanksApi(sessionConfiguration);
            var bank = await client.GetBankAsync(request.Id, cancellationToken);
            var respone = bank.Adapt<GetBankDetailsResponse>();
            return Ok(respone);
        }
    }
}
