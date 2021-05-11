using FluentValidation;

namespace wechselGod.Api.Settings
{
    public class JwtSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
    }

    public class JwtSettingsValidator : AbstractValidator<JwtSettings>
    {
        public JwtSettingsValidator()
        {
            RuleFor(x => x.Issuer).NotEmpty();
            RuleFor(x => x.Audience).NotEmpty();
            RuleFor(x => x.SecretKey).NotEmpty();
        }
    }
}
