using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        [ForeignKey("CartId")]
        [JsonIgnore]
        public Cart? Cart { get; set; }
    }
}
