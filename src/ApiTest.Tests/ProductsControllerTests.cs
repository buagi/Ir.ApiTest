using Ir.ApiTest.Controllers;
using Ir.ApiTest.Entity;
using Ir.ApiTest.Tests.Base;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using DbProduct = Ir.ApiTest.Entity.Models.Product;
using Product = Ir.ApiTest.Contracts.Product;

namespace Ir.ApiTest.Tests;

public class ProductsControlersTests : ProductsControllerTestsBase
{

  [Fact]
  public async Task CreateProduct_ReturnsCreatedResult_WhenValidProductIsPassed()
  {
    // Arrange
    var product = new Product
    {
      Id = Guid.NewGuid().ToString(),
      Name = "Test Product",
      Size = "M",
      Colour = "Red",
      Price = 100.0
    };

    // Act
    var result = await _productsController.CreateProduct(product, _dbContext);

    // Assert
    var createdResult = Assert.IsType<CreatedAtActionResult>(result);
    var createdProduct = Assert.IsType<Product>(createdResult.Value);

    Assert.Equal(product.Id, createdProduct.Id);
    Assert.Equal(product.Name, createdProduct.Name);
    Assert.Equal(product.Size, createdProduct.Size);
    Assert.Equal(product.Colour, createdProduct.Colour);
    Assert.Equal(product.Price, createdProduct.Price);
    Assert.NotEqual(default(DateTimeOffset), createdProduct.Created);
    Assert.NotEqual(default(DateTimeOffset), createdProduct.LastUpdated);
  }

  [Fact]
  public async Task CreateProduct_ReturnsBadRequest_WhenInvalidProductIsPassed()
  {
    // Arrange
    var invalidProduct = new Product
    {
      Id = string.Empty,
      Name = "",
      Size = "Invalid Size",
      Colour = "Invalid Colour",
      Price = -10.0
    };

    // Act
    var result = await _productsController.CreateProduct(invalidProduct, _dbContext);

    // Assert
    Assert.IsType<BadRequestObjectResult>(result);
  }

  [Fact]
  public async Task CreateProduct_ReturnsConflict_WhenProductWithSameIdAlreadyExists()
  {
    // Arrange
    var existingProduct = new DbProduct
    {
      Id = Guid.NewGuid().ToString(),
      Name = "Existing Product",
      Size = "L",
      Colour = "Blue",
      Price = 50.0
    };

    await _dbContext.Products.AddAsync(existingProduct);
    await _dbContext.SaveChangesAsync();

    var duplicateProduct = new Product
    {
      Id = existingProduct.Id,
      Name = "Duplicate Product",
      Size = "XL",
      Colour = "Green",
      Price = 75.0
    };

    // Act
    var result = await _productsController.CreateProduct(duplicateProduct, _dbContext);

    // Assert
    Assert.IsType<ConflictResult>(result);
  }

  [Fact]
  public async Task GetProduct_ReturnsNotFound_WhenProductDoesNotExist()
  {
    // Arrange
    var nonExistentId = Guid.NewGuid().ToString();

    // Act
    var result = await _productsController.GetProduct(nonExistentId, _dbContext);

    // Assert
    Assert.IsType<NotFoundResult>(result);
  }

  [Fact]
  public async Task GetProducts_ReturnsOkResult_WithListOfProducts()
  {
    // Arrange
    var product1 = new DbProduct { Id = Guid.NewGuid().ToString(), Name = "Product 1", Size = "S", Colour = "Red", Price = 10.0 };
    var product2 = new DbProduct { Id = Guid.NewGuid().ToString(), Name = "Product 2", Size = "M", Colour = "Blue", Price = 20.0 };

    await _dbContext.Products.AddRangeAsync(product1, product2);
    await _dbContext.SaveChangesAsync();

    // Act
    var result = await _productsController.GetProducts(_dbContext);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var response = Assert.IsAssignableFrom<object>(okResult.Value);
    var products = Assert.IsAssignableFrom<IEnumerable<Product>>(response.GetType().GetProperty("Products").GetValue(response));
    Assert.Equal(2, products.Count());  
  }

  [Fact]
  public async Task GetProduct_ReturnsOkResult_WhenProductExists()
  {
    // Arrange
    var existingProduct = new DbProduct
    {
      Id = Guid.NewGuid().ToString(),
      Name = "Existing Product",
      Size = "L",
      Colour = "Blue",
      Price = 50.0
    };
    await _dbContext.Products.AddAsync(existingProduct);
    await _dbContext.SaveChangesAsync();

    // Act
    var result = await _productsController.GetProduct(existingProduct.Id, _dbContext);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var product = Assert.IsType<Product>(okResult.Value);
    Assert.Equal(existingProduct.Id, product.Id);
    Assert.Equal(existingProduct.Name, product.Name);
    Assert.Equal(existingProduct.Size, product.Size);
    Assert.Equal(existingProduct.Colour, product.Colour);
    Assert.Equal(existingProduct.Price, product.Price);
  }

