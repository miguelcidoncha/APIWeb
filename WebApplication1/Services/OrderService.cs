using Data;
using Entities.Entities;
using WebApplication1.IServices;

namespace WebApplication1.Services
{
    public class OrderService : BaseContextService, IOrderService
    {
        public OrderService(ServiceContext serviceContext) : base(serviceContext) 
        {
        }

        public int InsertOrder(OrderItem orderItem)
        {
            _serviceContext.Orders.Add(orderItem);
            _serviceContext.SaveChanges();
            return orderItem.OrderId;
        }

        public int InsertOrderProduct(OrderProduct orderProduct)
        {
            _serviceContext.OrderProduct.Add(orderProduct);
            _serviceContext.SaveChanges();
            return orderProduct.Id;
        }
        public OrderItem GetOrderById(int orderId)
        {
            return _serviceContext.Orders.FirstOrDefault(o => o.OrderId == orderId);
        }
    }
}
