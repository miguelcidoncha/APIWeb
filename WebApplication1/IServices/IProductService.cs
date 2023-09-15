using Entities.Entities;

namespace WebApiEcommerce.IServices
{
    public interface IProductService
    {
        int InsertProduct(ProductItem productItem);
        void DeleteProduct(int id);
        void UpdateProduct(ProductItem productItem);
        List<ProductItem> GetAllProducts();
        List<ProductItem> GetProductById(int CourseId);
    }
}
