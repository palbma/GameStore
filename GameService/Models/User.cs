using Microsoft.EntityFrameworkCore;
namespace GameService.Models
{
    [PrimaryKey(nameof(UserId))]
    public class User
    {
        public int UserId { get; init; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserPassword { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

    }
}
