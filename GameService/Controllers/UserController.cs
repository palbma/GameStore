using Azure.Core;
using GameService.Models;
using GameService.Repository.Interfaces;
using GameService.UserContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GameService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private ApplicationDbContext _dbContext;
        private readonly IUserRepository _userRepository;

        public UserController(ApplicationDbContext dbContext, IUserRepository userRepository)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO createUserRequest)
        {
            if (createUserRequest == null)
            {
                return BadRequest("null request");
            }
            var loginResponse = await _userRepository.Login(createUserRequest);

            if (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token)) 
            {
                return BadRequest("username or password incorect");
            }
            return Ok(loginResponse);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO registrationRequest)
        {
            if (registrationRequest == null)
            {
                return BadRequest("null request");
            }
            bool isUserNameUnique = _userRepository.IsUniqueUser(registrationRequest.UserName);
            if (isUserNameUnique == false) 
            {
                return BadRequest("User alredy exist");
            }
            var user = await _userRepository.Register(registrationRequest);
            return Ok(user);
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest createUserRequest, CancellationToken ct)
        //{
        //    var user = new User {
        //            createUserRequest.UserName,
        //            createUserRequest.UserEmail,
        //            createUserRequest.UserPassword,
        //            createUserRequest.Role
        //        };
        //    await _dbContext.Users.AddAsync(user, ct);
        //    await _dbContext.SaveChangesAsync(ct);
        //    return Ok();
        //}

        [HttpGet]
        public async Task<IActionResult> GetUser([FromQuery] GetGameRequest getGameRequest, CancellationToken ct)
        {
            var userQuery = _dbContext.Users.Where(n => string.IsNullOrEmpty(getGameRequest.Search)
           || n.UserName.ToLower().Contains(getGameRequest.Search.ToLower()));


            switch (getGameRequest.SortOrder)
            {
                case "desc":
                    userQuery = userQuery.OrderByDescending(SortByItem(getGameRequest.SortItem));
                    break;
                default:
                    userQuery = userQuery.OrderBy(SortByItem(getGameRequest.SortItem));
                    break;
            }

            var users = await userQuery.ToListAsync(ct);

            var userDtos = await userQuery.Select(n => new UserDto(
                n.UserId,
                n.UserName,
                n.UserEmail,
                n.UserPassword,
                n.Role
                )).ToListAsync(ct);

            return Ok(new GetUserRespone(userDtos));

        }
        private Expression<Func<User, object>> SortByItem(string? sortItem)
        {
            Expression<Func<User, object>> selectorKey;
            switch (sortItem?.ToLower())
            {
                case "name":
                    selectorKey = user => user.UserName;
                    break;
                case "email":
                    selectorKey = user => user.UserEmail;
                    break;
                default:
                    selectorKey = user => user.UserId;
                    break;
            }
            return selectorKey;
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);

            if(user == null)
            {
                return NotFound("Doesnt Exist");
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest updateUserRequest)
        {
            var user = _dbContext.Users.Find(id);

            if(user == null)
            {
                return NotFound("Not found");
            }

            if (!string.IsNullOrWhiteSpace(updateUserRequest.UserName))
            {
                user.UserName = updateUserRequest.UserName;
            }
            if (!string.IsNullOrWhiteSpace(updateUserRequest.UserName))
            {
                user.Role = updateUserRequest.Role;
            }

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
