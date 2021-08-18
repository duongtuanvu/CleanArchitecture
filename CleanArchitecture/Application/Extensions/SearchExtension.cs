using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Extensions
{
    public class SearchExtension
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string Keyword { get; set; }
        public string OrderBy { get; set; }
        public bool OrderByDesc { get; set; } = true;

        /// <summary>
        /// Lấy danh sách các Field được mapping
        /// Key: Key dùng để mapping,
        /// Value: Field trong Database
        /// </summary>
        /// <returns>Danh sách các Field được mapping</returns>
        public virtual Dictionary<string, string> GetFieldMapping()
        {
            return new Dictionary<string, string>();
        }

        /// <summary>
        /// Lấy danh sách sort mặc định
        /// Key: Key dùng để mapping,
        /// Value: ASC/DESC
        /// </summary>
        /// <returns>Danh sách sort mặc định</returns>
        public virtual Dictionary<string, string> GetDefaultSortField()
        {
            return new Dictionary<string, string>();
        }
    }
}
