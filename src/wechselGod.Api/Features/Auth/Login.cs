using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using EasyCaching.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using wechselGod.Api.Services;
using wechselGod.Infrastructure;

namespace wechselGod.Api.Features.Auth
{
    public class Login : BaseAsyncEndpoint
        .WithRequest<LoginAccountRequest>
        .WithResponse<LoginAccountResponse>
    {
        private readonly DatabaseContext _databaseContext;
        private readonly TokenService _tokenService;
        private readonly IEasyCachingProvider _cachingProvider;
        private readonly FinApiService _finApiService;
        private readonly CachingService _cachingService;

        public Login(DatabaseContext databaseContext,
            TokenService tokenService, IEasyCachingProvider cachingProvider, FinApiService finApiService,
            CachingService cachingService)
        {
            _databaseContext = databaseContext;
            _tokenService = tokenService;
            _cachingProvider = cachingProvider;
            _finApiService = finApiService;
            _cachingService = cachingService;
        }

        [HttpPost("/auth/login")]
        [SwaggerOperation(
            Summary = "Authenticates an user account",
            Description = "Authenticates an user account internally and on finAPI",
            OperationId = "Auth.Login",
            Tags = new[] {"AuthEndpoint"})
        ]

        public override async Task<ActionResult<LoginAccountResponse>> HandleAsync(
            [FromBody] LoginAccountRequest request,
            CancellationToken cancellationToken = default)
        {
            var account = await _databaseContext.UserAccounts.SingleOrDefaultAsync(x =>
                request.Email.ToLower() == x.Email.ToLower() &&
                x.Password == request.Password, cancellationToken);

            if (account == null)
                return NotFound("Account not found");

            var finApiSession = await _finApiService.Login(account.Id.ToString(), request.Password, cancellationToken);
            var accessToken = _tokenService.GenerateToken(account, finApiSession.RefreshToken);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var session = new FinApiSession(finApiSession._AccessToken, finApiSession.RefreshToken);
            await _cachingService.SaveUserSession(account.Id, session, finApiSession.ExpiresIn);
            return Ok(new LoginAccountResponse(accessToken, refreshToken));
        }
    }

    public record FinApiSession (string AccessToken, string RefreshToken);
}
