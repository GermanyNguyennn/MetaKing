using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaKing.ProductCategories;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace MetaKing.ProductCategories
{
    public class ProductCategoryManager : DomainService
    {
        private readonly IRepository<ProductCategory, Guid> _productCategoryRepository;
        public ProductCategoryManager(IRepository<ProductCategory, Guid> productCategoryRepository)
        {
            _productCategoryRepository = productCategoryRepository;
        }

        public async Task<ProductCategory> CreateAsync(string name, string code, string slug,
            int sortOrder, bool visibility,
            bool isActive, Guid? parentId,
            string seoMetaDescriptione)
        {
            if (await _productCategoryRepository.AnyAsync(x => x.Name == name))
                throw new UserFriendlyException("Tên sản phẩm đã tồn tại", MetaKingDomainErrorCodes.ProductNameAlreadyExists);
            if (await _productCategoryRepository.AnyAsync(x => x.Code == code))
                throw new UserFriendlyException("Mã sản phẩm đã tồn tại", MetaKingDomainErrorCodes.ProductCodeAlreadyExists);           

            return new ProductCategory(Guid.NewGuid(), name, code, slug, sortOrder, null, visibility, isActive, parentId, seoMetaDescriptione);
        }
    }
}
