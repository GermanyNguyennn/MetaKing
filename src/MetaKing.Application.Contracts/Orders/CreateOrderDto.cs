using MetaKing.Orders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace MetaKing.Orders
{
    public class CreateOrderDto
    {
        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerAddress { get; set; }
        public string Commune { get; set; }
        public string Province { get; set; }
        public Guid? CustomerUserId { get; set; }
        [BindNever]
        public List<OrderItemDto>? Items { get; set; }
    }
}
