using Microsoft.AspNetCore.Mvc;
using TodoAPI.Helpers;
using TodoAPI.Models;
using TodoAPI.Services;

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }


        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                var json = await _userService.GetAll();

                return Ok(json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound(new { message = "Some thing was wrong when getting users" });
            }

        }

        // GET: api/Users/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(long id)
        {
            var json = await _userService.GetById(id);


            return json;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(long id, User user)
        {
            if (id != user.UserID)
            {
                return NotFound("User doesn't exist");
            }

            try
            {
                var json = await _userService.UpdateById(id, user);
                return json;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, new { message = " Server Error" });
            }
        }

        // POST: api/Users/register
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("register")]
        public async Task<IActionResult> Register([Bind("Name,username,password")] User user)
        {
            var json = await _userService.Register(user);
            if (json == null)
            {
                return BadRequest();
            }



            return json;
        }



        [HttpPost("login")]
        public ActionResult Login(string username, string password)
        {
            try
            {
                var token = _userService.Authenticate(username, password);
                if (string.IsNullOrEmpty(token))
                {
                    return Ok("Wrong password");
                }
                return Ok(new { token });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, new { message = " Server Error" });
            }

        }



        //private bool UserExists(long id)
        //{
        //    return (_context.Users?.Any(e => e.UserID == id)).GetValueOrDefault();
        //}

    }
}
