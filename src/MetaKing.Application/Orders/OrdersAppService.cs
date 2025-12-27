using MetaKing.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace MetaKing.Orders
{
    public class OrdersAppService : CrudAppService<
        Order,
        OrderDto,
        Guid,
        PagedResultRequestDto, CreateOrderDto, CreateOrderDto>, IOrdersAppService
    {
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly OrderCodeGenerator _orderCodeGenerator;
        private readonly IRepository<Product, Guid> _productRepository;
        public OrdersAppService(IRepository<Order, Guid> repository, 
            OrderCodeGenerator orderCodeGenerator,
            IRepository<OrderItem> orderItemRepository,
            IRepository<Product, Guid> productRepository)
            : base(repository)
        {
            _orderItemRepository = orderItemRepository;
            _orderCodeGenerator = orderCodeGenerator;
            _productRepository = productRepository;
        }

        public override async Task<OrderDto> CreateAsync(CreateOrderDto input)
        {
            var subTotal = input.Items.Sum(x => x.Quantity * x.Price);
            var orderId = Guid.NewGuid();

            var order = new Order(orderId)
            {   
                Code = await _orderCodeGenerator.GenerateAsync(),
                CustomerAddress = input.CustomerAddress,
                CustomerName = input.CustomerName,
                CustomerPhoneNumber = input.CustomerPhoneNumber,
                Commune = input.Commune,
                Province = input.Province,
                ShippingFee = 50000,
                CustomerUserId = input.CustomerUserId,
                Tax = 0,
                Subtotal = subTotal,
                GrandTotal = subTotal,
                Discount = 0,
                PaymentMethod = PaymentMethod.COD,
                Total = subTotal,
                Status = OrderStatus.New
            };

            await Repository.InsertAsync(order, autoSave: true);

            var items = new List<OrderItem>();
            foreach (var item in input.Items)
            {
                var product = await _productRepository.GetAsync(item.ProductId);

                items.Add(new OrderItem()
                {
                    OrderId = orderId,
                    Price = item.Price,
                    ProductId = item.ProductId,
                    ProductName = product.Name,
                    ProductCode = product.Code,
                    Quantity = item.Quantity
                });
            }

            await _orderItemRepository.InsertManyAsync(items, autoSave: true);

            return ObjectMapper.Map<Order, OrderDto>(order);
        }

        public async Task<List<OrderHistoryDto>> GetOrderHistoryAsync(Guid customerUserId)
        {
            // Lấy toàn bộ đơn của user
            var orders = await Repository.GetListAsync(x => x.CustomerUserId == customerUserId);

            // Sắp xếp theo thời gian
            orders = orders.OrderByDescending(x => x.CreationTime).ToList();

            // Lấy danh sách Id đơn hàng
            var orderIds = orders.Select(x => x.Id).ToList();

            // Lấy tất cả OrderItem
            var items = await _orderItemRepository.GetListAsync(x => orderIds.Contains(x.OrderId));

            // Ghép dữ liệu
            var result = new List<OrderHistoryDto>();
            foreach (var order in orders)
            {
                var orderItems = items
                    .Where(i => i.OrderId == order.Id)
                    .Select(i => new OrderItemDto
                    {
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        ProductCode = i.ProductCode,
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList();

                result.Add(new OrderHistoryDto
                {
                    OrderId = order.Id,
                    Code = order.Code,
                    CreationTime = order.CreationTime,
                    Total = order.Total,
                    Status = order.Status,
                    PaymentMethod = order.PaymentMethod,
                    CustomerName = order.CustomerName,
                    CustomerAddress = order.CustomerAddress,
                    CustomerPhoneNumber = order.CustomerPhoneNumber,
                    Items = orderItems
                });
            }

            return result;
        }
    }
}
