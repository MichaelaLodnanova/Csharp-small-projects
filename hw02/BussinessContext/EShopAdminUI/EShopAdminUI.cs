using HW02.AnalyticalDataContext;
using HW02.BussinessContext.DB.EShopAdminUI;

namespace HW02.BussinessContext.EShopAdminUI
{
    public class EShopAdminUI
    {
        private readonly ProductDBContext productDBContext;
        private readonly Parser parser;

        public EShopAdminUI(LoggerListener loggerListener,
                AnalyticalDataListener analyticalDataListener, Parser parserP,
                ProductDBContext productDB)
        {
            productDBContext = productDB;
            parser = parserP;
            parser.CommandParsedHandler += loggerListener.OnCommandProcessed;
            parser.CommandParsedHandler += analyticalDataListener.OnCommandProcessed;
        }

        public void Run()
        {
            while (true)
            {
                Console.WriteLine("Enter a command: ");
                string? command = Console.ReadLine();
                bool success = true;
                string message = string.Empty;
                if (command == null)
                {
                    Console.WriteLine("Not command entered");
                    continue;
                }
                else if (command.StartsWith("add-product"))
                {
                    (success, message) = parser.ParseAddProduct(command);
                }
                else if (command.StartsWith("delete-product"))
                {
                    (success, message) = parser.ParseDeleteProduct(command);
                }
                else if (command.StartsWith("list-products"))
                {
                    (success, message)  = parser.ListAllProducts(productDBContext.ReadProducts());
                }
                else if (command.StartsWith("add-category"))
                {
                    (success, message) = parser.ParseAddCategory(command);
                }
                else if (command.StartsWith("delete-category"))
                {
                    (success, message) = parser.ParseDeleteCategory(command);
                }
                else if (command.StartsWith("list-categories"))
                {
                    (success, message) = parser.ListAllCategories();
                }
                else if (command.StartsWith("get-products-by-category"))
                {
                    (success, message) = parser.ParseGetProductsByCategory(command);
                }
                else if (command.StartsWith("exit"))
                {
                    Console.WriteLine("See you next time!");
                    break;
                }
                else
                {
                    Console.WriteLine("Wrong input command");
                    continue;
                }
                Console.WriteLine(message);
            }
        }
    }
}
