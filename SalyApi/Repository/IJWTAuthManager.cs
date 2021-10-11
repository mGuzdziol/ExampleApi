using Dapper;
using ExampleApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleApi.Repository
{
  public interface IJWTAuthManager
  {
    Response<string> GenerateJWT(User user);
    Response<User> LoginUser(LoginModel user);
    Response<User> RegisterUser(User user);
    Response<User> DeleteUser(string userId);
    Response<List<User>> getUserList<User>();
  }
}
