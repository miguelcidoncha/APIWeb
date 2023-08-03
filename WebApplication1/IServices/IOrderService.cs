using Entities.Entities;

namespace WebApplication1.IServices
{
    public interface IOrderService
    {
        int InsertOrder(OrderItem orderItem);
        int InsertOrderProduct(OrderProduct orderProduct);
        OrderItem GetOrderById(int orderId);
    }
}
