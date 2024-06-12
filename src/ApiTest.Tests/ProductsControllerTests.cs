using Ir.ApiTest.Controllers;
using Ir.ApiTest.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using DbProduct = Ir.ApiTest.Entity.Models.Product;
using Product = Ir.ApiTest.Contracts.Product;

namespace Ir.ApiTest.Tests;

public class ProductsControlersTests
{
  private readonly ProductsController _productsController;
  private Context _dbContext;

  public ProductsControlersTests()
  {
    var dbContextOptions = new DbContextOptionsBuilder<Context>()
       .UseInMemoryDatabase(databaseName: "TestDatabase")
       .Options;

    _dbContext = new Context(dbContextOptions);

    _productsController = new ProductsController();
  }

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
    var products = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
    Assert.Equal(2, products.Count());
  }
}