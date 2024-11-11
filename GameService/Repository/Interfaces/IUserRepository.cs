using GameService.Models;
using GameService.UserContracts;

namespace GameService.Repository.Interfaces
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string username);
        Task<LoginResponeDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<User> Register(RegistrationRequestDTO registrationRequestDTO);
    }
}
