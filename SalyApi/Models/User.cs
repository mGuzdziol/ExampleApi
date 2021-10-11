using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using ExampleApi.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleApi.Models
{
  public class User
  {
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Role { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;


  }
}
