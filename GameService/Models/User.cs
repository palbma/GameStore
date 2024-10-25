using Microsoft.EntityFrameworkCore;
namespace GameService.Models
{
    [PrimaryKey(nameof(UserId))]
    public class User
    {
        public int UserId { get; init; }
        public string UserName { get; init; } = string.Empty;
        public string UserEmail { get; init; } = string.Empty;
        public string UserPassword { get; init; } = string.Empty;
        public string Role { get; init; } = string.Empty;

        public User(string userName, string userEmail, string userPassword, string role)
        {
            UserName = userName;
            UserEmail = userEmail;
            UserPassword = userPassword;
            Role = role;
        }
    }
}
