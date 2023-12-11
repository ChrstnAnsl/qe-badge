using Microsoft.AspNetCore.Mvc;
using CsharpBronze.Models.User;
using Microsoft.Extensions.Caching.Memory;

namespace CsharpBronze.Controllers.User
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly List<UserModel> _userStorage;
        private readonly IMemoryCache _cache;

        public UserController(IMemoryCache memoryCache, List<UserModel>? userData = null)
        {
            _cache = memoryCache;
            _userStorage = userData ?? new List<UserModel>();
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var userStorage = _cache.GetOrCreate("UserStorage", entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(15);
                return _userStorage;
            });

            if (userStorage.Count == 0)
            {
                return NotFound("No users found");
            }

            var response = new { users = userStorage };
            return new ObjectResult(response);
        }

        [HttpGet("{userId}", Name = "GetUser")]
        public IActionResult GetUser(int userId)
        {
            var userStorage = _cache.GetOrCreate("UserStorage", entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(15);
                return _userStorage;
            });

            Console.WriteLine(userStorage);
            var user = userStorage.Find(u => u.UserId == userId);

            if (user == null)
            {
                Console.WriteLine($"User with ID {userId} not found in user storage.");
                return NotFound($"User with ID {userId} not found");
            }

            var response = new { user };
            return new ObjectResult(response)
            {
                StatusCode = 200
            };
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] UserModel userModel)
        {
            try
            {
                if (userModel == null)
                {
                    return BadRequest("Invalid user data. Please provide valid data.");
                }

                if (userModel.Password != userModel.PasswordConfirmed)
                {
                    return BadRequest("Passwords do not match. Please re-enter passwords.");
                }

                var userStorage = _cache.GetOrCreate("UserStorage", entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromMinutes(15);
                    return _userStorage;
                });

                userModel.UserId = userStorage.Count + 1;
                userStorage.Add(userModel);

                _cache.Set("UserStorage", userStorage);

                Console.WriteLine($"User created: {userModel.Name}, ID: {userModel.UserId}");

                return Ok(new { message = "User created successfully", user = userModel });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpDelete("{userId}", Name = "DeleteUser")]
        public IActionResult DeleteUser(int userId)
        {
            var userStorage = _cache.GetOrCreate("UserStorage", entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(15);
                return _userStorage;
            });

            var user = userStorage.Find(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound($"User with ID {userId} not found");
            }

            userStorage.Remove(user);

            _cache.Set("UserStorage", userStorage);

            var response = new { user, message = $"Successfully deleted user with ID {userId}" };
            return new ObjectResult(response)
            {
                StatusCode = 200
            };
        }
    }
}
