using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        public required string Salt { get; set; }

        [Required]
        public string Role { get; set; } = "Customer"; // Ο προεπιλεγμένος ρόλος για τον κάθε χρήστη...
    }
}
