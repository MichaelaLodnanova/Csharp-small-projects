using HW02.AnalyticalDataContext;
using HW02.AnalyticalDataContext.DB;
using HW02.BussinessContext;
using HW02.BussinessContext.DB.EShopAdminUI;
using HW02.BussinessContext.EShopAdminUI;
using HW02.BussinessContext.FileDatabase;
using HW02.BussinessContext.Managers;
using HW02.LoggerContext.DB;

namespace HW02
{
    public class Program
    {
        public static void Main()
        {
            try
            {
                CategoryDBContext categoryDBContext = new();
                ProductDBContext productDBContext = new(categoryDBContext);

                ProductManager productManager = new(productDBContext);
                CategoryManager categoryManager = new(categoryDBContext, productDBContext);
                OutputFormatter formatter = new(productDBContext, categoryDBContext);
                Parser parser = new(productManager, categoryManager, productDBContext, categoryDBContext, formatter);
                DataSeeder dataSeeder = new(categoryManager, productManager, parser);
                LoggerDBContext loggerDB = new();
                LoggerListener loggerListener = new(loggerDB);

                AnalyticalDBContext analyticalDBContext = new();
                AnalyticalDataListener analyticalDataListener = new(analyticalDBContext);
                EShopAdminUI eShopAdminUI = new(loggerListener, analyticalDataListener, parser, productDBContext);
                dataSeeder.Seed();
                eShopAdminUI.Run();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
