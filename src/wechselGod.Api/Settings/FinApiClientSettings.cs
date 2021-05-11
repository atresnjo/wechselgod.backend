using FluentValidation;

namespace wechselGod.Api.Settings
{
    public class FinApiClientSettings
    {
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string DataDecryptionKey { get; set; }
    }

    public class FinApiClientSettingsValidator : AbstractValidator<FinApiClientSettings>
    {
        public FinApiClientSettingsValidator()
        {
            RuleFor(x => x.ClientId).NotEmpty().NotNull();
            RuleFor(x => x.Secret).NotEmpty().NotNull();
            RuleFor(x => x.DataDecryptionKey).NotEmpty().NotNull();
        }
    }
}
