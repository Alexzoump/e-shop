using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        // [ForeignKey("UserId")]
        // public User User { get; set; }

        public List<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
