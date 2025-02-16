using Data.Models;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    // Ορισμός ενός interface για την υπηρεσία αυθεντικοποίησης
    public interface IAuthService
    {
        // Δημιουργεί νέο χρήστη με τα δοθέντα username, email και password
        Task<User?> RegisterAsync(string username, string email, string password);

        // Εκτελεί τη διαδικασία σύνδεσης του χρήστη με username και password
        Task<bool> LoginAsync(string username, string password);

        // Εκτελεί την αποσύνδεση του χρήστη από το σύστημα
        Task LogoutAsync();

        Task<User?> GetUserByEmailAsync(string email);

        // Υπολογίζει το hash ενός κωδικού χρησιμοποιώντας salt
        string HashPassword(string password, string salt);

        // Ελέγχει εάν ένα συγκεκριμένο password ταιριάζει με το αποθηκευμένο hash και το salt
        bool VerifyPassword(string password, string storedHash, string storedSalt);
    }
}