using Ir.IntegrationTest.Contracts;
using Ir.IntegrationTest.Entity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ir.FakeMarketplace.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{

  [HttpGet]
  public IEnumerable<Product> GetProducts()
  {
    throw new NotImplementedException();
  }

  [HttpGet("{id}")]
  public IActionResult GetProduct([FromRoute]string id)
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
  public IActionResult UpdateProduct([FromBody] JsonPatchDocument<Product> productPatchDocument)
  {
    throw new NotImplementedException();
  }
}