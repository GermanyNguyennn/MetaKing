using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MetaKing.Orders
{
    public interface IOrdersAppService : ICrudAppService
        <OrderDto,
        Guid,
        PagedResultRequestDto, CreateOrderDto,CreateOrderDto>
    {
        Task<OrderDto> CreateAsync(CreateOrderDto input);
    }
}
