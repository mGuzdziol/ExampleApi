using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ExampleApi.Service
{
  public static class MD5
  {
    public static string HashString(string text)
    {
      // Byte array representation of source string
      var sourceBytes = Encoding.UTF8.GetBytes(text);

      // Generate hash value(Byte Array) for input data
      var hashBytes = System.Security.Cryptography.MD5.Create().ComputeHash(sourceBytes);

      // Convert hash byte array to string
      var hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);

      return hash;
    }
  }
}
