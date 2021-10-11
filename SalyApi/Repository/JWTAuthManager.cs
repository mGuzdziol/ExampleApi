using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ExampleApi.Models;
using ExampleApi.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ExampleApi.Repository
{
  public class JWTAuthManager : IJWTAuthManager
  {
    public IConfiguration _configuration;

    public JWTAuthManager(IConfiguration configuration)
    {
      _configuration = configuration;
    }
    public Response<string> GenerateJWT(User user)
    {
      Response<string> response = new Response<string>();

      var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtAuth:Key"]));
      var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

      //claim is used to add identity to JWT token
      var claims = new[] {
                 new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                 new Claim(JwtRegisteredClaimNames.Email, user.Email),
                 new Claim("roles", user.Role),
                 new Claim("Date", DateTime.Now.ToString()),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
             };

      var token = new JwtSecurityToken(_configuration["JwtAuth:Issuer"],
        _configuration["JwtAuth:Issuer"],
        claims,    //null original value
        expires: DateTime.Now.AddMinutes(1440),
        signingCredentials: credentials);

      response.Data = new JwtSecurityTokenHandler().WriteToken(token); //return access token
      response.code = 200;
      response.message = "Token generated";
      return response;
    }

    public Response<User> LoginUser(LoginModel user)
    {
      //int responseCode;
      //string stm = $"SELECT * FROM tbl_users where email = {user.Email} and password = {MD5.HashString(user.Password)}";
      //string cs = configuration.GetConnectionString("default");

      //using var con = new SqliteConnection(cs);
      //SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_dynamic_cdecl());
      //con.Open();

      //using var cmd = new SqliteCommand(stm, con);
      //var reader = cmd.ExecuteReader();

      //if (reader.HasRows)
      //{
      //  responseCode = 200;
      //}
      //else
      //{
      //  responseCode = 500;
      //}




      string stm = $"SELECT * FROM tbl_users where email = '{user.email}' and password = '{MD5.HashString(user.password)}'";
      Response<User> response = new Response<User>();
      using (IDbConnection dbConnection = new SqliteConnection(_configuration.GetConnectionString("default")))
      {
        if (dbConnection.State == ConnectionState.Closed)
          dbConnection.Open();

        using var transaction = dbConnection.BeginTransaction();
        try
        {
          response.Data = dbConnection.Query<User>(stm).FirstOrDefault();

          if(response.Data == null)
          {
            response.code = 404;
            response.message = "User not found";
          }
          else
          {
            response.code = 200;
            response.message = "Success";

          }
          
          transaction.Commit();
        }
        catch (Exception ex)
        {
          transaction.Rollback();
          response.code = 500;
          response.message = ex.Message;
        }
      }
      return response;

    }

    public Response<User> RegisterUser(User user)
    {
      string stm = $"INSERT INTO tbl_users VALUES(null,'{user.Username}','{MD5.HashString(user.Password)}','{user.Email}','{user.Role}',DATE('now'));";
      //string cs = configuration.GetConnectionString("default");

      //using var con = new SqliteConnection(cs);
      //SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_dynamic_cdecl());
      //con.Open();

      //using var cmd = new SqliteCommand(stm, con);
      //var rowsAffected = cmd.ExecuteNonQuery();

      //if (rowsAffected>0)
      //{
      //  return 200;
      //}
      //else
      //{
      //  return 500;
      //}




      Response<User> response = new Response<User>();
      using (IDbConnection dbConnection = new SqliteConnection(_configuration.GetConnectionString("default")))
      {
        if (dbConnection.State == ConnectionState.Closed)
          dbConnection.Open();
        using var transaction = dbConnection.BeginTransaction();
        try
        {
          response.Data = dbConnection.Query<User>(stm).FirstOrDefault();
          response.code = 200;
          response.message = "Success";
          transaction.Commit();
        }
        catch (Exception ex)
        {
          transaction.Rollback();
          response.code = 500;
          response.message = ex.Message;
        }
      }
      return response;
    }

    //DELETE FROM tbl_users WHERE userid = 1;

    public Response<User> DeleteUser(string userId)
    {
      string stm = $"DELETE FROM tbl_users WHERE userid = {userId};";
      //string cs = configuration.GetConnectionString("default");

      //using var con = new SqliteConnection(cs);
      //SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_dynamic_cdecl());
      //con.Open();

      //using var cmd = new SqliteCommand(stm, con);
      //var rowsAffected = cmd.ExecuteNonQuery();

      //if (rowsAffected > 0)
      //{
      //  return 200;
      //}
      //else
      //{
      //  return 500;
      //}



      Response<User> response = new Response<User>();
      using (IDbConnection dbConnection = new SqliteConnection(_configuration.GetConnectionString("default")))
      {
        if (dbConnection.State == ConnectionState.Closed)
          dbConnection.Open();
        using var transaction = dbConnection.BeginTransaction();
        try
        {
          response.Data = dbConnection.Query<User>(stm).FirstOrDefault();
          response.code = 200;
          response.message = "Success";
          transaction.Commit();
        }
        catch (Exception ex)
        {
          transaction.Rollback();
          response.code = 500;
          response.message = ex.Message;
        }
      }
      return response;

    }


    public Response<List<User>> getUserList<User>()
    {
      Response<List<User>> response = new Response<List<User>>();
      using IDbConnection db = new SqliteConnection(_configuration.GetConnectionString("default"));
      string query = "Select userid,username,email,role,reg_date FROM tbl_users";
      response.Data = db.Query<User>(query, null, commandType: CommandType.Text).ToList();
      return response;
    }
  }
}
