using Ir.ApiTest.Contracts;
using Ir.ApiTest.Entity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Ir.ApiTest.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ProductsController : ControllerBase
  {

    [HttpGet]
    public async Task<IEnumerable<Product>> GetProducts(Context dbContext)
    {
      throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct([FromRoute] string id, Context dbContext)
    {
      throw new NotImplementedException();
    }

    [HttpPost()]
    public async Task<IActionResult> CreateProduct([FromBody] Product product, Context dbContext)
    {
      if (await dbContext.Products.AnyAsync(p => p.Id == product.Id))
      {
        return Conflict();
      }

      product.Created = DateTimeOffset.UtcNow;
      product.LastUpdated = DateTimeOffset.UtcNow;

      //ToDo: Add separate validators namespace / project

      //dbContext.Products.Add(product);
      await dbContext.SaveChangesAsync();

      return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateProduct([FromBody] JsonPatchDocument<Product> productPatchDocument, Context dbContext)
    {
      throw new NotImplementedException();
    }
  }
}