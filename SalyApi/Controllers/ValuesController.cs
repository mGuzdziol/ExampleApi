using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ExampleApi.Models;
using ExampleApi.Repository;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ExampleApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class OrderController : ControllerBase
  {
    private readonly IOrders _orders;
    private readonly ILogger _logger;

    public OrderController(IOrders orders, ILogger<OrderController> logger)
    {
      _orders = orders;
      _logger = logger;
    }

    // POST api/Order
    ///// <param name="request"> Data w formacie: dd-MM-yyyy   \nWaluta np.: PLN, EUR,..." </param>
    /// <response code="200">Success. Returns nexo order id.</response>
    [ProducesResponseType(typeof(OrderIdResponse), 200)]
    [HttpPost]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> Post([FromBody] Order order)
    {
      try
      {
        _logger.LogDebug($"Użytkownik dodaje zamówienie. Example_id: {order.example_order_id}\n ");

        //foreach (var order in ordersList)
        //{
        OrderIdResponse orderIdResponse = new OrderIdResponse();
        orderIdResponse.OrderIdNexo = _orders.AddOrder(order);

        _logger.LogDebug($"Dodano zamówienie. response_order_id: {order.response_order_id}\n ");

        return Ok(orderIdResponse);
      }
      catch(Exception ex)
      {
        _logger.LogDebug($"Błąd podczas tworzenia zamówienia. Example_id: {order.example_order_id}\n" + ex.ToString());
        throw ex;
      }
      //}
    }

    /// <summary>
    /// Pole import_status zwraca jedną z trzech wartoiści: SUCCESS, WAITING lub ERROR. Chciałym aby ten import_status był dobrze widoczne przy wyświetleniu zamówienia. Gdy import_status == "ERROR", wtedy w error_description pojawi się opis błędu, który też musiałby być wyświetlony. Gdy status będzie różny
    /// od "ERROR", wtedy w error_description będzie null.
    /// </summary>
    [HttpGet]
    [Produces("application/json")]
    [Authorize(Roles = "User,GetOrderUser")]
    public async Task<ActionResult<List<OrderGet>>> Get()
    {
      var list = await _orders.GetOrders();
      _orders.GetErrors(list);
      //_orders.GetAdresses(list);
      //_orders.GetLanes(list);
      return list;
    }

  }
}
