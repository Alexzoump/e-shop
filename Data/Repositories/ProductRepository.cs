using Data.Interfaces;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context; // Πρόσβαση στη βάση δεδομένων

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        // Ανακτά όλα τα προϊόντα από τη βάση δεδομένων
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync(); // Επιστρέφει μια λίστα με όλα τα προϊόντα
        }

        // Ανακτά ένα προϊόν βάσει του ID
        public async Task<Product> GetByIdAsync(int id)
        {
            var product = await _context.Products.FindAsync(id); // Αναζήτηση προϊόντος βάσει του ID
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found."); // Επιστρέφει εξαίρεση εάν το προϊόν δεν βρεθεί
            }
            return product;
        }

        // Προσθέτει ένα νέο προϊόν στη βάση δεδομένων
        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product); // Προσθήκη προϊόντος
            await _context.SaveChangesAsync(); // Αποθήκευση αλλαγών
        }

        // Ενημερώνει τα δεδομένα ενός υπάρχοντος προϊόντος
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product); // Ενημέρωση του προϊόντος στη βάση δεδομένων
            await _context.SaveChangesAsync(); // Αποθήκευση αλλαγών
        }

        // Διαγράφει ένα προϊόν βάσει του ID
        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id); // Αναζήτηση του προϊόντος
            if (product != null)
            {
                _context.Products.Remove(product); // Διαγραφή του προϊόντος
                await _context.SaveChangesAsync(); // Αποθήκευση αλλαγών
            }
        }
    }
}