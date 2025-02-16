using Data;
using Data.Interfaces;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context; // Πρόσβαση στη βάση δεδομένων

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    // Ελέγχει αν υπάρχει χρήστης με το συγκεκριμένο username ή email
    public async Task<bool> ExistsAsync(string username, string email)
    {
        return await _context.Users.AnyAsync(u => u.Username == username || u.Email == email);
    }

    // Προσθέτει έναν νέο χρήστη στη βάση δεδομένων
    public async Task AddAsync(User user)
    {
        _context.Users.Add(user); // Προσθήκη χρήστη στη βάση δεδομένων
        await _context.SaveChangesAsync(); // Αποθήκευση αλλαγών στη βάση δεδομένων
    }

    // Ανακτά έναν χρήστη από τη βάση δεδομένων με βάση του username
    public async Task<User> GetByUsernameAsync(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username); // Αναζήτηση χρήστη
        if (user == null)
            throw new InvalidOperationException($"Δε βρέθηκε χρήστης με όνομα '{username}'!"); // Επιστρέφει εξαίρεση αν δεν βρεθεί...

        return user;
    }
}
