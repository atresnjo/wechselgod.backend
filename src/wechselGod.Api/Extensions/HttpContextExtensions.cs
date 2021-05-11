using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using wechselGod.Api.Exceptions;

namespace wechselGod.Api.Extensions
{
    public static class HttpContextExtensions
    {
        public static Guid GetAuthId(this HttpContext httpContext)
        {
            var hasName = httpContext?.User.HasClaim(x => x.Type == ClaimTypes.Name);
            if (!hasName.HasValue || !hasName.Value)
                return default;

            var userIdClaim = httpContext.User.FindFirst(y => y.Type == ClaimTypes.Name);
            var claim = userIdClaim != null ? Guid.Parse(userIdClaim.Value) : default;
            if (claim == default)
                throw new AuthIdMissingException();

            return claim;
        }

        public static string GetAuthEmail(this HttpContext httpContext)
        {
            var hasName = httpContext?.User.HasClaim(x => x.Type == ClaimTypes.Email);
            if (!hasName.HasValue || !hasName.Value)
                return default;

            var userIdClaim = httpContext.User.FindFirst(y => y.Type == ClaimTypes.Email);
            if (userIdClaim == null)
                throw new AuthIdMissingException();

            var claim = userIdClaim.Value;
            if (string.IsNullOrEmpty(claim))
                throw new AuthIdMissingException();

            return claim;
        }
        public static string GetFinApiRefreshToken(this HttpContext httpContext)
        {
            var hasName = httpContext?.User.HasClaim(x => x.Type == "finApiRefreshToken");
            if (!hasName.HasValue || !hasName.Value)
                return default;

            var userIdClaim = httpContext.User.FindFirst(y => y.Type == "finApiRefreshToken");
            if (userIdClaim == null)
                throw new AuthIdMissingException();

            var claim = userIdClaim.Value;
            if (string.IsNullOrEmpty(claim))
                throw new AuthIdMissingException();

            return claim;
        }
    }
}
