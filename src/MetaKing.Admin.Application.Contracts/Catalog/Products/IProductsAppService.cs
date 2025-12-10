using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MetaKing.Admin.Catalog.Products.Attributes;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MetaKing.Admin.Catalog.Products
{
    public interface IProductsAppService : ICrudAppService
        <ProductDto,
        Guid, 
        PagedResultRequestDto,
        CreateUpdateProductDto, 
        CreateUpdateProductDto>
    {
        Task<PagedResultDto<ProductInListDto>> GetListFilterAsync(ProductListFilterDto input);
        Task<List<ProductInListDto>> GetListAllAsync();
        Task DeleteMultipleAsync(IEnumerable<Guid> ids);

        Task<string> GetThumbnailImageAsync(string fileName);
        Task<string> GetSuggestNewCodeAsync();

        Task<ProductAttributeValueDto> AddProductAttributeAsync(CreateUpdateProductAttributeDto input);
        Task<ProductAttributeValueDto> UpdateProductAttributeAsync(Guid id, CreateUpdateProductAttributeDto input);

        Task RemoveProductAttributeAsync(Guid attributeId, Guid id);

        Task<List<ProductAttributeValueDto>> GetListProductAttributeAllAsync(Guid productId);
        Task<PagedResultDto<ProductAttributeValueDto>> GetListProductAttributesAsync(ProductAttributeListFilterDto input);
    }
}
