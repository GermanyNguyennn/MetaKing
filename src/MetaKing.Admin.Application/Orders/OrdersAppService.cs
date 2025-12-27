using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaKing.Orders;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace MetaKing.Admin.Orders
{
    //[Authorize(MetaKingPermissions.Order.Default, Policy = "Admin")]
    [AllowAnonymous]
    public class OrdersAppService : CrudAppService<
        Order,
        OrderDto,
        Guid,
        PagedResultRequestDto>, IOrdersAppService
    {
        public OrdersAppService(IRepository<Order, Guid> repository,
            IRepository<OrderItem> orderItemRepository)
            : base(repository)
        {
            GetPolicyName = null;
            GetListPolicyName = null;
            CreatePolicyName = null;
            UpdatePolicyName = null;
            DeletePolicyName = null;
        }

        // Get all active orders (no paging)
        public async Task<List<OrderDto>> GetListAllAsync()
        {
            var query = await Repository.GetQueryableAsync();
            var data = await AsyncExecuter.ToListAsync(query);
            return ObjectMapper.Map<List<Order>, List<OrderDto>>(data);
        }

        // Filter, search and sort orders for admin listing
        public async Task<PagedResultDto<OrderDto>> GetListFilterAsync(BaseListFilterDto input)
        {
            var query = await Repository.GetQueryableAsync();

            // Search by keyword across some fields
            query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword),
                x => x.Code.Contains(input.Keyword!)
                || x.CustomerName.Contains(input.Keyword!)
                || x.CustomerPhoneNumber.Contains(input.Keyword!)
                || x.CustomerAddress.Contains(input.Keyword!)
            );

            // Normalize sort
            var sortField = input.SortField?.ToLower();
            var sortOrder = input.SortOrder?.ToUpper() ?? "ASC";
            bool isAsc = sortOrder == "ASC";

            query = sortField switch
            {
                "id" => isAsc ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id),
                "code" => isAsc ? query.OrderBy(x => x.Code) : query.OrderByDescending(x => x.Code),
                "customername" => isAsc ? query.OrderBy(x => x.CustomerName) : query.OrderByDescending(x => x.CustomerName),
                "total" => isAsc ? query.OrderBy(x => x.Total) : query.OrderByDescending(x => x.Total),
                "status" => isAsc ? query.OrderBy(x => x.Status) : query.OrderByDescending(x => x.Status),
                "creationtime" => isAsc ? query.OrderBy(x => x.CreationTime) : query.OrderByDescending(x => x.CreationTime),
                _ => isAsc ? query.OrderBy(x => x.CreationTime) : query.OrderByDescending(x => x.CreationTime)
            };

            var totalCount = await AsyncExecuter.LongCountAsync(query);

            var items = await AsyncExecuter.ToListAsync(
                query
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
            );

            return new PagedResultDto<OrderDto>(    
                totalCount,
                ObjectMapper.Map<List<Order>, List<OrderDto>>(items)
            );
        }

        public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
        {
            await Repository.DeleteManyAsync(ids);
            await UnitOfWorkManager.Current!.SaveChangesAsync();
        }
    }
}
