using Ir.FakeMarketplace.Controllers;
using Ir.IntegrationTest.Contracts;
using Ir.IntegrationTest.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ApiTest.Tests;

public class ProductsControlersTests
{

  private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();
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
}