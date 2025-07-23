using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaKing.ViewModels
{
    public class PaginationBase
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int Total { get; set; }

        public int PageCount
        {
            get
            {
                var pageCount = (double)Total / PageSize;
                return (int)Math.Ceiling(pageCount);
            }
        }
    }
}
