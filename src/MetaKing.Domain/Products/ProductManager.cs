using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaKing.ProductCategories;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace MetaKing.Products
{
    public class ProductManager : DomainService
    {
        private readonly IRepository<Product, Guid> _productRepository;
        private readonly IRepository<ProductCategory, Guid> _productCategoryRepository;
        public ProductManager(IRepository<Product,Guid> productRepository,
             IRepository<ProductCategory, Guid> productCategoryRepository)
        {
            _productCategoryRepository = productCategoryRepository;
            _productRepository = productRepository;
        }

        public async Task<Product> CreateAsync(Guid manufacturerId,
            string name, string slug,
            ProductType productType, string code,  bool isVisibility,
            bool isActive, Guid categoryId,
            string seoMetaDescription, string description, double sellPrice)
        {
            if (await _productRepository.AnyAsync(x => x.Name == name))
                throw new UserFriendlyException("Tên sản phẩm đã tồn tại", MetaKingDomainErrorCodes.ProductNameAlreadyExists);

            var category =  await _productCategoryRepository.GetAsync(categoryId);

            return new Product(Guid.NewGuid(), manufacturerId, name, slug, productType, code, isVisibility, isActive, categoryId, seoMetaDescription, description, string.Empty, sellPrice, category?.Name!, category?.Slug!);
        }
    }
}
