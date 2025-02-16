using Business.Interfaces;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/orders")] // Ορίζει το βασικό route για τον controller
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET /api/orders/{orderId} -> Λαμβάνει τις λεπτομέρειες μιας συγκεκριμένης παραγγελίας
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var userId = GetUserId() ?? 1; // Αν δεν βρεθεί το userId τότε χρησιμοποιείται η προεπιλεγμένη τιμή 1

            var order = await _orderService.GetOrderByIdAsync(orderId);

            return Ok(order); // Επιστρέφει τις λεπτομέρειες της παραγγελίας
        }

        // GET /api/orders/user -> Επιστρέφει όλες τις παραγγελίες του συνδεδεμένου χρήστη
        [HttpGet("user")]
        public async Task<IActionResult> GetOrdersByUserId()
        {
            var userId = GetUserId() ?? 1; // Αν δεν βρεθεί το userId τότε χρησιμοποιείται η προεπιλεγμένη τιμή 1

            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders); // Επιστρέφει όλες τις παραγγελίες του χρήστη
        }

        // POST /api/orders/checkout -> Δημιουργεί νέα παραγγελία από το καλάθι του χρήστη
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDto request)
        {
            var userId = GetUserId() ?? 1; // Αν δεν βρεθεί το userId τότε χρησιμοποιείται η προεπιλεγμένη τιμή 1

            var order = await _orderService.CreateOrderAsync(
                userId, request.Name, request.Address, request.PaymentMethod
            );

            return Ok(new { message = "Η παραγγελία ολοκληρώθηκε επιτυχώς!", order });
        }

        // GET /api/orders/status/{orderId} -> Παίρνει την κατάσταση μιας παραγγελίας
        [HttpGet("status/{orderId}")]
        public async Task<IActionResult> GetOrderStatus(int orderId)
        {
            var userId = GetUserId() ?? 1; // Αν δεν βρεθεί το userId τότε χρησιμοποιείται η προεπιλεγμένη τιμή 1

            var order = await _orderService.GetOrderByIdAsync(orderId);

            return Ok(new { order.Id, order.Status }); // Επιστρέφει το ID και την κατάσταση της παραγγελίας
        }

        // Βοηθητική μέθοδος, δε χρησιμοποιείται, ήταν για το JWT...
        private int? GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Ανακτά το userId από το JWT token
            return int.TryParse(userId, out int parsedUserId) ? parsedUserId : null; // Μετατροπή του userId σε έναν ακέραιο αριθμό
        }
    }
}