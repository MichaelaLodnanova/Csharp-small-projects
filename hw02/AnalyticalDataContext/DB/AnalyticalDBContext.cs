﻿using HW02.BussinessContext.EShopAdminUI;
using HW02.Helpers;
using System.Text.Json;

namespace HW02.AnalyticalDataContext.DB
{
    public class AnalyticalDBContext
    {
        private readonly string[] _paths = { "..", "..", "..", "AnalyticalDataContext", "DB", "Storage", "AnalyticalData.json" };
        private readonly string _filePath;

        public AnalyticalDBContext()
        {
            _filePath = Path.Combine(_paths);
            FileHelper.CreateFile(_filePath);
        }

        public void SaveAnalyticalData(List<AnalyticalData> log)
        {
            string jsonString = JsonSerializer.Serialize(log);
            using StreamWriter outputFile = new StreamWriter(_filePath);
            outputFile.WriteLine(jsonString);
        }

        public List<AnalyticalData> ReadAnalyticalData()
        {
            string? line;
            using (StreamReader inputFile = new StreamReader(_filePath))
            {
                line = inputFile.ReadLine();
            }

            if (line == null)
            {
                return new List<AnalyticalData>();
            }

            var model = JsonSerializer.Deserialize<List<AnalyticalData>>(line);
            return model;
        }
    }
}
