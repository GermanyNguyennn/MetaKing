using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MetaKing.Admin.Orders
{
    public interface IOrdersAppService : ICrudAppService
        <OrderDto,
        Guid,
        PagedResultRequestDto>
    {
        Task<List<OrderDto>> GetListAllAsync();
        Task<PagedResultDto<OrderDto>> GetListFilterAsync(BaseListFilterDto input);
    }
}
