using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using static MetaKing.Admin.Permissions.MetaKingPermissions;

namespace MetaKing.Admin.Catalog.ProductCategories
{
    public interface IProductCategoriesAppService : ICrudAppService
        <ProductCategoryDto,
        Guid, 
        PagedResultRequestDto,
        CreateUpdateProductCategoryDto, 
        CreateUpdateProductCategoryDto>
    {
        Task<PagedResultDto<ProductCategoryInListDto>> GetListFilterAsync(BaseListFilterDto input);
        Task<List<ProductCategoryInListDto>> GetListAllAsync();
        Task<List<ProductCategoryInListDto>> GetListParentAsync();
        Task<List<ProductCategoryInListDto>> GetListChildAsync(Guid parentId);
        Task DeleteMultipleAsync(IEnumerable<Guid> ids);
    }
}
