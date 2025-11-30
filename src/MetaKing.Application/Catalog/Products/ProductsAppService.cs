using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetaKing.Catalog.ProductCategories;
using MetaKing.Catalog.Products.Attributes;
using MetaKing.ProductAttributes;
using MetaKing.ProductCategories;
using MetaKing.Products;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Repositories;

namespace MetaKing.Catalog.Products
{
    public class ProductsAppService : ReadOnlyAppService<
        Product,
        ProductDto,
        Guid,
        PagedResultRequestDto>, IProductsAppService
    {
        private readonly IBlobContainer<ProductThumbnailPictureContainer> _fileContainer;
        private readonly IRepository<ProductAttribute> _productAttributeRepository;
        private readonly IRepository<ProductAttributeDateTime> _productAttributeDateTimeRepository;
        private readonly IRepository<ProductAttributeInt> _productAttributeIntRepository;
        private readonly IRepository<ProductAttributeDecimal> _productAttributeDecimalRepository;
        private readonly IRepository<ProductAttributeVarchar> _productAttributeVarcharRepository;
        private readonly IRepository<ProductAttributeText> _productAttributeTextRepository;
        private readonly IRepository<Product, Guid> _productRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IProductCategoriesAppService _productCategoriesAppService;

        public ProductsAppService(IRepository<Product, Guid> repository,
            IRepository<ProductCategory> productCategoryRepository,
            IBlobContainer<ProductThumbnailPictureContainer> fileContainer,
            IRepository<ProductAttribute> productAttributeRepository,
            IRepository<ProductAttributeDateTime> productAttributeDateTimeRepository,
            IRepository<ProductAttributeInt> productAttributeIntRepository,
            IRepository<ProductAttributeDecimal> productAttributeDecimalRepository,
            IRepository<ProductAttributeVarchar> productAttributeVarcharRepository,
            IRepository<ProductAttributeText> productAttributeTextRepository,
            IRepository<Product, Guid> productRepository,
            IProductCategoriesAppService productCategoriesAppService
            )
        : base(repository)
        {
            _fileContainer = fileContainer;
            _productAttributeRepository = productAttributeRepository;
            _productAttributeDateTimeRepository = productAttributeDateTimeRepository;
            _productAttributeIntRepository = productAttributeIntRepository;
            _productAttributeDecimalRepository = productAttributeDecimalRepository;
            _productAttributeVarcharRepository = productAttributeVarcharRepository;
            _productAttributeTextRepository = productAttributeTextRepository;
            _productRepository = productRepository;
            _productCategoriesAppService = productCategoriesAppService;
        }

        public async Task<List<ProductInListDto>> GetListAllAsync()
        {
            var query = await Repository.GetQueryableAsync();
            query = query.Where(x => x.IsActive == true);
            var data = await AsyncExecuter.ToListAsync(query);

            return ObjectMapper.Map<List<Product>, List<ProductInListDto>>(data);
        }


        public async Task<PagedResult<ProductInListDto>> GetListFilterAsync(ProductListFilterDto input)
        {
            var query = await Repository.GetQueryableAsync();
            query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Name.Contains(input.Keyword));
            query = query.WhereIf(input.CategoryId.HasValue, x => x.CategoryId == input.CategoryId);

            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter
               .ToListAsync(
                  query.Skip((input.CurrentPage - 1) * input.PageSize)
               .Take(input.PageSize));

            return new PagedResult<ProductInListDto>(
                ObjectMapper.Map<List<Product>, List<ProductInListDto>>(data),
                totalCount,
                input.CurrentPage,
                input.PageSize
            );
        }

    
        public async Task<string> GetThumbnailImageAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }
            var thumbnailContent = await _fileContainer.GetAllBytesOrNullAsync(fileName);

            if (thumbnailContent is null)
            {
                return null;
            }
            var result = Convert.ToBase64String(thumbnailContent);
            return result;
        }

        public async Task<List<ProductAttributeValueDto>> GetListProductAttributeAllAsync(Guid productId)
        {
            var attributeQuery = await _productAttributeRepository.GetQueryableAsync();

            var attributeDateTimeQuery = await _productAttributeDateTimeRepository.GetQueryableAsync();
            var attributeDecimalQuery = await _productAttributeDecimalRepository.GetQueryableAsync();
            var attributeIntQuery = await _productAttributeIntRepository.GetQueryableAsync();
            var attributeVarcharQuery = await _productAttributeVarcharRepository.GetQueryableAsync();
            var attributeTextQuery = await _productAttributeTextRepository.GetQueryableAsync();

            var query = from a in attributeQuery
                        join adate in attributeDateTimeQuery on a.Id equals adate.AttributeId into aDateTimeTable
                        from adate in aDateTimeTable.DefaultIfEmpty()
                        join adecimal in attributeDecimalQuery on a.Id equals adecimal.AttributeId into aDecimalTable
                        from adecimal in aDecimalTable.DefaultIfEmpty()
                        join aint in attributeIntQuery on a.Id equals aint.AttributeId into aIntTable
                        from aint in aIntTable.DefaultIfEmpty()
                        join aVarchar in attributeVarcharQuery on a.Id equals aVarchar.AttributeId into aVarcharTable
                        from aVarchar in aVarcharTable.DefaultIfEmpty()
                        join aText in attributeTextQuery on a.Id equals aText.AttributeId into aTextTable
                        from aText in aTextTable.DefaultIfEmpty()
                        where (adate == null || adate.ProductId == productId)
                        && (adecimal == null || adecimal.ProductId == productId)
                         && (aint == null || aint.ProductId == productId)
                          && (aVarchar == null || aVarchar.ProductId == productId)
                           && (aText == null || aText.ProductId == productId)
                        select new ProductAttributeValueDto()
                        {
                            Label = a.Label,
                            AttributeId = a.Id,
                            DataType = a.DataType,
                            Code = a.Code,
                            ProductId = productId,
                            DateTimeValue = adate != null ? adate.Value : null,
                            DecimalValue = adecimal != null ? adecimal.Value : null,
                            IntValue = aint != null ? aint.Value : null,
                            TextValue = aText != null ? aText.Value : null,
                            VarcharValue = aVarchar != null ? aVarchar.Value : null,
                            DateTimeId = adate != null ? adate.Id : null,
                            DecimalId = adecimal!=null? adecimal.Id : null,
                            IntId = aint != null ? aint.Id : null,
                            TextId = aText != null ? aText.Id : null,
                            VarcharId = aVarchar != null ? aVarchar.Id : null,
                        };
            query = query.Where(x => x.DateTimeId != null
                           || x.DecimalId != null
                           || x.IntValue != null
                           || x.TextId != null
                           || x.VarcharId != null);
            return await AsyncExecuter.ToListAsync(query);
        }


        public async Task<PagedResult<ProductAttributeValueDto>> GetListProductAttributesAsync(ProductAttributeListFilterDto input)
        {
            var attributeQuery = await _productAttributeRepository.GetQueryableAsync();

            var attributeDateTimeQuery = await _productAttributeDateTimeRepository.GetQueryableAsync();
            var attributeDecimalQuery = await _productAttributeDecimalRepository.GetQueryableAsync();
            var attributeIntQuery = await _productAttributeIntRepository.GetQueryableAsync();
            var attributeVarcharQuery = await _productAttributeVarcharRepository.GetQueryableAsync();
            var attributeTextQuery = await _productAttributeTextRepository.GetQueryableAsync();

            var query = from a in attributeQuery
                        join adate in attributeDateTimeQuery on a.Id equals adate.AttributeId into aDateTimeTable
                        from adate in aDateTimeTable.DefaultIfEmpty()
                        join adecimal in attributeDecimalQuery on a.Id equals adecimal.AttributeId into aDecimalTable
                        from adecimal in aDecimalTable.DefaultIfEmpty()
                        join aint in attributeIntQuery on a.Id equals aint.AttributeId into aIntTable
                        from aint in aIntTable.DefaultIfEmpty()
                        join aVarchar in attributeVarcharQuery on a.Id equals aVarchar.AttributeId into aVarcharTable
                        from aVarchar in aVarcharTable.DefaultIfEmpty()
                        join aText in attributeTextQuery on a.Id equals aText.AttributeId into aTextTable
                        from aText in aTextTable.DefaultIfEmpty()
                        where (adate == null || adate.ProductId == input.ProductId)
                        && (adecimal == null || adecimal.ProductId == input.ProductId)
                         && (aint == null || aint.ProductId == input.ProductId)
                          && (aVarchar == null || aVarchar.ProductId == input.ProductId)
                           && (aText == null || aText.ProductId == input.ProductId)
                        select new ProductAttributeValueDto()
                        {
                            Label = a.Label,
                            AttributeId = a.Id,
                            DataType = a.DataType,
                            Code = a.Code,
                            ProductId = input.ProductId,
                            DateTimeValue = adate != null ? adate.Value : null,
                            DecimalValue = adecimal != null ? adecimal.Value : null,
                            IntValue = aint != null ? aint.Value : null,
                            TextValue = aText != null ? aText.Value : null,
                            VarcharValue = aVarchar != null ? aVarchar.Value : null,
                            DateTimeId = adate != null ? adate.Id : null,
                            DecimalId = adecimal != null ? adecimal.Id : null,
                            IntId = aint != null ? aint.Id : null,
                            TextId = aText != null ? aText.Id : null,
                            VarcharId = aVarchar != null ? aVarchar.Id : null,
                        };
            query = query.Where(x => x.DateTimeId != null 
            || x.DecimalId != null 
            || x.IntValue != null 
            || x.TextId != null 
            || x.VarcharId != null);
            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter
               .ToListAsync(
                  query.Skip((input.CurrentPage - 1) * input.PageSize)
               .Take(input.PageSize));

            return new PagedResult<ProductAttributeValueDto>(data,
                totalCount,
                input.CurrentPage,
                input.PageSize
            );
        }

        public async Task<ProductDto> GetBySlugAsync(string slug)
        {
            var query = await _productRepository.GetQueryableAsync();
            var product = query.Where(p => p.Slug == slug).FirstOrDefault();
            if (product == null)
            {
                return null;
            }
            return ObjectMapper.Map<Product, ProductDto>(product);
        }

        public async Task<List<ProductDto>> GetProductsByParentCategoryAsync(Guid parentCategoryId)
        {
            var categoryQuery = await _productCategoryRepository.GetQueryableAsync();
            var allCategories = await AsyncExecuter.ToListAsync(categoryQuery);

            // Lấy tất cả category con (đệ quy)
            List<Guid> GetAllChildren(Guid parentId)
            {
                var children = allCategories
                    .Where(c => c.ParentId == parentId)
                    .Select(c => c.Id)
                    .ToList();

                var allChildIds = new List<Guid>(children);

                foreach (var child in children)
                {
                    allChildIds.AddRange(GetAllChildren(child));
                }

                return allChildIds;
            }

            var categoryIds = GetAllChildren(parentCategoryId);
            categoryIds.Add(parentCategoryId); // Thêm chính danh mục cha

            // Lấy sản phẩm theo danh mục cha + danh mục con
            var productQuery = await _productRepository.GetQueryableAsync();
            var products = productQuery
                .Where(p => categoryIds.Contains(p.CategoryId))
                .ToList();

            return ObjectMapper.Map<List<Product>, List<ProductDto>>(products);
        }
        public async Task<List<ProductDto>> GetProductsByDirectChildrenAsync(Guid parentCategoryId)
        {
            var categoryQuery = await _productCategoryRepository.GetQueryableAsync();

            // Lấy các category con trực tiếp
            var directChildCategories = await AsyncExecuter.ToListAsync(
                categoryQuery.Where(c => c.ParentId == parentCategoryId)
            );

            var childCategoryIds = directChildCategories.Select(c => c.Id).ToList();

            if (!childCategoryIds.Any())
            {
                return new List<ProductDto>();
            }

            // Lấy sản phẩm thuộc các danh mục con trực tiếp
            var productQuery = await _productRepository.GetQueryableAsync();
            var products = productQuery
                .Where(p => childCategoryIds.Contains(p.CategoryId))
                .ToList();

            return ObjectMapper.Map<List<Product>, List<ProductDto>>(products);
        }

        public async Task<List<ProductInListDto>> GetListByParentCategoryAsync(Guid parentCategoryId)
        {
            var categoryIds = await _productCategoriesAppService.GetAllChildrenIdsAsync(parentCategoryId);

            var query = await Repository.GetQueryableAsync();
            var products = query.Where(x => categoryIds.Contains(x.CategoryId)).ToList();

            return ObjectMapper.Map<List<Product>, List<ProductInListDto>>(products);
        }

        public async Task<List<ProductInListDto>> GetListByChildCategoryAsync(Guid parentCategoryId)
        {
            var children = await _productCategoriesAppService.GetChildrenAsync(parentCategoryId);
            var childIds = children.Select(x => x.Id).ToList();

            var query = await Repository.GetQueryableAsync();
            var products = query.Where(x => childIds.Contains(x.CategoryId)).ToList();

            return ObjectMapper.Map<List<Product>, List<ProductInListDto>>(products);
        }

        public async Task<List<ProductInListDto>> GetListByCategoryIdsAsync(List<Guid> categoryIds)
        {
            var query = await Repository.GetQueryableAsync();

            var products = query
                .Where(x => categoryIds.Contains(x.CategoryId))
                .ToList();

            return ObjectMapper.Map<List<Product>, List<ProductInListDto>>(products);
        }
    }
}