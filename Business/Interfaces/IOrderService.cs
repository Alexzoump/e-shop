using Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IOrderService
    {
        // Δημιουργεί μια νέα παραγγελία για το χρήστη με το συγκεκριμένο userId
        Task<Order> CreateOrderAsync(int userId, string name, string address, string paymentMethod);

        // Ανακτά μια παραγγελία βάσει του orderId του χρήστη
        Task<Order> GetOrderByIdAsync(int orderId);

        // Ανακτά όλες τις παραγγελίες που έχουν γίνει από ένα συγκεκριμένο χρήστη
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);

        // Ολοκληρώνει τη διαδικασία της παραγγελίας
        Task PlaceOrderAsync(Order order);
    }
}