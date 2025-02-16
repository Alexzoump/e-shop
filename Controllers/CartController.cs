using Business.Interfaces;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // Διαβάζει το καλάθι ενός συγκεκριμένου χρήστη
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(int userId)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null || cart.Items.Count == 0)
                return NotFound(new { message = "Το καλάθι είναι άδειο ή δε βρέθηκε!" });

            return Ok(cart);
        }

        // Προσθήκη του προϊόντος στο καλάθι χρησιμοποιώντας το AddItemDto
        [HttpPost("add")]
        public async Task<IActionResult> AddItem([FromBody] AddItemDto dto)
        {
            // Εύρεση του καλαθιού για το συγκεκριμένο χρήση
            var cart = await _cartService.GetCartByUserIdAsync(dto.UserId);
            if (cart == null)
            {
                return BadRequest(new { message = "Δεν υπάρχει χρήστης ή καλάθι που να σχετίζονται!" });
            }

            // Δημιουργούμε το αντικείμενο του καλαθιού
            var item = new CartItem
            {
                CartId = cart.Id,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                Price = dto.Price
            };

            // Προσθήκη στη βάση δεδομένων
            await _cartService.AddItemAsync(item);

            return Ok(new { message = $"Το προϊόν {dto.ProductId} προστέθηκε στο καλάθι (User {dto.UserId})." });
        }

        // Αφαίρεση ενός προϊόντος από το καλάθι
        [HttpDelete("remove/{itemId}")]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            await _cartService.RemoveItemAsync(itemId);
            return Ok(new { message = "Το προϊόν αφαιρέθηκε από το καλάθι!" });
        }

        // Διαγραφή όλων των προϊόντων του καλαθιού
        [HttpDelete("clear/{userId}")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            await _cartService.ClearCartAsync(userId);
            return Ok(new { message = "Το καλάθι σας είναι πλέον άδειο!" });
        }
    }

    // DTO για προσθήκη του προϊόντος στο καλάθι
    public class AddItemDto
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
