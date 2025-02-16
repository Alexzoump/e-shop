using Data;
using Data.Models;
using System.Linq;

public static class DbInitializer
{
    public static void Seed(AppDbContext context)
    {
        if (!context.Products.Any())
        {
            var products = new[]
            {
                new Product {
                    Name = "Fake Product 1",
                    Description = "A nice product...",
                    Price = 9.99M,
                    Stock = 10,
                    ImageUrl = "https://placehold.co/200"
                },
                new Product {
                    Name = "Fake Product 2",
                    Description = "Another product...",
                    Price = 15.50M,
                    Stock = 5,
                    ImageUrl = "https://placehold.co/200"
                },
                new Product {
                    Name = "Fake Product 3",
                    Description = "More about product...",
                    Price = 22.00M,
                    Stock = 20,
                    ImageUrl = "https://placehold.co/200"
                },
                new Product {
                    Name = "Fake Product 4",
                    Description = "Lorem ipsum...",
                    Price = 15.00M,
                    Stock = 10,
                    ImageUrl = "https://placehold.co/200"
                }
            };

            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}