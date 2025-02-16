using Business.Interfaces;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Ορίζει το βασικό route για το controller ως "api/auth"
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService; // Υπηρεσία αυθεντικοποίησης

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // Endpoint για την εγγραφή του χρήστη
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Έλεγχος εάν τα δεδομένα είναι έγκυρα
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Λανθασμένα στοιχεία!" });

            // Έλεγχος εάν υπάρχει ήδη χρήστης με το συγκεκριμένο email
            var existingUser = await _authService.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
                return Conflict(new { message = "Υπάρχε ήδη χρήστης με τη συγκεκριμένη διεύθυνση e-mail!" });

            // Δημιουργία του νέου χρήστη
            var user = await _authService.RegisterAsync(request.Username, request.Email, request.Password);
            if (user == null)
                return BadRequest(new { message = "Αποτυχία εγγραφής!" });

            return Ok(new { message = "Επιτυχής εγγραφή!" });
        }

        // Endpoint για τη σύνδεση του χρήστη
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Έλεγχος εάν τα δεδομένα είναι έγκυρα
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Λανθασμένα στοιχεία!" });

            // Προσπάθεια σύνδεσης του χρήστη...
            var isAuthenticated = await _authService.LoginAsync(request.Username, request.Password);
            if (!isAuthenticated)
                return Unauthorized(new { message = "Λάθος κωδικός πρόσβασης!" });

            return Ok(new { message = "Επιτυχής σύνδεση!" });
        }

        // Endpoint για την αποσύνδεση τπυ χρήστη
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return Ok(new { message = "Επιτυχής αποσύνδεση!" });
        }
    }

    // DTO (Data Transfer Object) για την εγγραφή του χρήστη
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty; // Όνομα χρήστη
        public string Email { get; set; } = string.Empty; // Email χρήστη
        public string Password { get; set; } = string.Empty; // Κωδικός χρήστη
    }

    // DTO για τη σύνδεση του χρήστη
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty; // Όνομα χρήστη
        public string Password { get; set; } = string.Empty; // Κωδικός χρήστη
    }
}