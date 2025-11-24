using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaKing.Orders
{
    public class OrderHistoryDto
    {
        public Guid OrderId { get; set; }
        public string Code { get; set; }
        public DateTime CreationTime { get; set; }
        public double Total { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}
