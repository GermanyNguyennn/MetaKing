        using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetaKing.Admin.Permissions;
using MetaKing.ProductAttributes;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace MetaKing.Admin.Catalog.ProductAttributes
{
    //[Authorize(MetaKingPermissions.Attribute.Default, Policy = "Admin")]
    [AllowAnonymous]
    public class ProductAttributesAppService : CrudAppService<
        ProductAttribute,
        ProductAttributeDto,
        Guid,
        PagedResultRequestDto,
        CreateUpdateProductAttributeDto,
        CreateUpdateProductAttributeDto>, IProductAttributesAppService
    {
        public ProductAttributesAppService(IRepository<ProductAttribute, Guid> repository)
            : base(repository)
        {
            //GetPolicyName = MetaKingPermissions.Attribute.Default;
            //GetListPolicyName = MetaKingPermissions.Attribute.Default;
            //CreatePolicyName = MetaKingPermissions.Attribute.Create;
            //UpdatePolicyName = MetaKingPermissions.Attribute.Update;
            //DeletePolicyName = MetaKingPermissions.Attribute.Delete;

            GetPolicyName = null;
            GetListPolicyName = null;
            CreatePolicyName = null;
            UpdatePolicyName = null;
            DeletePolicyName = null;
        }

        //[Authorize(MetaKingPermissions.Attribute.Delete)]
        public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
        {
            await Repository.DeleteManyAsync(ids);
            await UnitOfWorkManager.Current!.SaveChangesAsync();
        }

        //[Authorize(MetaKingPermissions.Attribute.Default)]

        public async Task<List<ProductAttributeInListDto>> GetListAllAsync()
        {
            var query = await Repository.GetQueryableAsync();
            query = query.Where(x => x.IsActive == true);
            var data = await AsyncExecuter.ToListAsync(query);

            return ObjectMapper.Map<List<ProductAttribute>, List<ProductAttributeInListDto>>(data);

        }

        //[Authorize(MetaKingPermissions.Attribute.Default)]
        public async Task<PagedResultDto<ProductAttributeInListDto>> GetListFilterAsync(BaseListFilterDto input)
        {
            // Base query
            var query = await Repository.GetQueryableAsync();

            // Filter
            query = query.WhereIf(
                !string.IsNullOrWhiteSpace(input.Keyword),
                x => x.Name.Contains(input.Keyword!)
            );

            // Chuẩn hoá sort
            var sortField = input.SortField?.ToLower();
            var sortOrder = input.SortOrder?.ToUpper() ?? "ASC";
            bool isAsc = sortOrder == "ASC";

            // Apply sorting
            query = sortField switch
            {
                "id" => isAsc ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id),
                "name   " => isAsc ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name),
                "code" => isAsc ? query.OrderBy(x => x.Code) : query.OrderByDescending(x => x.Code),
                "datatype" => isAsc ? query.OrderBy(x => x.DataType) : query.OrderByDescending(x => x.DataType),
                "visibility" => isAsc ? query.OrderBy(x => x.IsVisibility) : query.OrderByDescending(x => x.IsVisibility),
                "isactive" => isAsc ? query.OrderBy(x => x.IsActive) : query.OrderByDescending(x => x.IsActive),
                "isrequired" => isAsc ? query.OrderBy(x => x.IsRequired) : query.OrderByDescending(x => x.IsRequired),
                "isunique" => isAsc ? query.OrderBy(x => x.IsUnique) : query.OrderByDescending(x => x.IsUnique),
                _ => isAsc ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name),
            };

            // Count
            var totalCount = await AsyncExecuter.LongCountAsync(query);

            // Paging
            var items = await AsyncExecuter.ToListAsync(
                query
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
            );

            return new PagedResultDto<ProductAttributeInListDto>(
                totalCount,
                ObjectMapper.Map<List<ProductAttribute>, List<ProductAttributeInListDto>>(items)
            );
        }
    }
}
