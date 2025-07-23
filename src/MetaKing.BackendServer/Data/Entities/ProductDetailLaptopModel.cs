using MetaKing.BackendServer.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MetaKing.BackendServer.Data
{
    public class ProductDetailLaptopModel : IDateTracking
    {
        [Key]
        public int Id { get; set; }
        public string? GraphicsCardType { get; set; } // Card đồ hoạ
        public string? RAMCapacity { get; set; } // RAM
        public string? RAMType { get; set; } // Loại RAM
        public string? NumberOfRAMSlots { get; set; } // Số khe RAM
        public string? HardDrive {  get; set; } // Ổ cứng
        public string? ScreenSize { get; set; } // Kích thước màn hình
        public string? ScreenTechnology { get; set; } // Công nghệ màn hình
        public string? Battery { get; set; } // Pin
        public string? OperatingSystem { get; set; } // Hệ điều hành
        public string? ScreenResolution { get; set; } // Độ phân giải màn hình
        public string? CPUType { get; set; } // Loại CPU
        public string? Interface {  get; set; } // Cổng giao tiếp
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [JsonIgnore]
        public ProductModel? Product { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [JsonIgnore]
        public CategoryModel? Category { get; set; }
        public int BrandId { get; set; }
        [ForeignKey("BrandId")]
        [JsonIgnore]
        public BrandModel? Brand { get; set; }
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        [JsonIgnore]
        public CompanyModel? Company { get; set; }
    }
}
