using System;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

namespace Ir.ApiTest.Validators.Product;

public class ProductValidationHelper
{
  public static bool ValidateProduct(Contracts.Product product)
  {
    if (product == null)
    {
      return false;
    }

    if (string.IsNullOrWhiteSpace(product.Id))
    {
      return false;
    }

    if (string.IsNullOrWhiteSpace(product.Name))
    {
      return false;
    }

    if (string.IsNullOrWhiteSpace(product.Size))
    {
      return false;
    }

    if (string.IsNullOrWhiteSpace(product.Colour))
    {
      return false;
    }

    if (product.Price <= 0)
    {
      return false;
    }

    // Strip any HTML tags or special characters from the product name
    product.Name = StripHtmlTags(product.Name);
    product.Name = RemoveSpecialCharacters(product.Name);

    // Ensure the product size is a valid value
    if (!IsValidSize(product.Size))
    {
      return false;
    }

    // Ensure the product colour is a valid value
    if (!IsValidColour(product.Colour))
    {
      return false;
    }

    // more validation rules as needed

    return true;
  }

  private static string StripHtmlTags(string input)
  {
    return Regex.Replace(input, "<.*?>", string.Empty);
  }

  private static string RemoveSpecialCharacters(string input)
  {
    return Regex.Replace(input, "[^a-zA-Z0-9 ]", string.Empty);
  }

  private static bool IsValidSize(string size)
  {
    string[] validSizes = { "S", "M", "L", "XL" };

    return Array.Exists(validSizes, s => s.Equals(size, StringComparison.OrdinalIgnoreCase));
  }

  private static bool IsValidColour(string colour)
  {
    string[] validColours = { "Red", "Green", "Blue", "Black", "White" };
    return Array.Exists(validColours, c => c.Equals(colour, StringComparison.OrdinalIgnoreCase));
  }
}