using Application.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleApi.Queries.ExampleQuey
{
    public class ExampleSearch : SearchExtension
    {
        /// <summary>
        /// Lấy danh sách các Field được mapping
        /// Key: Key dùng để mapping,
        /// Value: Field trong Database
        /// </summary>
        /// <returns>Danh sách các Field được mapping</returns>
        public override Dictionary<string, string> GetFieldMapping()
        {
            return new Dictionary<string, string> {
                 { "createddate", "CreatedDate" }, //{ "createdate", "Item.CreateDate" }
                 { "name", "Name" }
            };
        }

        /// <summary>
        /// Lấy danh sách sort mặc định
        /// Key: Key dùng để mapping,
        /// Value: ASC/DESC
        /// </summary>
        /// <returns>Danh sách sort mặc định</returns>
        public override Dictionary<string, string> GetDefaultSortField()
        {
            return new Dictionary<string, string> {
                 { "createddate", "DESC" }, //{ "createddate", "Item.DESC" }
                 { "name", "ASC" }
            };
        }
    }
}
