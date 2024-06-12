using Ir.ApiTest.Entity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Product = Ir.ApiTest.Contracts.Product;

using static Ir.ApiTest.Validators.Product.ProductValidationHelper;
using static Ir.ApiTest.Validators.Helpers.ProductMapper;
using static Ir.ApiTest.Validators.Helpers.GeneralHelpers;
using Ir.ApiTest.Validators.Helpers;

namespace Ir.ApiTest.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ProductsController : ControllerBase
  {

    [HttpGet]
    public async Task<IActionResult> GetProducts(Context dbContext, DateTime? lastUpdated = null, int page = 1, int pageSize = 10)
    {
      IQueryable<Entity.Models.Product> query = dbContext.Products.AsQueryable();

      if (lastUpdated.HasValue)
      {
        query = query.Where(p => p.LastUpdated >= lastUpdated.Value);
      }

      var products = await dbContext.Products
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .Select(p => p.ToContractProduct())
          .ToListAsync();


      var totalCount = await query.CountAsync();
      var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

      var response = new 
      {
        TotalCount = totalCount,
        TotalPages = totalPages,
        CurrentPage = page,
        PageSize = pageSize,
        Products = products
      };

      // could return different if no products
      return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct([FromRoute] string id, Context dbContext)
    {
      var product = await dbContext.Products.FindAsync(id);
      if (product == null)
      {
        return NotFound();
      }

      return Ok(product.ToContractProduct());
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

      // This is obviously oversimplified
      if (ValidateProduct(product))
      {
        product.Hash = CalculateProductHash(product);

        await dbContext.Products.AddAsync(product.ToEntityProduct());
        await dbContext.SaveChangesAsync();

        // in some cases we would want to html encode data to prevent XSS.
        await dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
      }
      else
      {
        // Generic error to prevent exploratory insertion of malicious data
        return BadRequest("There was an error.");
      }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateProduct([FromRoute] string id, [FromBody] JsonPatchDocument<Product> productPatchDocument, Context dbContext)
    {
      var dbProduct = await dbContext.Products.FindAsync(id);
      var updateHash = false;

      if (dbProduct == null)
      {
        return NotFound();
      }

      if (productPatchDocument == null || productPatchDocument.Operations == null || productPatchDocument.Operations.Count == 0)
      {
        return BadRequest("Empty or invalid patch document.");
      }
      else
      {
        updateHash = true;
      }

      var product = ProductMapper.ToContractProduct(dbProduct);
      productPatchDocument.ApplyTo(product);
      dbProduct.LastUpdated = DateTime.UtcNow;

      if(updateHash)
      {
        dbProduct.Hash = CalculateProductHash(product);
      }

      // again, validate and XSS check if applicable
      if (!ValidateProduct(product))
      {
        return BadRequest("There was an error.");
      }

      await dbContext.SaveChangesAsync();

      return Ok();
    }
  }
}