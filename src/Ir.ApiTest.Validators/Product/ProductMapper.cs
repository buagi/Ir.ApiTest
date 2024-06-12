using Entity = Ir.ApiTest.Entity;
using Contracts = Ir.ApiTest.Contracts;
using Product = Ir.ApiTest.Contracts.Product;
using dbProduct = Ir.ApiTest.Entity.Models.Product;

namespace Ir.ApiTest.Validators.Product;

public static class ProductMapper
{
  public static Entity.Models.Product ToEntityProduct(this dbProduct contractProduct)
  {
    return new Entity.Models.Product
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

  public static Contracts.Product ToContractProduct(this Contracts.Product entityProduct)
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