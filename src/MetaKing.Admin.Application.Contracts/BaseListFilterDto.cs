using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace MetaKing.Admin
{
    public class BaseListFilterDto : PagedResultRequestDto
    {
        public string? Keyword { get; set; }
        public string? SortField { get; set; }
        public string? SortOrder { get; set; }
    }
}
