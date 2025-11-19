using MetaKing.Orders;
using MetaKing.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;

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
                    Quantity = item.Quantity,
                    SKU = product.SKU
                });
            }

            await _orderItemRepository.InsertManyAsync(items, autoSave: true);

            // 3. Map lại
            return ObjectMapper.Map<Order, OrderDto>(order);
        }
    }
}
