using Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart> GetCartByUserIdAsync(int userId);
        Task AddItemAsync(CartItem item);
        Task RemoveItemAsync(int itemId);
        Task UpdateItemAsync(CartItem item);
        Task ClearCartAsync(int userId);
        Task<IEnumerable<CartItem>> GetCartItemsAsync(int userId);
    }
}
