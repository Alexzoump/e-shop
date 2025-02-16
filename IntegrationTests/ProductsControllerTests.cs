using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Data.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;
using System.IO;

public class ProductsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.UseContentRoot("/Users/giannisk/Documents/eshop-project/");
            })
            .CreateClient();
    }

    [Fact]
    public async Task GetAllProducts_ReturnsOkResult()
    {
        var response = await _client.GetAsync("/api/products");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var products = await response.Content.ReadFromJsonAsync<List<Product>>();
        Assert.NotNull(products);
    }

    [Fact]
    public async Task CreateProduct_ReturnsCreatedResponse()
    {
        var newProduct = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 19.99M,
            Stock = 10,
            ImageUrl = "https://placehold.co/400"
        };

        var response = await _client.PostAsJsonAsync("/api/products", newProduct);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdProduct = await response.Content.ReadFromJsonAsync<Product>();
        Assert.NotNull(createdProduct);
        Assert.Equal("Test Product", createdProduct.Name);
    }

    [Fact]
    public async Task GetProductById_ReturnsNotFoundForInvalidId()
    {
        var response = await _client.GetAsync("/api/products/9999"); 
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}