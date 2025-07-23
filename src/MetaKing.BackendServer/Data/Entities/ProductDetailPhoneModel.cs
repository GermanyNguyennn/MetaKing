using MetaKing.BackendServer.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MetaKing.BackendServer.Data
{
    public class ProductDetailPhoneModel : IDateTracking
    {
        [Key]
        public int Id { get; set; }
        public string? ScreenSize { get; set; } // Kích thước màn hình
        public string? ScreenTechnology { get; set; } // Công nghệ màn hình
        public string? RearCamera { get; set; } // Camera sau
        public string? FrontCamera { get; set; } // Camera sau
        public string? ChipSet { get; set; } // Chipset
        public bool NFC { get; set; } // NFC
        public string? RAMCapacity { get; set; } // Phiên Bản RAM
        public string? InternalStorage { get; set; } // Bộ nhớ trong
        public string? SimCard { get; set; } // Thẻ sim
        public string? OperatingSystem { get; set; } // Hệ điều hành
        public string? DisplayResolution { get; set; } // Độ phân giải màn hình
        public string? DisplayFeatures { get; set; } // Tính năng màn hình
        public string? CPUType { get; set; } // Loại CPU
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
