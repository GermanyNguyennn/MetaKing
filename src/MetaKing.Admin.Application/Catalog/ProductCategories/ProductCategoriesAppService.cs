using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaKing.Admin.Permissions;
using MetaKing.ProductCategories;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace MetaKing.Admin.Catalog.ProductCategories
{
    //[Authorize(MetaKingPermissions.ProductCategory.Default, Policy = "Admin")]
    [AllowAnonymous]
    public class ProductCategoriesAppService : CrudAppService<
        ProductCategory,
        ProductCategoryDto,
        Guid,
        PagedResultRequestDto,
        CreateUpdateProductCategoryDto,
        CreateUpdateProductCategoryDto>, IProductCategoriesAppService
    {
        public ProductCategoriesAppService(IRepository<ProductCategory, Guid> repository)
            : base(repository)
        {
            //GetPolicyName = MetaKingPermissions.ProductCategory.Default;
            //GetListPolicyName = MetaKingPermissions.ProductCategory.Default;
            //CreatePolicyName = MetaKingPermissions.ProductCategory.Create;
            //UpdatePolicyName = MetaKingPermissions.ProductCategory.Update;
            //DeletePolicyName = MetaKingPermissions.ProductCategory.Delete;

            GetPolicyName = null;
            GetListPolicyName = null;
            CreatePolicyName = null;
            UpdatePolicyName = null;
            DeletePolicyName = null;
        }

        //[Authorize(MetaKingPermissions.ProductCategory.Delete)]

        public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
        {
            await Repository.DeleteManyAsync(ids);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        //[Authorize(MetaKingPermissions.ProductCategory.Default)]

        public async Task<List<ProductCategoryInListDto>> GetListAllAsync()
        {
            var query = await Repository.GetQueryableAsync();
            query = query.Where(x=>x.IsActive == true);
            var data = await AsyncExecuter.ToListAsync(query);

            return ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryInListDto>>(data);

        }

        //[Authorize(MetaKingPermissions.ProductCategory.Default)]
        public async Task<List<ProductCategoryInListDto>> GetListParentAsync()
        {
            var query = await Repository.GetQueryableAsync();
            query = query.Where(x => x.ParentId == null && x.IsActive == true);
            var data = await AsyncExecuter.ToListAsync(query);
            return ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryInListDto>>(data);
        }

        //[Authorize(MetaKingPermissions.ProductCategory.Default)]
        public async Task<List<ProductCategoryInListDto>> GetListChildAsync(Guid parentId)
        {
            var query = await Repository.GetQueryableAsync();
            query = query.Where(x => x.ParentId == parentId && x.IsActive == true);
            var data = await AsyncExecuter.ToListAsync(query);
            return ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryInListDto>>(data);
        }

        //[Authorize(MetaKingPermissions.ProductCategory.Default)]
        public async Task<PagedResultDto<ProductCategoryInListDto>> GetListFilterAsync(BaseListFilterDto input)
        {
            var query = await Repository.GetQueryableAsync();
            query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Name.Contains(input.Keyword));

            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));

            return new PagedResultDto<ProductCategoryInListDto>(totalCount,ObjectMapper.Map<List<ProductCategory>,List<ProductCategoryInListDto>>(data));
        }
    }
}
