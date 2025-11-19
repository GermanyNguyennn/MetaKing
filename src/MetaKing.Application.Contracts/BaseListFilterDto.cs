using System;
using System.Collections.Generic;
using System.Text;

namespace MetaKing
{
    public class BaseListFilterDto : PagedResultRequestBase
    {
        public string? Keyword { get; set; } = string.Empty;
    }
}
