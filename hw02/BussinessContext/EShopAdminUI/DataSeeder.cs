using HW02.AnalyticalDataContext;
using HW02.BussinessContext.DB.Entities;
using HW02.BussinessContext.DB.EShopAdminUI;
using HW02.BussinessContext.Managers;

namespace HW02.BussinessContext.EShopAdminUI
{
    public class DataSeeder
    {
        private readonly CategoryManager categoryManager;
        private readonly ProductManager productManager;
        private readonly Parser parser;

        public DataSeeder(CategoryManager categoryM, ProductManager productM, Parser parserP)
        {
            categoryManager = categoryM;
            productManager = productM;
            parser = parserP;
        }

        public void Seed()
        {
            var categories = new List<Category>
            {
                new Category { Name = "Electrinics" },
                new Category { Name = "Books" },
                new Category { Name = "Clothing" }
            };
            foreach (var category in categories)
            {
                categoryManager.Create(category);
                parser.CommandParsed(CommandType.Add, EntityType.Category, category, true);
            }

            List<Product> products = new List<Product>
            {
            new Product { Name = "iPhone", Price = 1000, CategoryId = 1 },
            new Product { Name = "Samsung Galaxy", Price = 1000, CategoryId = 1 },
            new Product { Name = "The Great Gatsby", Price = 15, CategoryId = 2 },
            new Product { Name = "To Kill a Mockingbird", Price = 13, CategoryId = 2 },
            new Product { Name = "T-shirt", Price = 25, CategoryId = 3 },
            new Product { Name = "Jeans", Price = 50, CategoryId = 3 }
            };
            foreach(var product in products)
            {
                productManager.Create(product);
                parser.CommandParsed(CommandType.Add, EntityType.Product, product, true);
            }
        }
    }
}
