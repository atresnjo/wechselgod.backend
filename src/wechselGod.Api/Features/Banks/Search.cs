using System.Collections.Generic;
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
    public record BankDetails(long Id, string Name, string Blz, string Bic);

    public class Search : BaseAsyncEndpoint
        .WithRequest<SearchBankRequest>
        .WithResponse<SearchBankResponse>
    {
        private readonly FinApiService _finApiService;

        public Search(FinApiService finApiService)
        {
            _finApiService = finApiService;
        }

        [HttpGet("/banks/search")]
        [SwaggerOperation(
            Summary = "Searches banks by name",
            Description = "Searches all available banks with the requested search term",
            OperationId = "Banks.Search",
            Tags = new[] {"BankEndpoint"})
        ]
        public override async Task<ActionResult<SearchBankResponse>> HandleAsync([FromQuery] SearchBankRequest request,
            CancellationToken cancellationToken = default)
        {
            var sessionConfiguration = await _finApiService.BuildClientSessionConfiguration(cancellationToken);
            var client = new BanksApi(sessionConfiguration);
            var banks = await client.GetAndSearchAllBanksAsync(search: request.SearchTerm,
                cancellationToken: cancellationToken);

            var respone = banks.Banks.Adapt<IReadOnlyList<BankDetails>>();
            return Ok(respone);
        }
    }
}
