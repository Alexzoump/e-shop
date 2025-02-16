using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public string Status { get; set; } = "Σε Εξέλιξη";

        public required ICollection<OrderItem> OrderItems { get; set; }
    }
}