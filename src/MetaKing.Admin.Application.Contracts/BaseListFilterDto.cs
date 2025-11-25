using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace MetaKing.Admin
{
    public class BaseListFilterDto : PagedResultRequestDto
    {
        public string? Keyword { get; set; } = string.Empty;
        public string? Sorting { get; set; } = string.Empty;

    }
}
