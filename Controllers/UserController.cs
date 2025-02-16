using Microsoft.AspNetCore.Mvc;
using Business.Interfaces;
using Data.Models;
using System.Threading.Tasks;

namespace Controllers;

[ApiController]
[Route("api/[controller]")] // Ορίζει το βασικό route για τον controller
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    // POST /api/user/register -> Δημιουργεί ένα νέο χρήστη (εγγραφή)
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto userDto)
    {
        var result = await _userService.RegisterAsync(userDto);
        if (!result.Success)
            return BadRequest(result.Message); // Αν η εγγραφή αποτύχει τότε επιστρέφει "400 Bad Request"

        return Ok(result); // Αν η εγγραφή είναι επιτυχής τότε επιστρέφει "200 OK"
    }

    // POST /api/user/login -> Συνδέει έναν υπάρχοντα χρήστη
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto loginDto)
    {
        var result = await _userService.LoginAsync(loginDto);
        if (!result.Success)
            return Unauthorized(result.Message); // Αν ο κωδικός πρόσβασης είναι λάθος τότε επιστρέφει "401 Unauthorized"

        return Ok(result); // Αν η σύνδεση είναι επιτυχής τότε επιστρέφει "200 OK"
    }
}
