using HW02.BussinessContext.DB.Entities;
using HW02.BussinessContext.EShopAdminUI;
using HW02.BussinessContext.FileDatabase;
using HW02.BussinessContext.Managers;
using System.Text;

namespace HW02.BussinessContext.DB.EShopAdminUI
{
    public delegate void OnCommandParsedHandler(Object sender, CommandParsedEventArgs e);

    public enum CommandType
    {
        Add,
        Delete,
        Get
    }

    public enum EntityType
    {
        Category,
        Product
    }

    public class Parser
    {
        private readonly ProductManager productManager;
        private readonly CategoryManager categoryManager;
        private readonly ProductDBContext productDBContext;
        private readonly CategoryDBContext categoryDBContext;
        private readonly OutputFormatter formatter;
        public event OnCommandParsedHandler CommandParsedHandler;

        public Parser(ProductManager productM,
                      CategoryManager categoryM,
                      ProductDBContext productDB,
                      CategoryDBContext categoryDB,
                      OutputFormatter formatter) 
        {
            productManager = productM;
            categoryManager = categoryM;
            productDBContext = productDB;
            categoryDBContext = categoryDB;
            this.formatter = formatter;
        }

        public void CommandParsed(CommandType commandType, EntityType entityType, Entity entity, bool success)
        {
            CommandParsedHandler?.Invoke(this, new CommandParsedEventArgs() { 
                entity = entity,
                entityType = entityType, 
                logType = commandType,
                success = success   
            });
        }

        private void CommandParsed(CommandType commandType, EntityType entityType, bool success, string? message)
        {
            CommandParsedHandler?.Invoke(this, new CommandParsedEventArgs()
            {
                entityType = entityType,
                logType = commandType,
                success = success,
                errorMessage = message
            });
        }

        private Tuple<bool, string> ParseCommand(CommandType commandType, EntityType entityType, string[] commands, ref int id)
        {
            
            if (commandType.Equals(CommandType.Add) && entityType.Equals(EntityType.Product))
            {
                if (commands.Length != 4)
                    return Tuple.Create(false, "Invalid number of arguments");
                if (!int.TryParse(commands[2], out id))
                    return Tuple.Create(false, "Invalid value of entity id");
            }
            else
            {
                if (commands.Length != 2)
                    return Tuple.Create(false, "Invalid number of arguments");
                if (!(commandType.Equals(CommandType.Add) && entityType.Equals(EntityType.Category)))
                {
                    if (!int.TryParse(commands[1], out id))
                        return Tuple.Create(false, "Invalid value of entity id");
                } 
            }
            return Tuple.Create(true, "");
        }
        public Tuple<bool, string> ParseAddProduct(string command)
        {
            string[] commands = command.Trim().Split(' ');
            int categoryId = 0;
            (bool success, string message) = ParseCommand(CommandType.Add, EntityType.Product, commands, ref categoryId);
            if (!success)
            {
                CommandParsed(CommandType.Add, EntityType.Product, false, message);
                return Tuple.Create(success, message);
            }

            string name = commands[1];
            Category? category = categoryManager.Retrieve(categoryId);
            if (category == null)
            {
                message = $"Category with id: {categoryId} does not exist.";
                CommandParsed(CommandType.Add, EntityType.Product, false, message);
                return Tuple.Create(false, message);
            }
            int price;
            if (!int.TryParse(commands[3], out price))
            {
                message = "Invalid price";
                CommandParsed(CommandType.Add, EntityType.Product, false, message);
                return Tuple.Create(false, message);
            }


            Product product = new Product
            {
                CategoryId = categoryId,
                Name = name,
                Price = price
            };
            productManager.Create(product);
            CommandParsed(CommandType.Add, EntityType.Product, product, true);
            return Tuple.Create(true, $"Product with id: {product.Id} successfully added.");
        }


        public Tuple<bool, string> ParseDeleteProduct(string command)
        {
            string[] commands = command.Trim().Split(" ");
            int productId = 0;
            (bool success, string message) = ParseCommand(CommandType.Delete, EntityType.Product, commands, ref productId);
            if (!success)
            {
                CommandParsed(CommandType.Delete, EntityType.Product, false, message);
                return Tuple.Create(success, message);
            }
            
            Product? product = productManager.Retrieve(productId);
            if (product == null)
            {
                message = $"Product with id: {productId} does not exist";
                CommandParsed(CommandType.Delete, EntityType.Product, false, message);
                return Tuple.Create(false, message);
            }
            CommandParsed(CommandType.Delete, EntityType.Product, product, true);
            productManager.Delete(productId); ;     
            return Tuple.Create(true, $"Product with id: {productId} deleted successfully");
        }

        public Tuple<bool, string> ParseAddCategory(string command)
        {
            string[] commands = command.Trim().Split(" ");
            int categoryId = 0;
            (bool success, string message) = ParseCommand(CommandType.Add, EntityType.Category, commands, ref categoryId);
            if (!success)
            {
                CommandParsed(CommandType.Add, EntityType.Category, false, message);
                return Tuple.Create(success, message);
            }

            string name = commands[1];
            Category category = new Category
            {
                Name = name
            };
            categoryManager.Create(category);
            CommandParsed(CommandType.Add, EntityType.Category, category, true);
            return Tuple.Create(true, "Category added successfully");
        }

        public Tuple<bool, string> ParseDeleteCategory(string command)
        {
            string[] commands = command.Trim().Split(" ");
            int categoryId = 0;
            (bool success, string message) = ParseCommand(CommandType.Delete, EntityType.Category, commands, ref categoryId);
            if (!success)
            {
                CommandParsed(CommandType.Delete, EntityType.Category, false, message);
                return Tuple.Create(success, message);
            }
            Category? category = categoryManager.Retrieve(categoryId);
            if (category == null)
            {
                message = $"Category with id: {categoryId} not found";
                CommandParsed(CommandType.Delete, EntityType.Category, false, message);
                return Tuple.Create(false, message);
            }
                
            categoryManager.Delete(categoryId);
            CommandParsed(CommandType.Delete, EntityType.Category, category, true);
            return Tuple.Create(true, "Category deleted successfully");
        }

        public Tuple<bool, string> ParseGetProductsByCategory(string command)
        {
            string[] commands = command.Trim().Split(" ");
            int categoryId = 0;
            string message = string.Empty;
            if (!int.TryParse(commands[1], out categoryId))
            {
                message = "Wrong value for category id";
                CommandParsed(CommandType.Get, EntityType.Category, false, message);
                return Tuple.Create(false, message);
            }

            Category? category = categoryManager.Retrieve(categoryId);
            if (category == null)
            {
                message = $"Category with id: {categoryId} not found";
                CommandParsed(CommandType.Get, EntityType.Category, false, message);
                return Tuple.Create(false, message);
            }
            
            CommandParsed(CommandType.Get, EntityType.Category, category, true);
            return ListAllProducts(productDBContext.ReadProducts().FindAll(p => p.CategoryId == category.Id));
        }
        public Tuple<bool, string> ListAllCategories()
        {
            string output = formatter.GetFormattedCategoriesTable();
            CommandParsed(CommandType.Get, EntityType.Category, true, null);
            return Tuple.Create(true, output);
        }

        public Tuple<bool, string> ListAllProducts(List<Product> products)
        {
            string output = formatter.GetFormattedTable(products);
            CommandParsed(CommandType.Get, EntityType.Product, true, null);
            return Tuple.Create(true, output);
        }
    }
}
