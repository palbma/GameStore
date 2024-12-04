using GameService.Models;

namespace GameService.UserContracts
{
    public class LoginResponeDTO
    {
        public User User { get; set; }

        public string Token { get; set; }
    }
}
