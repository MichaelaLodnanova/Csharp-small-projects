
using HW02.BussinessContext.DB.Entities;
using HW02.BussinessContext.FileDatabase;
using System.Text;

namespace HW02.BussinessContext.EShopAdminUI
{
    public class OutputFormatter
    {
        private string[] headerProducts = { "ID", "Name", "CategoryId", "Price" };
        private string[] headerCategories = { "ID", "Name" };
        private readonly ProductDBContext db;
        private readonly CategoryDBContext cdb;

        public OutputFormatter(ProductDBContext db, CategoryDBContext cdb)
        {
            this.db = db;
            this.cdb = cdb;
        }

        private int[] GetColumnLengths(List<Product> data)
        {
            int[] columnLengths = new int[headerProducts.Length];
            for (int i = 0; i < headerProducts.Length; i++)
            {
                columnLengths[i] = headerProducts[i].Length;
            }
            foreach (Product p in data)
            {
                if (p.Id.ToString().Length > columnLengths[0])
                {
                    columnLengths[0] = p.Id.ToString().Length;
                }
                if (p.Name.Length > columnLengths[1])
                {
                    columnLengths[1] = p.Name.Length;
                }
                if (p.CategoryId.ToString().Length > columnLengths[2])
                {
                    columnLengths[2] = p.CategoryId.ToString().Length;
                }
                if (p.Price.ToString().Length > columnLengths[3])
                {
                    columnLengths[3] = p.Price.ToString().Length;
                }
            }
            return columnLengths;
        }
        private int[] GetColumnLengths(List<Category> data)
        {
            int[] columnLengths = new int[headerCategories.Length];
            for (int i = 0; i < headerCategories.Length; i++)
            {
                columnLengths[i] = headerCategories[i].Length;
            }
            foreach (Category c in data)
            {
                if (c.Id.ToString().Length > columnLengths[0])
                {
                    columnLengths[0] = c.Id.ToString().Length;
                }
                if (c.Name.Length > columnLengths[1])
                {
                    columnLengths[1] = c.Name.Length;
                }
            }
            return columnLengths;
        }
        public string GetFormattedTable(List<Product> data)
        {
            StringBuilder table = new StringBuilder();
            int[] columnLengths = GetColumnLengths(data);
            string headerRow = string.Empty;
            for (int i = 0; i < headerProducts.Length - 1; i++)
            {
                headerRow += String.Format("{0,-" + columnLengths[i] + "} | ", headerProducts[i]);
            }
            headerRow += String.Format("{0,-" + columnLengths[3] + "} ", headerProducts[3]);
            string separator = new string('-', headerRow.Length - 1);
            table.AppendLine(headerRow);
            table.AppendLine(separator);

            for (int i = 0; i < data.Count; i++)
            {
                string dataRow = string.Empty;
                dataRow += String.Format("{0,-" + columnLengths[0] + "} | ", data[i].Id.ToString());
                dataRow += String.Format("{0,-" + columnLengths[1] + "} | ", data[i].Name);
                dataRow += String.Format("{0,-" + columnLengths[2] + "} | ", data[i].CategoryId.ToString());
                dataRow += String.Format("{0,-" + columnLengths[3] + "} ", data[i].Price.ToString());
                table.AppendLine(dataRow);
            }

            return table.ToString();
        }

        public string GetFormattedCategoriesTable()
        {
            StringBuilder table = new StringBuilder();
            List<Category> data = cdb.ReadCategories();
            int[] columnLengths = GetColumnLengths(data);
            string headerRow = string.Empty;
            headerRow += String.Format("{0,-" + columnLengths[0] + "} | ", headerCategories[0]);
            headerRow += String.Format("{0,-" + columnLengths[1] + "} ", headerCategories[1]);
            string separator = new string('-', headerRow.Length - 1);
            table.AppendLine(headerRow);
            table.AppendLine(separator);

            for (int i = 0; i < data.Count; i++)
            {
                string dataRow = string.Empty;
                dataRow += String.Format("{0,-" + columnLengths[0] + "} | ", data[i].Id.ToString());
                dataRow += String.Format("{0,-" + columnLengths[1] + "} ", data[i].Name);
                table.AppendLine(dataRow);
            }
            return table.ToString();
        }
    }
}
