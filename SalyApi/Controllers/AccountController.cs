using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ExampleApi.Models;
using ExampleApi.Repository;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;


namespace ExampleApi.Controllers
{
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class AccountController : ControllerBase
  {

    private readonly IJWTAuthManager _authentication;

    public AccountController(IJWTAuthManager authentication)
    {
      _authentication = authentication;
    }


    /// <summary>
    /// Returns token.
    /// </summary>
    /// <response code="200">Success. Returns user's token.</response>
    [ProducesResponseType(typeof(LoginExample), 200)]
    [Microsoft.AspNetCore.Mvc.HttpPost("Login")]
    [AllowAnonymous]
    public IActionResult login([FromBody] LoginModel user)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest("Parameter is missing");
      }

      var result = _authentication.LoginUser(user);
      if (result.code == 200)
      {
        var token = _authentication.GenerateJWT(result.Data);
        return Ok(token);
      }
      return NotFound(result);
    }

    [HttpGet("UserList")]
    [Authorize(Roles = "Admin")]
    public IActionResult getAllUsers()
    {

      var result = _authentication.getUserList<User>();
      return Ok(result);
    }

    [HttpPost("Register")]
    [Authorize(Roles = "Admin")]
    public IActionResult Register([Microsoft.AspNetCore.Mvc.FromBody] User user)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest("Parameter is missing");
      }

      var result = _authentication.RegisterUser(user);
      if (result.code == 200)
      {
        return Ok(result);
      }
      return BadRequest(result);
    }

    [HttpDelete("Delete")]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(string id)
    {
      if (id == string.Empty)
      {
        return BadRequest("Parameter is missing");
      }

      var result = _authentication.DeleteUser(id);
      if (result.code == 200)
      {
        return Ok(result);
      }
      return NotFound(result);
    }
  }
}
