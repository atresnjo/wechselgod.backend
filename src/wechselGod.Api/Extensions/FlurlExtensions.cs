using Flurl.Http;

namespace wechselGod.Api.Extensions
{
    public static class FlurlExtensions
    {
        public static IFlurlRequest AddAuthorizationHeader(this IFlurlRequest request, string token) =>
            request.WithHeader("Authorization", $"Bearer {token}");
    }
}
