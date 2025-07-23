using MetaKing.BackendServer.Data.Entities;
using MetaKing.BackendServer.Data.Interfaces;
using MetaKing.ViewModels.Enum;
using System.ComponentModel.DataAnnotations;

namespace MetaKing.BackendServer.Data
{
    public class CouponModel : IDateTracking
    {
        [Key]
        public int Id { get; set; }
        public string CouponCode { get; set; }
        public string Description { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Quantity { get; set; }
        public StatusType Status { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
