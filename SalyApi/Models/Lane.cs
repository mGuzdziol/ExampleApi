using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleApi.Models
{
  public class Lane
  {
    ///<example>5908453487954</example>
    [RequiredAttribute]
    public string product_code { get; set; }
    ///<example>12.98</example>
    [RequiredAttribute]
    public decimal price_netto { get; set; }
    ///<example>9</example>
    [RequiredAttribute]
    public decimal quantity { get; set; }
    ///<example>23</example>
    [RequiredAttribute]
    public int vat { get; set; }
    ///<example>PLN</example>
    [RequiredAttribute]
    public string currency { get; set; }
  }
}
