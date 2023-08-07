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

        public int InsertDetal(OrderDetal orderDetal)
        {
            _serviceContext.OrderDetal.Add(orderDetal);
            _serviceContext.SaveChanges();
            return orderDetal.Id;
        }
    }
}
