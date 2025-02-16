using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Stock { get; set; }

        public string ImageUrl { get; set; } = "https://placehold.co/200"; // Η προεπιλεγμένη εικόνα για τα προϊόντα...
    }
}