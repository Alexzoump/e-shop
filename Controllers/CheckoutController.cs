using Business.Interfaces;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Ορίζει το βασικό route για τον controller
    public class CheckoutController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public CheckoutController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // Endpoint για την ολοκλήρωση της αγοράς
        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto checkoutDto)
        {
            // Λήψη του userId
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                return BadRequest("Ο χρήστης δεν είναι έγκυρος!"); // Αν ο χρήστης δεν είναι έγκυρος τότε επιστρέφει BadRequest

            // Δημιουργία παραγγελίας για το χρήστη με κάποια βασικά (τυχαία) στοιχεία
            var order = await _orderService.CreateOrderAsync(userId, "Default Name", "Default Address", "Cash");

            return Ok(order); // Επιστρέφει την ολοκληρωμένη παραγγελία
        }
    }

    // DTO (Data Transfer Object) για τα δεδομένα του Checkout
    public class CheckoutDto
    {
        public required string Name { get; set; } // Όνομα παραλήπτη
        public required string Address { get; set; } // Διεύθυνση αποστολής
        public required string PaymentMethod { get; set; } // Μέθοδος πληρωμής
    }
}