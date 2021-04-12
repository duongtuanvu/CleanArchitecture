using Application.Features.ExampleFeature.Queries;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commons
{
    public static class Excel
    {
        public static async Task<object> ReadDataFromExcelFile<T>(IFormFile file) where T : class, new()
        {
            if (file == null || file.Length <= 0)
            {
                return "File is empty";
            }

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return "Not support file extension";
            }

            var data = new List<T>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var lstProperties = typeof(T).GetProperties();

                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    var rowCount = worksheet.Dimension.Rows;
                    var colCount = worksheet.Dimension.Columns;
                    var dictionaryHeader = new Dictionary<string, int>();
                    for (int i = 1; i <= colCount; i++)
                    {
                        var headerName = worksheet.Cells[1, i].Value.ToString().Trim();
                        dictionaryHeader.Add(headerName, worksheet.Cells["1:1"].First(c => c.Value.ToString().Equals(headerName)).Start.Column);
                    }
                    for (int row = 3; row <= rowCount; row++)
                    {
                        var dynamicObject = new ExpandoObject() as IDictionary<string, Object>;
                        foreach (var prop in lstProperties)
                        {
                            dynamicObject.Add(prop.Name, worksheet.Cells[row, dictionaryHeader[prop.Name]].Value.ToString().Trim());
                        }
                        data.Add(dynamicObject.ToObject<T>());
                    }
                }
            }
            return data;
        }

        public static TObject ToObject<TObject>(this IDictionary<string, object> someSource, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public) where TObject : class, new()
        {
            Contract.Requires(someSource != null);
            TObject targetObject = new TObject();
            Type targetObjectType = typeof(TObject);

            // Go through all bound target object type properties...
            foreach (PropertyInfo property in
                        targetObjectType.GetProperties(bindingFlags))
            {
                // ...and check that both the target type property name and its type matches
                // its counterpart in the ExpandoObject
                if (someSource.ContainsKey(property.Name)
                    && property.PropertyType == someSource[property.Name].GetType())
                {
                    property.SetValue(targetObject, someSource[property.Name]);
                }
            }

            return targetObject;
        }
    }
}
