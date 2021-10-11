using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleApi.Models
{
  public class Order
  {
    public int response_order_id { get; set; } = -1;
    ///<example>4</example>
    [RequiredAttribute]
    public string client_id { get; set; }
    ///<example>13</example>
    public int? example_order_id { get; set; }
    ///<example>34.25</example>
    [RequiredAttribute]
    public decimal value_total_netto { get; set; }
    ///<example>37.48</example>
    [RequiredAttribute]
    public decimal value_total_brutto { get; set; }
    ///<example>PayU</example>
    public string payment_type { get; set; }
    ///<example>ZK</example>
    [RequiredAttribute]
    public string document_type {get;set;}
    ///<example>0</example>
    [RequiredAttribute]
    public int delivery_type {get;set;}
    ///<example>PLN</example>
    [RequiredAttribute]
    public string currency { get; set; }
    ///<example>Comment</example>
    public string comment { get; set; }
    [RequiredAttribute]
    public List<Lane> lanes { get; set; }
    public Address address { get; set; }


    /////<example>0</example>
    //public int? status_id { get; set; }
    /////<example>Status</example>
    //public string status_name { get; set; }
    /////<example>2021-05-26</example>
    //[RequiredAttribute]
    //public string order_date { get; set; }
    /////<example>2021-05-26</example>
    //public string shipment_date { get; set; }
  }
}
