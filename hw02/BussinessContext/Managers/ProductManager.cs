using HW02.BussinessContext.DB.Entities;
using HW02.BussinessContext.FileDatabase;

namespace HW02.BussinessContext.Managers
{
    public class ProductManager
    {
        private readonly ProductDBContext productDB;
        private int nextId = 1;
        public ProductManager(ProductDBContext productDB)
        {
            this.productDB = productDB;
        }

        public int Create(Product product)
        {
            List<Product> products = productDB.ReadProducts();
            product.Id = nextId++;
            products.Add(product);
            productDB.SaveProducts(products);
            return product.Id;
        }

        public Product? Retrieve(int id)
        {
            List<Product> products = productDB.ReadProducts();
            try
            {
                return products.First(p => p.Id == id);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public void Delete(int id)
        {
            List<Product> products = productDB.ReadProducts();
            int idx = products.FindIndex((p) => p.Id == id);
            if (idx != -1)
            {
                products.RemoveAt(idx);
            }
            productDB.SaveProducts(products);

        }
    }
}
