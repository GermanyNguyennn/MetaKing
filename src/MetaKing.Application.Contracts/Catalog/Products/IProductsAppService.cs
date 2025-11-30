using MetaKing.Catalog.Products.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MetaKing.Catalog.Products
{
    public interface IProductsAppService : IReadOnlyAppService
        <ProductDto,
        Guid, 
        PagedResultRequestDto>
    {
        Task<PagedResult<ProductInListDto>> GetListFilterAsync(ProductListFilterDto input);
        Task<List<ProductInListDto>> GetListAllAsync();
        Task<string> GetThumbnailImageAsync(string fileName);
        Task<List<ProductAttributeValueDto>> GetListProductAttributeAllAsync(Guid productId);
        Task<PagedResult<ProductAttributeValueDto>> GetListProductAttributesAsync(ProductAttributeListFilterDto input);
        Task<ProductDto> GetBySlugAsync(string slug);
        Task<List<ProductDto>> GetProductsByParentCategoryAsync(Guid parentCategoryId);
        Task<List<ProductDto>> GetProductsByDirectChildrenAsync(Guid parentCategoryId);
        Task<List<ProductInListDto>> GetListByCategoryIdsAsync(List<Guid> categoryIds);

    }
}
