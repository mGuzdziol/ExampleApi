using ExampleApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleApi.Repository
{
  public interface IOrders
  {
    public int? AddOrder(Order order);
    public Task<List<OrderGet>> GetOrders();
    //public void GetAdresses(List<OrderGet> orders);
    //public void GetLanes(List<OrderGet> ordersList);
    public void GetErrors(List<OrderGet> ordersList);
  }
}
