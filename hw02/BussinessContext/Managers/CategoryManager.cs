using HW02.BussinessContext.DB.Entities;
using HW02.BussinessContext.FileDatabase;

namespace HW02.BussinessContext.Managers
{
    public class CategoryManager
    {
        private readonly CategoryDBContext categoryDB;
        private readonly ProductDBContext productDB;
        private int nextId = 1;

        public CategoryManager(CategoryDBContext categoryDBContext, ProductDBContext productDBContext)
        {
            categoryDB = categoryDBContext;
            productDB = productDBContext;
        }

        public int Create(Category category)
        {
            category.Id = nextId++;
            category.CategoryId = 0;
            List<Category> categories = categoryDB.ReadCategories();
            categories.Add(category);
            categoryDB.SaveCategories(categories);

            return category.Id;
        }

        public Category? Retrieve(int id)
        {
            List<Category> categories = categoryDB.ReadCategories();
            try
            {
                return categories.First(c => c.Id == id);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public void Delete(int id)
        {
            List<Category> categories = categoryDB.ReadCategories();
            int idx = categories.FindIndex(c => c.Id == id);
            if (idx == -1)
            {
                Console.WriteLine("Category does not exist.");
                return;
            }
            Category category = categories[idx];
            List<Product> products = productDB.ReadProducts();
            products.RemoveAll(p => p.CategoryId == id);
            productDB.SaveProducts(products);
            categories.Remove(category);
            categoryDB.SaveCategories(categories);
        }

        public List<Product> GetProductsByCategory(int categoryId)
        {
            List<Product> products = new List<Product>();
            List<Product> allProducts = productDB.ReadProducts();
            foreach (Product product in allProducts)
            {
                if (product.CategoryId == categoryId)
                {
                    products.Add(product);
                }
            }
            return products;
        }
    }
}
