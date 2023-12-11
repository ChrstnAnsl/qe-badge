using System.Text.Json.Serialization;

namespace CsharpBronze.Models.User
{
    public class UserModel
    {
        [JsonIgnore]
        public int UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? PasswordConfirmed { get; set; }

    }
}
