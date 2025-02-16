using Business.Interfaces;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context; // Πρόσβαση στη βάση δεδομένων
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<User?> RegisterAsync(string username, string email, string password)
        {
            // Έλεγχος εάν υπάρχει ήδη χρήστης με το ίδιο username ή email
            if (await _context.Users.AnyAsync(u => u.Username == username || u.Email == email))
                return null;

            // Δημιουργία ενός salt για την κρυπτογραφημένη αποθήκευση του password
            var salt = GenerateSalt();
            var passwordHash = HashPassword(password, salt);

            // Δημιουργία του χρήστη με τον προεπιλεγμένο ρόλο "Customer"
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                Salt = salt,
                Role = "Customer"
            };

            // Προσθήκη του χρήστη στη βάση δεδομένων
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            // Αναζήτηση του χρήστη στη βάση δεδομένων με βάση το username
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null || !VerifyPassword(password, user.PasswordHash, user.Salt))
                return false; // Αν δεν βρεθεί χρήστης ή ο κωδικός είναι λάθος τότε επιστρέφει false.

            // Δημιουργία των claims για το authentication token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Αυθεντικοποίηση με χρήση cookies...
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                return true; // Επιτυχής σύνδεση...
            }

            return false; // Εάν το HttpContext είναι null τότε επιστρέφει false.
        }

        public async Task LogoutAsync()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }

        // Υπολογίζει το hash ενός password χρησιμοποιώντας ένα salt
        public string HashPassword(string password, string salt)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(salt));
            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        // Επαληθεύει αν ένα συγκεκριμένο password (που δίνεται από το χρήστη) ταιριάζει με το αποθηκευμένο hash και salt
        public bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            return HashPassword(password, storedSalt) == storedHash;
        }

        // Δημιουργεί ένα τυχαίο salt για χρήση κατά τη διάρκεια του hashing του password
        private string GenerateSalt()
        {
            var randomBytes = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        // Ανακτά έναν χρήστη από τη βάση δεδομένων με βάση το email του...
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
