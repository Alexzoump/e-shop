using Business.Interfaces;
using Data.Interfaces;
using Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository; // Χρήση repository για την αλληλεπίδραση με τα στοιχεία των προϊόντων

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // Επιστρέφει μια λίστα με όλα τα διαθέσιμα προϊόντα
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        // Ανακτά ένα προϊόν από τη βάση δεδομένων με βάση το μοναδικό αναγνωριστικό (id)
        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        // Προσθέτει ένα νέο προϊόν στη βάση δεδομένων
        public async Task AddProductAsync(Product product)
        {
            await _productRepository.AddAsync(product);
        }

        // Ενημερώνει τα δεδομένα ενός υπάρχοντος προϊόντος στη βάση δεδομένων
        public async Task UpdateProductAsync(Product product)
        {
            await _productRepository.UpdateAsync(product);
        }

        // Διαγράφει ένα προϊόν από τη βάση δεδομένων με βάση το μοναδικό αναγνωριστικό (id)
        public async Task DeleteProductAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
        }
    }
}
