using Data;
using Entities;
using WebApplication1.IServices;

namespace WebApplication1.Services
{
    public class OrderService : BaseContextService, IOrderService
    {
        public OrderService(ServiceContext serviceContext) : base(serviceContext) 
        {
        }

        public int insertOrder(OrderItem orderItem)
        {
            _serviceContext.Orders.Add(orderItem);
            _serviceContext.SaveChanges();
            return orderItem.Id;
        }
    }
}
