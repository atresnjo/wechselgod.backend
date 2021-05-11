using System;

namespace wechselGod.Domain
{
    public class UserAccount
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
    }
}
