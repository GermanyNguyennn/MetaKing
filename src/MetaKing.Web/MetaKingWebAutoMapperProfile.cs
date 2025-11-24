using AutoMapper;
using MetaKing.Catalog.Manufacturers;
using MetaKing.Catalog.ProductCategories;
using MetaKing.Catalog.Products;
using MetaKing.Orders;
using MetaKing.System.Users;
using Microsoft.AspNetCore.Identity;

namespace MetaKing
{
    public class MetaKingWebAutoMapperProfile : Profile
    {
        public MetaKingWebAutoMapperProfile()
        {
            // Product
            CreateMap<ProductInListDto, ProductDto>();
            CreateMap<ProductDto, ProductInListDto>();

            // ProductCategory
            CreateMap<ProductCategoryInListDto, ProductCategoryDto>();
            CreateMap<ProductCategoryDto, ProductCategoryInListDto>();

            // Manufacturer
            CreateMap<ManufacturerInListDto, ManufacturerDto>();
            CreateMap<ManufacturerDto, ManufacturerInListDto>();

            CreateMap<CreateOrderDto, Order>();
            CreateMap<OrderItemDto, OrderItem>();
            CreateMap<OrderItem, OrderItemDto>();

            CreateMap<IdentityUser, UserDto>();

        }
    }
}
