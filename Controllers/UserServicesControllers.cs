using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;

    public AuthController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (await _userService.ValidateUserAsync(request.Username, request.Password))
        {
            return Ok(new { message = "Success" });
        }
        return Unauthorized(new { message = "Invalid username or password" });
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateRequest request)
    {
        var result = await _userService.CreateUserAsync(request.Username, request.Password);
        if (result)
        {
            return CreatedAtAction(nameof(GetUser), new { username = request.Username }, request);
        }
        return BadRequest(new { message = "Error creating user" });
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetUser(string username)
    {
        var user = await _userService.GetUserAsync(username);
        if (user != null)
        {
            return Ok(user);
        }
        return NotFound(new { message = "User not found" });
    }

    [HttpPut("{username}")]
    public async Task<IActionResult> UpdateUser(string username, [FromBody] UserUpdateRequest request)
    {
        var result = await _userService.UpdateUserAsync(username, request.Password);
        if (result)
        {
            return NoContent();
        }
        return NotFound(new { message = "User not found" });
    }

    [HttpDelete("{username}")]
    public async Task<IActionResult> DeleteUser(string username)
    {
        var result = await _userService.DeleteUserAsync(username);
        if (result)
        {
            return NoContent();
        }
        return NotFound(new { message = "User not found" });
    }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class UserCreateRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class UserUpdateRequest
{
    public string Password { get; set; }
}
