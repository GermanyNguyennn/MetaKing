using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MetaKing.BackendServer.Data.Entities;
using MetaKing.BackendServer.Data.Interfaces;
using System.Text.Json.Serialization;

namespace MetaKing.BackendServer.Data
{
    public enum OrderType
    {
        Ordered = 0,
        Delivered = 1,
        Received = 2
    }
    public class OrderModel : IDateTracking
    {
        [Key]
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int? CouponId { get; set; }
        public string CouponCode { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public OrderType StatusOrderType { get; set; }
        public string PaymentMethod { get; set; }

        [ForeignKey("UserId")]
        [JsonIgnore]
        public UserModel? User { get; set; }

        [ForeignKey("CouponId")]
        [JsonIgnore]
        public CouponModel? Coupon { get; set; }
        public ICollection<OrderDetailModel> OrderDetails { get; set; } = new List<OrderDetailModel>();
    }
}
