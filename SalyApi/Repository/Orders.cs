using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using ExampleApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleApi.Repository
{
  public class Orders : IOrders
  {
    public IConfiguration _configuration;

    public Orders(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public async Task<List<OrderGet>>GetOrders()
    {
      List<OrderGet> ordersList = new List<OrderGet>();

      Response<List<OrderGet>> response = new Response<List<OrderGet>>();
      IDbConnection db = new SqliteConnection(_configuration.GetConnectionString("default"));
      SQLitePCL.Batteries_V2.Init();
      string query = @"Select response_order_id
      ,example_order_id
      ,client_id
      ,CAST(value_total_netto AS REAL) AS value_total_netto
      ,CAST(value_total_brutto AS REAL) AS value_total_brutto
      ,payment_type,document_type
      ,delivery_type
      ,currency
      ,comment
      ,import_date
      ,import_status " +
        "FROM orders_h";
      ordersList = db.Query<OrderGet>(query, null, commandType: CommandType.Text).ToList();
      db.Close();

      List<OrderGet> bla = new List<OrderGet>();

      foreach(var el in ordersList)
      {
        bla.Add(el);
      }
      
      return bla;
    }

    public async void GetErrors(List<OrderGet> ordersList)
    {
      IDbConnection db = new SqliteConnection(_configuration.GetConnectionString("default"));
      SQLitePCL.Batteries_V2.Init();

      foreach (var ord in ordersList)
      {
        if (ord.import_status == "ERROR")
        {
          string query = $"SELECT error_msg FROM orders_error WHERE order_id = {ord.response_order_id} ORDER BY id desc LIMIT 1";
          var reader = db.ExecuteReader(query);
          while (reader.Read())
          {
            ord.error_description = reader.GetString(0);
          }
        }
      }

    }

    //public async void GetAdresses(List<OrderGet> ordersList)
    //{
    //  IDbConnection db = new SqliteConnection(_configuration.GetConnectionString("default"));
    //  SQLitePCL.Batteries_V2.Init();

    //  foreach (var ord in ordersList)
    //  {
    //    string query = $"SELECT address_name,address_street,address_zip_code,address_city,address_country FROM orders_h WHERE response_order_id = {ord.response_order_id}";
    //    var reader = db.ExecuteReader(query);
    //    while (reader.Read())
    //    {
    //      var address = new Address()
    //      {
    //        name = reader.IsDBNull(0) ? "" : reader.GetString(0),
    //        street = reader.IsDBNull(1) ? "" : reader.GetString(1),
    //        zip_code = reader.IsDBNull(2) ? "" : reader.GetString(2),
    //        city = reader.IsDBNull(3) ? "" : reader.GetString(3),
    //        country = reader.IsDBNull(4) ? "" : reader.GetString(4)
    //      };
    //      ord.address = address;
    //    }
    //  }

    //}

    //public async void GetLanes(List<OrderGet> ordersList)
    //{
    //  IDbConnection db = new SqliteConnection(_configuration.GetConnectionString("default"));
    //  SQLitePCL.Batteries_V2.Init();

    //  foreach (var ord in ordersList)
    //  {
    //    string query = $"SELECT product_code,price_netto,quantity,vat,currency FROM orders_p WHERE order_id = {ord.response_order_id}";
    //    var reader = db.ExecuteReader(query);

    //    List<Lane> lanes = new List<Lane>();
    //    while (reader.Read())
    //    {
    //      var lane = new Lane()
    //      {
    //        product_code = reader.IsDBNull(0) ? "" : reader.GetString(0),
    //        price_netto = reader.IsDBNull(1) ? -1 : reader.GetDecimal(1),
    //        quantity = reader.IsDBNull(2) ? -1 : reader.GetDecimal(2),
    //        vat = reader.IsDBNull(3) ? -1 : reader.GetInt32(3),
    //        currency = reader.IsDBNull(4) ? "" : reader.GetString(4),
    //      };
    //      lanes.Add(lane);
    //    }

    //    ord.lanes = lanes;
    //  }

    //}


    public int? AddOrder(Order order)
    {
      //string order_date;
      //string shipment_date;
      //string status_id = order.status_id == null ? "null" : $"'{order.status_id}'";
      //string status_name = order.status_name == null ? "null" : $"'{order.status_name}'";
      //try
      //{
      //  order_date = DateTime.Parse(order.order_date).ToString("dd-MM-yyyy");
      //}
      //catch (Exception ex)
      //{
      //  //order_date = DateTime.MinValue.ToShortDateString();
      //  order_date = "error";
      //}
      //try
      //{
      //  shipment_date = order.shipment_date != null ? $"'{DateTime.Parse(order.shipment_date).ToString("dd-MM-yyyy")}'" : "null";
      //}
      //catch (Exception ex)
      //{
      //  //shipment_date = DateTime.MinValue.ToShortDateString();
      //  shipment_date = "error";
      //}

      int? response_order_id =null;

      string payment_type = order.payment_type == null ? "null" : $"'{order.payment_type}'";
      string comment = order.comment == null ? "null" : $"'{order.comment}'";
      string examleOrderId = order.example_order_id == null ? "null" : $"{order.example_order_id}";

      string address_name = "null";
      string address_street = "null";
      string address_zip_code = "null";
      string address_city = "null";
      string address_country = "null";

      if (order.address != null) {
        address_name = $"'{order.address.name}'";
        address_street = $"'{order.address.street}'";
        address_zip_code = $"'{order.address.zip_code}'";
        address_city = $"'{order.address.city}'";
        address_country = $"'{order.address.country}'";
      }

      string stm = $"INSERT INTO orders_h (example_order_id,client_id,value_total_netto,value_total_brutto,payment_type,document_type,delivery_type,currency,comment,address_name,address_street,address_zip_code,address_city,address_country,import_date,import_status) " +
        $"VALUES({examleOrderId},{order.client_id}," +
        $"{order.value_total_netto.ToString("0.00").Replace(",",".")},{order.value_total_brutto.ToString("0.00").Replace(",", ".")},{payment_type},'{order.document_type}',{order.delivery_type},'{order.currency}',{comment}" +
        $",{address_name},{address_street},{address_zip_code},{address_city},{address_country},datetime('now'),'WAITING');";

      using (IDbConnection dbConnection = new SqliteConnection(_configuration.GetConnectionString("default")))
      {
        if (dbConnection.State == ConnectionState.Closed)
          dbConnection.Open();
        using var transaction = dbConnection.BeginTransaction();
        try
        {
          var response = dbConnection.Execute(stm);

          stm = "SELECT MAX(response_order_id) FROM orders_h";

          var reader = dbConnection.ExecuteReader(stm);

          if (reader.Read())
          {
            response_order_id = reader.IsDBNull(0) ? null : reader.GetInt32(0);
          }

          if (response > 0)
          {
            foreach (var product in order.lanes)
            {
              stm = $"INSERT INTO orders_p VALUES({response_order_id},'{product.product_code}',{product.price_netto.ToString("0.00").Replace(",", ".")}," +
                $"{product.quantity},{product.vat.ToString()},'{product.currency}');";

              var productResponse = dbConnection.Query<Lane>(stm).FirstOrDefault();
            }

            transaction.Commit();
          }
        }
        catch (Exception ex)
        {
          transaction.Rollback();
        }

        return response_order_id;
      }
    }
  }
}
