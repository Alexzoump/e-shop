using Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IProductService
    {
        // Επιστρέφει μια λίστα με όλα τα διαθέσιμα προϊόντα
        Task<IEnumerable<Product>> GetAllProductsAsync();

        // Προσθέτει ένα νέο προϊόν στη βάση δεδομένων
        Task AddProductAsync(Product product);

        // Ανακτά ένα προϊόν με βάση το μοναδικό αναγνωριστικό (id)
        Task<Product> GetProductByIdAsync(int id);

        // Ενημερώνει τα δεδομένα ενός υπάρχοντος προϊόντος
        Task UpdateProductAsync(Product product);

        // Διαγράφει ένα προϊόν με βάση το μοναδικό αναγνωριστικό (id)
        Task DeleteProductAsync(int id);
    }
}
