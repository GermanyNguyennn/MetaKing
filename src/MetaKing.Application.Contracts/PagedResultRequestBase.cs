using System;
using System.Collections.Generic;
using System.Text;

namespace MetaKing
{
    public class PagedResultRequestBase
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 6;
    }
}
