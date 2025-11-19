using MetaKing.Catalog.ProductCategories;
using MetaKing.Catalog.Products;
using MetaKing.Catalog.Products.Attributes;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;

namespace MetaKing.Pages.Products
{
    public class DetailsModel : PageModel
    {
        private readonly IProductsAppService _productsAppService;
        private readonly IProductCategoriesAppService _productCategoriesAppService;
        private readonly IObjectMapper _objectMapper;

        public ProductCategoryDto Category { get; set; } = new ProductCategoryDto();
        public ProductDto Product { get; set; } = new ProductDto();
        public List<ProductDto> RelatedProducts { get; set; } = new List<ProductDto>();
        public List<ProductAttributeValueDto> Attributes { get; set; } = new List<ProductAttributeValueDto>();


        public DetailsModel(
            IProductsAppService productsAppService,
            IProductCategoriesAppService productCategoriesAppService,
            IObjectMapper objectMapper)
        {
            _productsAppService = productsAppService;
            _productCategoriesAppService = productCategoriesAppService;
            _objectMapper = objectMapper;
        }

        public async Task OnGetAsync(string categorySlug, string slug)
        {
            Category = await _productCategoriesAppService.GetBySlugAsync(categorySlug);;

            Product = await _productsAppService.GetBySlugAsync(slug);

            Attributes = await _productsAppService.GetListProductAttributeAllAsync(Product.Id);

            var allProducts = await _productsAppService.GetListAllAsync();
            var related = allProducts
                .Where(p => p.CategoryId == Product.CategoryId && p.Id != Product.Id)
                .Distinct()
                .Take(4)
                .ToList();

            RelatedProducts = related.Select(p => _objectMapper.Map<ProductInListDto, ProductDto>(p)).ToList();

        }
    }
}
