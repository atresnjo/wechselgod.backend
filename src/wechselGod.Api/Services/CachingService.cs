using System;
using System.Threading.Tasks;
using EasyCaching.Core;
using wechselGod.Api.Features.Auth;

namespace wechselGod.Api.Services
{
    public class CachingService
    {
        private readonly IEasyCachingProvider _cachingProvider;

        public CachingService(IEasyCachingProvider cachingProvider) => _cachingProvider = cachingProvider;

        public async Task SaveUserSession(Guid accountId, FinApiSession finApiSession, int expiresIn) =>
            await _cachingProvider.SetAsync(accountId.ToString(), finApiSession, TimeSpan.FromSeconds(expiresIn));

        public async Task<CacheValue<FinApiSession>> GetUserSession(Guid accountId) =>
            await _cachingProvider.GetAsync<FinApiSession>(accountId.ToString());

        public async Task SaveClientSession(string accessToken) =>
            await _cachingProvider.SetAsync("client", accessToken, TimeSpan.FromHours(1));

        public async Task<CacheValue<string>> GetClientSession() =>
            await _cachingProvider.GetAsync<string>("client");
    }
}
