using GameService.Models;
using GameService.Repository.Interfaces;
using GameService.UserContracts;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GameService.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private string secretKey;

        public UserRepository(ApplicationDbContext context,IConfiguration configuration)
        {
            _context = context;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        public bool IsUniqueUser(string username)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserName == username);
            if (user == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<LoginResponeDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _context.Users.FirstOrDefault(
            l => l.UserName.ToLower() == loginRequestDTO.UserName.ToLower()
            && l.UserPassword == loginRequestDTO.Password);

            if (user == null) 
            {
                return null;
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name,user.UserId.ToString()),
                    new Claim(ClaimTypes.Role,user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(30),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponeDTO loginResponeDTO = new LoginResponeDTO()
            {
                Token = tokenHandler.WriteToken(token),
                User = user,
            };
            return loginResponeDTO;
        }

        public async Task<User> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            var user = new User()
            {
                UserName = registrationRequestDTO.UserName,
                UserEmail = registrationRequestDTO.Email,
                UserPassword = registrationRequestDTO.Password,
                Role = registrationRequestDTO.Role
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            user.UserPassword = string.Empty;
            return user;
        }
    }
}
