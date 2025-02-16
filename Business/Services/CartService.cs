using Business.Interfaces;
using Data.Interfaces;
using Data.Models;
using System.Threading.Tasks;

namespace Business.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository; // Χρήση repository για την αλληλεπίδραση με τα δεδομένα του καλαθιού

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        // Ανακτά το καλάθι για έναν συγκεκριμένο χρήστη με βάση το userId
        public async Task<Cart> GetCartByUserIdAsync(int userId)
        {
            return await _cartRepository.GetCartByUserIdAsync(userId);
        }

        // Προσθέτει ένα προϊόν (CartItem) στο καλάθι
        public async Task AddItemAsync(CartItem item)
        {
            await _cartRepository.AddItemAsync(item);
        }

        // Αφαιρεί ένα προϊόν (CartItem) από το καλάθι βάσει του itemId
        public async Task RemoveItemAsync(int itemId)
        {
            await _cartRepository.RemoveItemAsync(itemId);
        }

        // Ενημερώνει ένα προϊόν που βρίσκεται στο καλάθι (π.χ. ποσότητα)
        // Αυτή η μέθοδος υπάρχει για το ICartService interface
        public async Task UpdateItemAsync(CartItem item)
        {
            await _cartRepository.UpdateItemAsync(item);
        }

        // Αφαιρεί όλα τα προϊόντα από το καλάθι ενός συγκεκριμένου χρήστη
        public async Task ClearCartAsync(int userId)
        {
            await _cartRepository.ClearCartAsync(userId);
        }
    }
}