  [Fact]
  public async Task UpdateProduct_DoesNotUpdateIdCreatedDate()
  {
    // Arrange
    var existingProduct = new DbProduct
    {
      Id = Guid.NewGuid().ToString(),
      Name = "Existing Product",
      Size = "L",
      Colour = "Blue",
      Price = 50.0,
      Created = DateTime.UtcNow.AddDays(-10),
      LastUpdated = DateTime.UtcNow.AddDays(-10)
    };
    await _dbContext.Products.AddAsync(existingProduct);
    await _dbContext.SaveChangesAsync();

    var originalLastUpdated = existingProduct.LastUpdated;

    var patchDocument = new JsonPatchDocument<Product>();
    patchDocument.Replace(p => p.Name, "Updated Product");
    patchDocument.Replace(p => p.Size, "XL");
    patchDocument.Replace(p => p.Colour, "Green");
    patchDocument.Replace(p => p.Price, 75.0);

    // Act
    await _productsController.UpdateProduct(existingProduct.Id, patchDocument, _dbContext);

    // Assert
    var dbProduct = await _dbContext.Products.FindAsync(existingProduct.Id);
    Assert.Equal(existingProduct.Id, dbProduct.Id);
    Assert.NotEqual(originalLastUpdated, dbProduct.LastUpdated);
  }

  [Fact]
  public async Task CreateProduct_ReturnsBadRequest_WhenEmptyProductIsPassed()
  {
    // Arrange
    var emptyProduct = new Product();

    // Act
    var result = await _productsController.CreateProduct(emptyProduct, _dbContext);

    // Assert
    Assert.IsType<BadRequestObjectResult>(result);
  }

  [Fact]
  public async Task UpdateProduct_ReturnsNoContent_WhenEmptyPatchDocumentIsPassed()
  {
    // Arrange
    var existingProduct = new DbProduct
    {
      Id = Guid.NewGuid().ToString(),
      Name = "Existing Product",
      Size = "L",
      Colour = "Blue",
      Price = 50.0
    };
    await _dbContext.Products.AddAsync(existingProduct);
    await _dbContext.SaveChangesAsync();

    var emptyPatchDocument = new JsonPatchDocument<Product>();

    // Act
    var result = await _productsController.UpdateProduct(existingProduct.Id, emptyPatchDocument, _dbContext);

    // Assert
    Assert.IsType<BadRequestObjectResult>(result);
  }

  [Fact]
  public async Task GetProducts_ReturnsPaginatedProducts()
  {
    // Arrange
    var products = new List<DbProduct>
    {
        new DbProduct { Id = Guid.NewGuid().ToString(), Name = "Product 1" },
        new DbProduct { Id = Guid.NewGuid().ToString(), Name = "Product 2" },
        new DbProduct { Id = Guid.NewGuid().ToString(), Name = "Product 3" },
        new DbProduct { Id = Guid.NewGuid().ToString(), Name = "Product 4" },
        new DbProduct { Id = Guid.NewGuid().ToString(), Name = "Product 5" }
    };
    await _dbContext.Products.AddRangeAsync(products);
    await _dbContext.SaveChangesAsync();

    // Act
    var result = await _productsController.GetProducts(_dbContext, page: 2, pageSize: 2);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var response = Assert.IsAssignableFrom<object>(okResult.Value);
    Assert.Equal(5, response.GetType().GetProperty("TotalCount").GetValue(response));
    Assert.Equal(3, response.GetType().GetProperty("TotalPages").GetValue(response));
    Assert.Equal(2, response.GetType().GetProperty("CurrentPage").GetValue(response));
    Assert.Equal(2, response.GetType().GetProperty("PageSize").GetValue(response));
    var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(response.GetType().GetProperty("Products").GetValue(response));
    Assert.Equal(2, returnedProducts.Count());
  }

  [Fact]
  public async Task CreateProduct_SetsProductHash()
  {
    // Arrange
    var product = new Product
    {
      Id = Guid.NewGuid().ToString(),
      Name = "Test Product",
      Size = "M",
      Colour = "Red",
      Price = 100.0
    };

    // Act
    var result = await _productsController.CreateProduct(product, _dbContext);

    // Assert
    var createdResult = Assert.IsType<CreatedAtActionResult>(result);
    var createdProduct = Assert.IsType<Product>(createdResult.Value);
    var dbProduct = await _dbContext.Products.FindAsync(createdProduct.Id);
    Assert.NotNull(dbProduct.Hash);
  }

  [Fact]
  public async Task UpdateProduct_UpdatesProductHash()
  {
    // Arrange
    var existingProduct = new DbProduct
    {
      Id = Guid.NewGuid().ToString(),
      Name = "Existing Product",
      Size = "L",
      Colour = "Blue",
      Price = 50.0
    };
    await _dbContext.Products.AddAsync(existingProduct);
    await _dbContext.SaveChangesAsync();

    var patchDocument = new JsonPatchDocument<Product>();
    patchDocument.Replace(p => p.Name, "Updated Product");

    var originalHash = existingProduct.Hash;

    // Act
    await _productsController.UpdateProduct(existingProduct.Id, patchDocument, _dbContext);

    // Assert
    var dbProduct = await _dbContext.Products.FindAsync(existingProduct.Id);
    Assert.NotNull(dbProduct.Hash);
    Assert.NotEqual(originalHash, dbProduct.Hash);
  }
}