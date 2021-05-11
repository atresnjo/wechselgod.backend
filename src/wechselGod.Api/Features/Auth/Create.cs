using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Model;
using Swashbuckle.AspNetCore.Annotations;
using wechselGod.Api.Services;
using wechselGod.Domain;
using wechselGod.Infrastructure;

namespace wechselGod.Api.Features.Auth
{
    public class Create : BaseAsyncEndpoint
        .WithRequest<CreateAccountRequest>
        .WithResponse<CreateAccountResponse>
    {
        private readonly DatabaseContext _databaseContext;
        private readonly TokenService _tokenService;
        private readonly FinApiService _finApiService;

        public Create(DatabaseContext databaseContext,
            TokenService tokenService, FinApiService finApiService)
        {
            _databaseContext = databaseContext;
            _tokenService = tokenService;
            _finApiService = finApiService;
        }

        [HttpPost("/auth/create")]
        [SwaggerOperation(
            Summary = "Creates an user account",
            Description = "Creates an user account internally and on finAPI",
            OperationId = "Auth.Create",
            Tags = new[] {"AuthEndpoint"})
        ]

        public override async Task<ActionResult<CreateAccountResponse>> HandleAsync(
            [FromBody] CreateAccountRequest request,
            CancellationToken cancellationToken = default)
        {
            var sessionConfiguration = await _finApiService.BuildClientSessionConfiguration(cancellationToken);
            var clientApi = new UsersApi(sessionConfiguration);

            var account = request.Adapt<UserAccount>();
            await _databaseContext.UserAccounts.AddAsync(account, cancellationToken);
            await _databaseContext.SaveChangesAsync(cancellationToken);

            var userCreateParams = account.Adapt<UserCreateParams>();
            var response = await clientApi.CreateUserAsync(userCreateParams, cancellationToken);
            var userSession = await _finApiService.Login(response.Id, response.Password, cancellationToken);

            var accessToken = _tokenService.GenerateToken(account, userSession.RefreshToken);
            var refreshToken = _tokenService.GenerateRefreshToken();
            return Ok(new CreateAccountResponse(accessToken, refreshToken));
        }
    }
}
