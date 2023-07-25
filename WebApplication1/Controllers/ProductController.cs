using Data;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web.Http.Cors;
using WebApplication1.IServices;
using WebApplication1.Services;

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
        //добавление продукта
        [HttpPost(Name = "InsertProduct")]
        public int Post([FromBody] ProductItem productItem)
        {
            return _productService.insertProduct(productItem);
        }
        //получение продукта из таблицы Products по Id
        [HttpGet("productId", Name = "GetProduct")]
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
            //удаление продукта из таблицы Products по Id
        [HttpDelete("productId", Name = "DeleteProduct")]
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
                    return BadRequest("Error Ошибка при удалении продукта.");
                }
            }
            else
            {
                return NotFound("No se ha encontrado el PRODUCTO con el identificador especificado.");
            }
        }
    }

    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ServiceContext _serviceContext;
        //метод для создания заказов
        public OrderController(IOrderService orderService, ServiceContext serviceContext)
        {
            _orderService = orderService;
            _serviceContext = serviceContext;
        }

        //добавление заказа
        //[HttpPost("InsertOrder", Name = "InsertOrder")]
        //public int Post([FromBody] OrderItem orderItem)
        //{
        //    return _orderService.insertOrder(orderItem);
        //}

        //получение заказа из таблицы Ordens по Id
        [HttpGet("OrdensId", Name = "GetOrder")]
        public IActionResult Get(int orderId)
        {
            var order = _serviceContext.Orders.FirstOrDefault(p => p.Id == orderId);
            if (order != null)
            {
                return Ok(order);
            }
            else
            {
                return NotFound("Заказ с указанным идентификатором не найден.");
            }
        }

        //удаление заказа из таблицы Orders по Id
        [HttpDelete("OrdersId", Name = "DeleteOrder")]
        public IActionResult Delete(int orderId)
        {
            var order = _serviceContext.Orders.Find(orderId);
            if (order != null)
            {
                // Вызываем метод для удаления продукта по идентификатору
                bool isDeleted = _serviceContext.RemoveOrderById(orderId);

                if (isDeleted)
                {
                    return Ok("El order se ha eliminado correctamente.");
                }
                else
                {
                    return BadRequest("Error Ошибка при удалении ордера.");
                }
            }
            else
            {
                return NotFound("No se ha encontrado el !order! con el identificador especificado.");
            }
        }

        [HttpPost("productId", Name = "InsertOrder")]
        public IActionResult CreateOrder(int productId, [FromBody] OrderItem orderItem)
        {
            //var product = _serviceContext.Orders.FirstOrDefault(o => o.ProductId == productId);

            //if (product == null)
            //{
                if (orderItem != null)
                {
                   var newOrderItem = new OrderItem(); // Создаем новый экземпляр OrderItem, если его нет в теле запроса
                    newOrderItem.ProductId = productId;
                    newOrderItem.CustomerName = orderItem.CustomerName;
                // Просто устанавливаем ProductId, не создавая новый экземпляр OrderItem
                     _serviceContext.Orders.Add(newOrderItem);
                    _serviceContext.SaveChanges();
                return Ok("El order se ha creado correctamente.");
                }
                // Связываем заказ с продуктом по идентификатору продукта
               

                
            //}
            else
            {
                return NotFound("No se ha encontrado el order con el identificador especificado.");
            }
}
    }
}
