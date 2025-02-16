using Business.Interfaces;
using Data.Interfaces;
using Data.Models;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services;
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository; // Χρήση repository για την αλληλεπίδραση με τα στοιχεία των χρηστών

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<AuthResult> RegisterAsync(UserRegisterDto userDto)
    {
        try
        {
            // Έλεγχος αν υπάρχει ήδη χρήστης με το ίδιο username ή email
            if (await _userRepository.ExistsAsync(userDto.Username, userDto.Email))
                return new AuthResult { Success = false, Message = "User already exists." };

            // Δημιουργία salt για την αποθήκευση του password
            var salt = GenerateSalt();

            // Δημιουργία του αντικειμένου (object) του χρήστη
            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                PasswordHash = HashPassword(userDto.Password, salt), // Hash του κωδικού με το salt
                Salt = salt // Αποθήκευση του salt
            };

            // Προσθήκη του χρήστη στη βάση δεδομένων
            await _userRepository.AddAsync(user);
            return new AuthResult { Success = true, Message = "Registration successful." };
        }
        catch (Exception ex)
        {
            // Αν υπάρξει εξαίρεση, επιστρέφει μήνυμα λάθους
            return new AuthResult { Success = false, Message = $"Error: {ex.Message}" };
        }
    }

    public async Task<AuthResult> LoginAsync(UserLoginDto loginDto)
    {
        try
        {
            // Αναζήτηση του χρήστη στη βάση δεδομένων με βάση το username
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash, user.Salt))
                return new AuthResult { Success = false, Message = "Invalid credentials." };

            return new AuthResult { Success = true, Message = "Login successful." };
        }
        catch (Exception ex)
        {
            // Αν υπάρξει εξαίρεση, επιστρέφει μήνυμα λάθους
            return new AuthResult { Success = false, Message = $"Error: {ex.Message}" };
        }
    }

    // Δημιουργία του hash από το password και το salt με χρήση SHA-256.
    private string HashPassword(string password, string salt)
    {
        using var sha256 = SHA256.Create();
        var combined = password + salt; // Συνδυασμός του password με το salt
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined)); // Υπολογισμός hash
        return Convert.ToBase64String(bytes); // Μετατροπή του hash σε συμβολοσειρά Base64
    }

    // Επαληθεύει εάν το παρεχόμενο password ταιριάζει με το αποθηκευμένο hash και salt
    private bool VerifyPassword(string password, string hash, string salt)
    {
        return HashPassword(password, salt) == hash;
    }

    // Δημιουργεί ένα τυχαίο salt για χρήση κατά το hashing του password
    private string GenerateSalt()
    {
        var saltBytes = new byte[16]; // Δημιουργία πίνακα 16 bytes
        RandomNumberGenerator.Fill(saltBytes); // Γέμισμα του πίνακα με τυχαίες τιμές
        return Convert.ToBase64String(saltBytes); // Μετατροπή του salt σε Base64
    }

}
