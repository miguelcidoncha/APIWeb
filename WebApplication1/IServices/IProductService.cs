using Entities.Entities;

namespace WebApplication1.IServices
{
    public interface IProductService
    {
        int InsertProduct(ProductItem productItem);
        //List<ProductItem> GetProductsInOrder(int orderId);
    }
}
