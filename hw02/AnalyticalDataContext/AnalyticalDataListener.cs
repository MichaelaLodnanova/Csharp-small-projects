using HW02.AnalyticalDataContext.DB;
using HW02.BussinessContext.DB.Entities;
using HW02.BussinessContext.EShopAdminUI;

namespace HW02.AnalyticalDataContext
{
    public class AnalyticalDataListener
    {
        private readonly AnalyticalDBContext _dbContext;
        public AnalyticalDataListener(AnalyticalDBContext analyticalDB)
        {
            _dbContext = analyticalDB;
        }

        public void OnCommandProcessed(Object sender, CommandParsedEventArgs e)
        {
            var data = _dbContext.ReadAnalyticalData();
            switch(e.entity)
            {
                case Category:
                    if (e.logType == BussinessContext.DB.EShopAdminUI.CommandType.Add)
                    {
                        AnalyticalData? categoryData = AddCategory(e);
                        if (categoryData != null)
                        {
                            data.Add(categoryData);
                        }
                    }
                    else if (e.logType == BussinessContext.DB.EShopAdminUI.CommandType.Delete)
                    {
                        DeleteCategory(data, e.entity.Id);
                    }                    
                    break;
                case Product:
                    if (e.logType == BussinessContext.DB.EShopAdminUI.CommandType.Add)
                    {
                        AddProduct(data, e.entity.CategoryId);
                    }
                    else if (e.logType == BussinessContext.DB.EShopAdminUI.CommandType.Delete)
                    {
                        DeleteProduct(data, e.entity.CategoryId);
                    }                    
                    break;
                default: break;
            }
            _dbContext.SaveAnalyticalData(data);
        }

        private AnalyticalData? AddCategory(CommandParsedEventArgs e)
        {
            if (e == null) return null;
            AnalyticalData categoryData = new AnalyticalData
            {
                CategoryId = e.entity.Id,
                CategoryName = e.entity.Name,
                ProductCount = 0
            };
            return categoryData;
        }

        private void AddProduct(List<AnalyticalData> analyticalDataList, int categoryId)
        {
            if (analyticalDataList.Count == 0) return;
            AnalyticalData? currentData = analyticalDataList.FirstOrDefault(d => d.CategoryId == categoryId);
            if (currentData == null) return;
            currentData.ProductCount++;
        }

        private void DeleteCategory(List<AnalyticalData> analyticalDataList, int categoryId)
        {
            if (analyticalDataList.Count == 0) return;
            AnalyticalData? currentData = analyticalDataList.FirstOrDefault(d => d.CategoryId == categoryId);
            if (currentData == null) return;
            analyticalDataList.Remove(currentData);
        }

        private void DeleteProduct(List<AnalyticalData> analyticalDataList, int categoryId)
        {
            if (analyticalDataList.Count == 0) return;
            AnalyticalData? currentData = analyticalDataList.FirstOrDefault(d => d.CategoryId == categoryId);
            if (currentData == null) return;
            currentData.ProductCount--;
        }
    }
}
