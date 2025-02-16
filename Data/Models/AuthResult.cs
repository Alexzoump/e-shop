using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models;
public class AuthResult
{
    public bool Success { get; set; }
    public required string Message { get; set; }
} 
