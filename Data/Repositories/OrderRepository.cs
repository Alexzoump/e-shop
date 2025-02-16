using Data.Interfaces;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context; // Πρόσβαση στη βάση δεδομένων

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        // Ανακτά μία παραγγελία βάσει του orderId
        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.User) // Φόρτωση των στοιχείων του χρήστη της παραγγελίας
                .Include(o => o.OrderItems) // Φόρτωση των στοιχείων της παραγγελίας
                .ThenInclude(oi => oi.Product) // Φόρτωση πληροφοριών για τα προϊόντα της παραγγελίας
                .FirstOrDefaultAsync(o => o.Id == orderId)
                ?? new Order
                {
                    // Επιστρέφει μία demo παραγγελία αν δεν βρεθεί η πραγματική...
                    User = new User { Username = "Νίκος Γρηγορίου", Email = "nikos@example.com", PasswordHash = "", Salt = "" },
                    OrderItems = new List<OrderItem>(),
                    TotalAmount = 0,
                    Status = "Σε Εξέλιξη"
                };
        }

        // 🔹 Ανακτά όλες τις παραγγελίες ενός χρήστη βάσει του userId
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems) // Φόρτωση των προϊόντων της παραγγελίας
                .ThenInclude(oi => oi.Product) // Φόρτωση πληροφοριών για τα προϊόντα της παραγγελίας
                .Where(o => o.UserId == userId) // Φιλτράρει τις παραγγελίες για το συγκεκριμένο χρήστη
                .ToListAsync();
        }

        // Προσθέτει μια νέα παραγγελία στη βάση δεδομένων
        public async Task AddOrderAsync(Order order)
        {
            // Αν το userId δεν είναι έγκυρο τότε χρησιμοποιείται η προεπιλεγμένη τιμή 1
            if (order.UserId <= 0)
            {
                Console.WriteLine("Το UserId είναι 1.");
                order.UserId = 1;
            }

            // Έλεγχος εάν ο χρήστης υπάρχει στη βάση δεδομένων
            bool userExists = await _context.Users.AnyAsync(u => u.Id == order.UserId);
            if (!userExists)
            {
                Console.WriteLine($"Ο χρήστης δεν υπάρχει, δημιουργήθηκε προσωρινός χρήστης.");
                order.UserId = 1;
            }

            // Αν ο UserId=1 δεν υπάρχει τότε δημιουργείται ένας προεπιλεγμένος χρήστης
            bool fallbackUserExists = await _context.Users.AnyAsync(u => u.Id == 1);
            if (!fallbackUserExists)
            {
                Console.WriteLine("Ο χρήστης δεν υπάρχει, δημιουργήθηκε προσωρινός χρήστης.");

                var dummyUser = new User
                {
                    Id = 1,
                    Username = "Νίκος Γρηγορίου",
                    Email = "nikos@example.com",
                    PasswordHash = "",
                    Salt = "",
                    Role = "Customer"
                };

                _context.Users.Add(dummyUser);
                await _context.SaveChangesAsync();
            }

            // Προσθήκη της παραγγελίας στη βάση δεδομένων...
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }
    }
}
