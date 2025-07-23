using MetaKing.BackendServer.Data.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MetaKing.BackendServer.Data
{
    public class VNPayModel : IDateTracking
    {
        [Key]
        public int Id { get; set; }
        public string OrderId { get; set; }
        public string OrderInfo { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
