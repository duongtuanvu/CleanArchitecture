﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common
{
    public class SearchCommon
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string Keyword { get; set; }
        public string OrderBy { get; set; }
        public bool OrderByDesc { get; set; } = true;
    }
}
