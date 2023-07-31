using Entities;

namespace WebApplication1.IServices
{
    public interface IOrderService
    {
        int insertOrder(OrderItem orderItem);
    }
}
