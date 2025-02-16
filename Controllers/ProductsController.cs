using Business.Interfaces;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Ορίζει το βασικό route για τον controller
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET /api/products -> Επιστρέφει όλα τα διαθέσιμα προϊόντα
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products); // Επιστρέφει τη λίστα των προϊόντων
        }

        // GET /api/products/{id} -> Επιστρέφει ένα συγκεκριμένο προϊόν με βάση το ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound(); // Αν το προϊόν δεν βρεθεί τότε επιστρέφει "404 Not Found"
            return Ok(product); // Επιστρέφει το προϊόν
        }

        // POST /api/products -> Δημιουργεί ένα νέο προϊόν
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Product product)
        {
            await _productService.AddProductAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product); 
            // Επιστρέφει "201 Created" και το νέο προϊόν
        }

        // PUT /api/products/{id} -> Ενημερώνει ένα υπάρχον προϊόν
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Product product)
        {
            if (id != product.Id)
                return BadRequest("Mismatched product ID."); // Έλεγχος εάν το ID ταιριάζει με το προϊόν

            await _productService.UpdateProductAsync(product);
            return NoContent(); // Επιστρέφει "204 No Content" αν η ενημέρωση ήταν επιτυχής
        }

        // DELETE /api/products/{id} -> Διαγράφει ένα προϊόν με βάση το ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteProductAsync(id);
            return NoContent(); // Επιστρέφει "204 No Content" μετά τη διαγραφή
        }
    }
}