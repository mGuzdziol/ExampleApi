using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleApi.Models
{
  public class Address
  {
    [RequiredAttribute]
    public string name { get; set; }
    [RequiredAttribute]
    public string street { get; set; }
    [RequiredAttribute]
    public string zip_code { get; set; }
    [RequiredAttribute]
    public string city { get; set; }
    [RequiredAttribute]
    public string country { get; set; }
  }
}
