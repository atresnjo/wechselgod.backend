using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using wechselGod.Api.Settings;
using wechselGod.Domain;

namespace wechselGod.Api.Services
{
    public class TokenService
    {
        private readonly IOptions<JwtSettings> _jwtSettings;

        public TokenService(IOptions<JwtSettings> jwtSettings) => _jwtSettings = jwtSettings;

        public string GenerateToken(UserAccount account, string finApiRefreshToken)
        {
            var utcNow = DateTime.UtcNow;
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, utcNow.ToString(CultureInfo.InvariantCulture)),
                new(ClaimTypes.Name, account.Id.ToString()),
                new(ClaimTypes.Email, account.Email),
                new("finApiRefreshToken", finApiRefreshToken)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.SecretKey));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                signingCredentials: signingCredentials,
                claims: claims,
                notBefore: utcNow,
                expires: utcNow.AddHours(1),
                audience: _jwtSettings.Value.Audience,
                issuer: _jwtSettings.Value.Issuer
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
