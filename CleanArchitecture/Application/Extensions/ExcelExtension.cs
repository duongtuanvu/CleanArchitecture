using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Core.Extensions
{
    public static class ExcelExtension
    {
        /// <summary>
        /// Read data from Excel file then return a List<typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static async Task<List<T>> ToListAsync<T>(this List<T> data, IFormFile file) where T : class, new()
        {
            if (file == null || file.Length <= 0)
            {
                throw new Exception($"File is empty");
            }

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"Not support file extension");
            }

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
                            try
                            {
                                object value = null;
                                var cell = worksheet.Cells[row, dictionaryHeader[prop.Name]];
                                var cellFormat = cell.Style.Numberformat.Format;
                                var cellValue = cell.Value.ToString().Trim();

                                if (prop.PropertyType == typeof(int))
                                {
                                    value = int.Parse(cellValue);
                                }
                                if (prop.PropertyType == typeof(string))
                                {
                                    value = cellValue;
                                }
                                if (prop.PropertyType == typeof(DateTime))
                                {
                                    value = DateTime.FromOADate(double.Parse(cellValue));
                                }
                                dynamicObject.Add(prop.Name, value);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception($"{ex.Message} [Column/Row: {prop.Name}/{row}]");
                            }
                        }
                        data.Add(dynamicObject.ToObject<T>());
                    }
                    return data;
                }
            }
        }

        /// <summary>
        /// Convert a list data to byte array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ToByteArray<T>(this IEnumerable<T> data) where T : class
        {
            using (var stream = new MemoryStream())
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet workSheet = package.Workbook.Worksheets.Add("Data");
                    workSheet.Cells.LoadFromCollection(data, true);
                    package.Save();
                }
                //stream.Position = 0;
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Convert dynamic object to T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="someSource"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static T ToObject<T>(this IDictionary<string, object> someSource, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public) where T : class, new()
        {
            Contract.Requires(someSource != null);
            T targetObject = new T();
            Type targetObjectType = typeof(T);
            // Go through all bound target object type properties...
            foreach (PropertyInfo property in targetObjectType.GetProperties(bindingFlags))
            {
                // ...and check that both the target type property name and its type matches
                // its counterpart in the ExpandoObject
                if (someSource.ContainsKey(property.Name) && property.PropertyType == someSource[property.Name].GetType())
                {
                    property.SetValue(targetObject, someSource[property.Name]);
                }
            }
            return targetObject;
        }
    }

    public class FormatExcel
    {
        public const string Date = "m/d/yy";

    }
}
