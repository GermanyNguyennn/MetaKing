using MetaKing.BackendServer.Data.Entities;
using MetaKing.BackendServer.Data.Interfaces;
using MetaKing.ViewModels.Enum;
using System.ComponentModel.DataAnnotations;

namespace MetaKing.BackendServer.Data
{
    public class BrandModel : IDateTracking
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public StatusType Status { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public ICollection<ProductModel> Products { get; set; } = new List<ProductModel>();
    }
}
