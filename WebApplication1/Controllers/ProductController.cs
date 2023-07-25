using Data;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web.Http.Cors;
using WebApplication1.IServices;

namespace WebApplication1.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Route("[controller]/[action]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ServiceContext _serviceContext;
        

        public ProductController(IProductService productService, ServiceContext serviceContext)
        {
            _productService = productService;
            _serviceContext = serviceContext;
        }

        [HttpPost(Name = "InsertProduct")]
        public int Post([FromBody] ProductItem productItem)
        {
            return _productService.insertProduct(productItem);
        }

        [HttpGet("{productId}", Name = "GetProduct")]
        public IActionResult Get(int productId)
        {
            var product = _serviceContext.Products.Include(p => p.Orders).FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                return Ok(product);
            }
            else
            {
                return NotFound("Продукт с указанным идентификатором не найден.");
            }
        }

        [HttpDelete("{productId}", Name = "DeleteProduct")]
        public IActionResult Delete(int productId)
        {
            var product = _serviceContext.Products.Find(productId);
            if (product != null)
            {
                    // Вызываем метод для удаления продукта по идентификатору
                    bool isDeleted = _serviceContext.RemoveProductById(productId);

                if (isDeleted)
                {
                    return Ok("El producto se ha eliminado correctamente.");
                }
                else
                {
                    return BadRequest("Ошибка при удалении продукта.");
                }
            }
            else
            {
                return NotFound("No se ha encontrado el producto con el identificador especificado.");
            }
        }
    }

    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ServiceContext _serviceContext;
        // Новый метод для создания заказов
        public OrderController(IOrderService orderService, ServiceContext serviceContext)
        {
            _orderService = orderService;
            _serviceContext = serviceContext;
        }

        [HttpPost(Name = "InsertOrder")]
        public int Post([FromBody] OrderItem orderItem)
        {
            return _orderService.insertOrder(orderItem);
        }

        [HttpPost("{productId}")]
        public IActionResult CreateOrder(int productId, [FromBody] OrderItem orderItem)
        {
            var product = _serviceContext.Orders.FirstOrDefault(p => p.Id == productId);

            if (product != null)
            {
                if (orderItem == null)
                {
                    orderItem = new OrderItem(); // Создаем новый экземпляр OrderItem, если его нет в теле запроса
                }
                // Связываем заказ с продуктом по идентификатору продукта
                orderItem.ProductId = productId; // Просто устанавливаем ProductId, не создавая новый экземпляр OrderItem
                _serviceContext.Orders.Add(orderItem);
                _serviceContext.SaveChanges();

                return Ok("Заказ успешно создан.");
            }
            else
            {
                return NotFound("Заказ с указанным идентификатором не найден.");
            }
        }
    }
}
