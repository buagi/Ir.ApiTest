using System;
using System.Linq;
using global::Ir.ApiTest.Controllers;
using global::Ir.ApiTest.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ir.ApiTest.Tests.Base;

  public abstract class ProductsControllerTestsBase : IDisposable
  {
    protected readonly ProductsController _productsController;
    protected readonly Context _dbContext;

    protected ProductsControllerTestsBase()
    {
      var dbContextOptions = new DbContextOptionsBuilder<Context>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;
      _dbContext = new Context(dbContextOptions);
      _productsController = new ProductsController();
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        // Clear the database after each test run to prevent data leaks
        _dbContext.Products.RemoveRange(_dbContext.Products);
        _dbContext.SaveChanges();
        _dbContext.Dispose();
      }
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
  }
