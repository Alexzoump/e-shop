using Data.Interfaces;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context; // Πρόσβαση στη βάση δεδομένων

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        // Ανακτά το καλάθι του χρήστη βάσει του userId
        public async Task<Cart> GetCartByUserIdAsync(int userId)
        {
            // Αν το userId δεν είναι έγκυρο τότε χρησιμοποιείται η προεπιλεγμένη τιμή 1
            if (userId <= 0)
            {
                Console.WriteLine("Το UserId είναι 1.");
                userId = 1;
            }

            // Έλεγχος εάν ο χρήστης υπάρχει στη βάση δεδομένων
            bool userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                Console.WriteLine("Ο χρήστης δεν υπάρχει, δημιουργήθηκε προσωρινός χρήστης.");
                userId = 1;
            }

            // Αναζήτηση του καλαθιού του χρήστη και λήψη των προϊόντων...
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            // Αν το καλάθι δεν υπάρχει, δημιουργείται νέο...
            if (cart == null)
            {
                cart = new Cart { UserId = userId, Items = new List<CartItem>() };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        // Προσθήκη ενός προϊόντος στο καλάθι..
        public async Task AddItemAsync(CartItem item)
        {
            // Αναζήτηση του προϊόντος και λήψη της τιμής...
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException("Το προϊόν δεν βρέθηκε.");
            }

            // Αντιγραφή της τιμής του προϊόντος στο καλάθι
            item.Price = product.Price;

            await _context.CartItems.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        // Αφαίρεση του προϊόντος από το καλάθι με βάση του itemId
        public async Task RemoveItemAsync(int itemId)
        {
            var item = await _context.CartItems.FindAsync(itemId);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        // Ενημέρωση ενός προϊόντος στο καλάθι, π.χ. ποσότητα
        public async Task UpdateItemAsync(CartItem item)
        {
            var existingItem = await _context.CartItems.FindAsync(item.Id);
            if (existingItem != null)
            {
                existingItem.Quantity = item.Quantity;
                existingItem.Price = item.Price;
                await _context.SaveChangesAsync();
            }
        }

        // Επιστροφή όλων των προϊόντων που βρίσκονται στο καλάθι του χρήστη...
        public async Task<IEnumerable<CartItem>> GetCartItemsAsync(int userId)
        {
            var cart = await GetCartByUserIdAsync(userId);
            return cart?.Items ?? new List<CartItem>(); // Επιστρέφει τα προϊόντα ή μια κενή λίστα
        }

        // Διαγραφή όλων των προϊόντων από το καλάθι του χρήστη...
        public async Task ClearCartAsync(int userId)
        {
            var cart = await GetCartByUserIdAsync(userId);
            if (cart != null && cart.Items.Any()) // Αν υπάρχουν προϊόντα στο καλάθι...
            {
                _context.CartItems.RemoveRange(cart.Items); // Διαγραφή όλων των προϊόντων...
                await _context.SaveChangesAsync();
            }
        }
    }
}