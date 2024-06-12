using Entity = Ir.ApiTest.Entity;
using Contracts = Ir.ApiTest.Contracts;
using DbProduct = Ir.ApiTest.Entity.Models.Product;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Ir.ApiTest.Validators.Helpers;

public static class ProductMapper
{
  public static DbProduct ToEntityProduct(this Ir.ApiTest.Contracts.Product contractProduct)
  {
    return new DbProduct
    {
      Id = contractProduct.Id,
      Name = contractProduct.Name,
      Size = contractProduct.Size,
      Colour = contractProduct.Colour,
      Price = contractProduct.Price,
      LastUpdated = contractProduct.LastUpdated,
      Created = contractProduct.Created,
      Hash = contractProduct.Hash
    };
  }

  public static Contracts.Product ToContractProduct(this DbProduct entityProduct)
  {
    return new Contracts.Product
    {
      Id = entityProduct.Id,
      Name = entityProduct.Name,
      Size = entityProduct.Size,
      Colour = entityProduct.Colour,
      Price = entityProduct.Price,
      LastUpdated = entityProduct.LastUpdated,
      Created = entityProduct.Created,
      Hash = entityProduct.Hash
    };
  }
}