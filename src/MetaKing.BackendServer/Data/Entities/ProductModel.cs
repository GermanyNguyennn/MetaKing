using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MetaKing.BackendServer.Data.Interfaces;
using MetaKing.BackendServer.Data.Entities;
using MetaKing.ViewModels.Enum;
using System.Text.Json.Serialization;

namespace MetaKing.BackendServer.Data
{
    public class ProductModel : IDateTracking
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public ProductType ProductType { get; set; }
        public string Slug { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public int CompanyId { get; set; }
        public StatusType Status { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [ForeignKey("CategoryId")]
        [JsonIgnore]
        public CategoryModel? Category { get; set; }
        [ForeignKey("BrandId")]
        [JsonIgnore]
        public BrandModel? Brand { get; set; }
        [ForeignKey("CompanyId")]
        [JsonIgnore]
        public CompanyModel? Company { get; set; }

        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }

        public ProductDetailPhoneModel? ProductDetailPhones { get; set; }
        public ProductDetailLaptopModel? ProductDetailLaptops { get; set; }
        public ProductDetailWatchModel? ProductDetailWatches { get; set; }
        public ICollection<OrderDetailModel> OrderDetails { get; set; } = new List<OrderDetailModel>();
        public ICollection<ProductVariantModel> ProductVariants { get; set; } = new List<ProductVariantModel>();
    }
}
