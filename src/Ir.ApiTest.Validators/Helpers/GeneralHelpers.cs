using System.Text;

namespace Ir.ApiTest.Validators.Helpers;

public class GeneralHelpers
{
  public static string CalculateProductHash(Contracts.Product product)
  {
    // simple hash based on product properties
    string hashInput = $"{product.Id}|{product.Name}|{product.Size}|{product.Colour}|{product.Price}";
    using (var sha256 = System.Security.Cryptography.SHA256.Create())
    {
      byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(hashInput));
      return Convert.ToBase64String(hashBytes);
    }
  }
}
