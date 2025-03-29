using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using OrderManagement.API.Controllers;
using OrderManagement.Application.Services;
using OrderManagement.Domain.Entities;
using OrderManagement.Infrastructure.Data;

public class ProductControllerTests
{
    private readonly ProductService _productService;
    private readonly ProductController _productController;
    private readonly OrderManagementDbContext _context;

    public ProductControllerTests()
    {
        var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
            .UseInMemoryDatabase(databaseName: "TestProductsDb")
            .Options;
        _context = new OrderManagementDbContext(options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        _productService = new ProductService(_context);
        _productController = new ProductController(_productService);
    }

    [Fact]
    public void GetProducts_ReturnsOkResult_WithProductsList()
    {
        _context.Products.AddRange(new List<Product>
        {
            new Product { Id = 1, Nombre = "Producto A", Precio = 50 },
            new Product { Id = 2, Nombre = "Producto B", Precio = 100 }
        });
        _context.SaveChanges();

        var actionResult = _productController.GetProducts();
        var result = actionResult as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        var productsList = result.Value as IEnumerable<Product>;
        Assert.NotNull(productsList);
        Assert.Equal(2, productsList.Count());
    }

    [Fact]
    public void CreateProduct_ValidProduct_ReturnsCreatedResult()
    {
        var product = new Product { Id = 1, Nombre = "Producto A", Precio = 50 };

        var actionResult = _productController.CreateProduct(product);
        var result = actionResult as CreatedAtActionResult;

        Assert.NotNull(result);
        Assert.Equal(201, result.StatusCode);
        Assert.Equal("GetProductById", result.ActionName);

        var createdProduct = _context.Products.Find(1);
        Assert.NotNull(createdProduct);
        Assert.Equal("Producto A", createdProduct.Nombre);
    }
}
