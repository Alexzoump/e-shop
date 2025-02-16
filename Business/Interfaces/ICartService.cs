using Data.Models;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface ICartService
    {
        // Επιστρέφει ένα υπάρχον καλάθι ή δημιουργεί ένα νέο για τον χρήστη με το συγκεκριμένο userId
        Task<Cart> GetCartByUserIdAsync(int userId);

        // Προσθέτει ένα προϊόν στο καλάθι
        Task AddItemAsync(CartItem item);

        // Αφαιρεί ένα προϊόν από το καλάθι
        Task RemoveItemAsync(int itemId);

        // Ενημερώνει ένα προϊόν του καλαθιού
        Task UpdateItemAsync(CartItem item);

        // Διαγράφει όλα τα προϊόντα από το καλάθι του συγκεκριμένου χρήστη
        Task ClearCartAsync(int userId);
    }

}
