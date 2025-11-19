using AutoMapper;
using MetaKing.Orders;
using MetaKing.ProductAttributes;
using MetaKing.Manufacturers;
using MetaKing.ProductCategories;
using MetaKing.Products;
using MetaKing.Catalog.ProductCategories;
using MetaKing.Catalog.Products;
using MetaKing.Catalog.Manufacturers;
using MetaKing.Catalog.ProductAttributes;

namespace MetaKing;

public class MetaKingPublicApplicationAutoMapperProfile : Profile
{
    public MetaKingPublicApplicationAutoMapperProfile()
    {
        //Product Category
        CreateMap<ProductCategory, ProductCategoryDto>();
        CreateMap<ProductCategory, ProductCategoryInListDto>();

        //Product
        CreateMap<Product, ProductDto>();
        CreateMap<Product, ProductInListDto>();

        CreateMap<Manufacturer, ManufacturerDto>();
        CreateMap<Manufacturer, ManufacturerInListDto>();

        //Product attribute
        CreateMap<ProductAttribute, ProductAttributeDto>();
        CreateMap<ProductAttribute, ProductAttributeInListDto>();

        //Order
        CreateMap<Order, OrderDto>();
    }
}
