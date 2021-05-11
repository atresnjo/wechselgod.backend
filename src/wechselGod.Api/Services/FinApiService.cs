using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EasyCaching.Core;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;
using wechselGod.Api.Extensions;
using wechselGod.Api.Features.Accounts;
using wechselGod.Api.Features.Auth;
using wechselGod.Api.Services.Models;
using wechselGod.Api.Settings;
using wechselGod.Infrastructure;

namespace wechselGod.Api.Services
{
    public class FinApiService
    {
        private const string _baseUrl = "https://sandbox.finapi.io/api/v1/";
        private readonly IFlurlClientFactory _flurlClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEasyCachingProvider _cachingProvider;
        private readonly DatabaseContext _databaseContext;
        private readonly FinApiClientSettings _settings;
        private readonly ILogger<FinApiService> _logger;
        private readonly CachingService _cachingService;

        public FinApiService(IOptions<FinApiClientSettings> settings, IFlurlClientFactory flurlClientFactory,
            IHttpContextAccessor httpContextAccessor, DatabaseContext databaseContext,
            IEasyCachingProvider cachingProvider, ILogger<FinApiService> logger, CachingService cachingService)
        {
            _flurlClientFactory = flurlClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _databaseContext = databaseContext;
            _cachingProvider = cachingProvider;
            _logger = logger;
            _cachingService = cachingService;
            _settings = settings.Value;
        }

        public async Task<string> ImportBank(long id, CancellationToken cancellationToken = default)
        {
            var sessionConfiguration = await BuildUserSessionConfiguration(cancellationToken);
            var client = _flurlClientFactory.Get(_baseUrl);
            var request = client.Request("/bankConnections/import")
                .AddAuthorizationHeader(sessionConfiguration.AccessToken);
            try
            {
                var accountTypes = new List<string>
                    {"Checking", "Savings", "CreditCard", "Security", "Loan", "Pocket", "Membership", "Bausparen"};

                var importBankPayload = new FinImportBankRequest(id, "Test connection new", "XS2A", accountTypes);
                await request.PostJsonAsync(importBankPayload, cancellationToken);
            }
            catch (FlurlHttpException ex)
            {
                if (ex.StatusCode == 451)
                {
                    var location = ex.Call.Response.Headers.FirstOrDefault("Location");
                    return location;
                }

                var payload = await ex.GetResponseStringAsync();
                _logger.LogError(ex, payload);
            }

            return null;
        }

        public async Task<Configuration> BuildUserSessionConfiguration(CancellationToken cancellationToken = default)
        {
            var id = _httpContextAccessor.HttpContext.GetAuthId();
            var session = await _cachingService.GetUserSession(id);
            if (session.HasValue)
            {
                return new Configuration {AccessToken = session.Value.AccessToken};
            }

            var email = _httpContextAccessor.HttpContext.GetAuthEmail();
            var user = await _databaseContext.UserAccounts.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

            var finApiSession = await Login(user.Id.ToString(), user.Password, cancellationToken);

            var newSession = new FinApiSession(finApiSession._AccessToken, finApiSession.RefreshToken);
            await _cachingProvider.SetAsync(user.Id.ToString(), newSession,
                TimeSpan.FromHours(finApiSession.ExpiresIn));

            return new Configuration {AccessToken = finApiSession._AccessToken};
        }

        public async Task<Configuration> BuildClientSessionConfiguration(CancellationToken cancellationToken = default)
        {
            var session = await _cachingService.GetClientSession();
            if (session.HasValue)
            {
                return new Configuration {AccessToken = session.Value};
            }

            try
            {
                var auth = new AuthorizationApi();
                var fintechSession = await auth.GetTokenAsync("client_credentials", _settings.ClientId,
                    _settings.Secret, cancellationToken: cancellationToken);

                await _cachingService.SaveClientSession(fintechSession._AccessToken);
                return new Configuration {AccessToken = fintechSession._AccessToken};
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex.ToString());
            }

            return null;
        }

        public async Task<FinGetAllBankAccountsResponse> GetAllBankAccounts(CancellationToken cancellationToken = default)
        {
            var sessionConfiguration = await BuildUserSessionConfiguration(cancellationToken);
            var client = _flurlClientFactory.Get(_baseUrl);
            var request = client.Request("/accounts").AddAuthorizationHeader(sessionConfiguration.AccessToken);

            try
            {
                return await request.GetJsonAsync<FinGetAllBankAccountsResponse>(cancellationToken);
            }
            catch (FlurlHttpException ex)
            {
                var payload = await ex.GetResponseStringAsync();
                _logger.LogError(ex, payload);
            }

            return null;
        }

        public async Task<FinGetAllBankTransactionsResponse> GetAllBankTransactions(long id,
            CancellationToken cancellationToken = default)
        {
            var sessionConfiguration = await BuildUserSessionConfiguration(cancellationToken);
            var client = _flurlClientFactory.Get(_baseUrl);
            var request = client.Request("/transactions")
                .AddAuthorizationHeader(sessionConfiguration.AccessToken)
                .SetQueryParam("view", "userView")
                .SetQueryParam("order", "finapiBookingDate,desc")
                .SetQueryParam("accountIds", id);

            try
            {
                return await request.GetJsonAsync<FinGetAllBankTransactionsResponse>(cancellationToken);
            }
            catch (FlurlHttpException ex)
            {
                var payload = await ex.GetResponseStringAsync();
                _logger.LogError(ex, payload);
            }

            return null;
        }

        public async Task<AccessToken> Login(string id, string password, CancellationToken cancellationToken = default)
        {
            try
            {
                var auth = new AuthorizationApi();
                return await auth.GetTokenAsync("password", _settings.ClientId,
                    _settings.Secret,
                    null, id, password, cancellationToken);

            }
            catch (ApiException ex)
            {
                _logger.LogError(ex.ToString());
            }

            return null;
        }
    }
}
