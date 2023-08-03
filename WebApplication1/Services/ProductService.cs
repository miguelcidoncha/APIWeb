using Data;
using Entities.Entities;
using WebApplication1.IServices;

namespace WebApplication1.Services
{
    public class ProductService : BaseContextService, IProductService
    {
        public ProductService(ServiceContext serviceContext) : base(serviceContext) 
        {
        }

        public int InsertProduct(ProductItem productItem)
        {
            _serviceContext.Products.Add(productItem);
            _serviceContext.SaveChanges();
            return productItem.ProductId;
        }
        public List<ProductItem> GetProductsInOrder(int orderId)
        {
            var productsInOrder = _serviceContext.OrderProduct
                .Where(op => op.OrderId == orderId)
                .Select(op => op.Product)
                .ToList();

            return productsInOrder;
        }
}
}
