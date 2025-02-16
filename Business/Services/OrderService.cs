using Business.Interfaces;
using Data;
using Data.Interfaces;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services
{
     public class OrderService : IOrderService
    {
        private readonly ICartRepository _cartRepository; // Χρήση repository για την αλληλεπίδραση με το καλάθι αγορών
        private readonly AppDbContext _context; // Για την πρόσβαση στη βάση δεδομένων...

        public OrderService(ICartRepository cartRepository, AppDbContext context)
        {
            _cartRepository = cartRepository;
            _context = context;
        }

        // Δημιουργεί μια νέα παραγγελία για τον χρήστη με το δοθέν userId
        public async Task<Order> CreateOrderAsync(int userId, string name, string address, string paymentMethod)
        {
            // Αν δεν έχει δοθεί έγκυρο userId, τότε χρησιμοποιείται η προεπιλεγμένη τιμή 1
            userId = userId > 0 ? userId : 1;

            // Ανάκτηση του καλαθιού αγορών για το συγκεκριμένο χρήστη
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
                throw new InvalidOperationException("Το καλάθι δεν βρέθηκε."); // Αν το καλάθι δεν υπάρχει, πετάει εξαίρεση

            // Δημιουργία του αντικειμένου Order με τις απαραίτητες πληροφορίες
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow, // Χρησιμοποιούμε την τρέχουσα ημερομηνία και ώρα
                TotalAmount = cart.Items.Sum(item => item.Price * item.Quantity), // Υπολογισμός του συνολικού ποσού της παραγγελίας
                Status = "Σε Εξέλιξη", // Αρχική κατάσταση της παραγγελίας
                OrderItems = cart.Items.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product.Price, // Αποθήκευση της τιμής του προϊόντος τη στιγμή της παραγγελίας
                    Product = item.Product, // Διατήρηση αναφοράς στο προϊόν
                    Order = null // Η παραγγελία δεν έχει ακόμα συσχετιστεί
                }).ToList()
            };

            // Προσθήκη της παραγγελίας στη βάση δεδομένων
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            // Καθαρισμός του καλαθιού μετά την ολοκλήρωση της παραγγελίας
            await _cartRepository.ClearCartAsync(userId);

            return order; // Επιστροφή του αντικειμένου Order
        }

        // Ανακτά μια παραγγελία από τη βάση δεδομένων με βάση το orderId
        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.User) // Συμπεριλαμβάνει πληροφορίες για τον χρήστη της παραγγελίας
                .Include(o => o.OrderItems) // Συμπεριλαμβάνει τα στοιχεία της παραγγελίας
                .ThenInclude(oi => oi.Product) // Συμπεριλαμβάνει τις πληροφορίες των προϊόντων
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        // Ανακτά όλες τις παραγγελίες ενός συγκεκριμένου χρήστη
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        // Ολοκληρώνει την παραγγελία και την αποθηκεύει στη βάση δεδομένων
        public async Task PlaceOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }
    }
}