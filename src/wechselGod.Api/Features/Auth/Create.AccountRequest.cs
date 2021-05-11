namespace wechselGod.Api.Features.Auth
{
    public record CreateAccountRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
    }
}
