using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models;
public class UserLoginDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}