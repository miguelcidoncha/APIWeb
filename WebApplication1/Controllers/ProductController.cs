using Data;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
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
        public int Post([FromQuery] string userNombreUsuario, [FromQuery] string userContraseña, [FromBody] ProductItem productItem)
        {

            var seletedUser = _serviceContext.Set<UserItem>()
                               .Where(u => u.NombreUsuario == userNombreUsuario
                                    && u.Contraseña == userContraseña
                                    && u.Rol == 1)
                                .FirstOrDefault();

            if (seletedUser != null)
            {
                return _productService.insertProduct(productItem);
            }
            else
            {
                throw new InvalidCredentialException("El ususario no esta autorizado o no existe");
            }

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
                return NotFound("No se ha encontrado el producto con el identificador especificado.");
            }
        }

        // Метод для изменения записий из таблицы "Products" по идентификатору
        [HttpPut(Name = "UpdateProduct")]
        public IActionResult UpdateProduct(int productId, [FromBody] ProductItem updatedProduct)
        {
            var product = _serviceContext.Products.FirstOrDefault(p => p.Id == productId);

            if (product != null)
            {
                // Обновляем значения полей продукта с помощью данных из updatedProduct
                product.ProductName = updatedProduct.ProductName;
                product.Quantity = updatedProduct.Quantity;
                product.Manufacturer = updatedProduct.Manufacturer;

                // Сохраняем изменения в базе данных
                _serviceContext.SaveChanges();

                return Ok("El producto se ha actualizado correctamente.");
            }
            else
            {
                return NotFound("No se ha encontrado el producto con el identificador especificado.");
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

        //добавление заказа
        [HttpPost("Order/Post", Name = "InsertOrder")]
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
            else
            {
                return NotFound("No se ha encontrado el order con el identificador especificado.");
            }
        }

        //получение заказа из таблицы Ordens по Id
        [HttpGet("Order/Get", Name = "GetOrder")]
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

        [HttpPut("Order/UpdateOrder", Name = "UpdateOrder")]
        public IActionResult UpdateOrder(int orderId, [FromBody] OrderItem updatedOrder)
        {
            var order = _serviceContext.Orders.FirstOrDefault(o => o.Id == orderId);

            if (order != null)
            {
                // Обновляем значения полей заказа с помощью данных из updatedOrder
                order.CustomerName = updatedOrder.CustomerName;
                order.Quantity = updatedOrder.Quantity;

                // Сохраняем изменения в базе данных
                _serviceContext.SaveChanges();

                return Ok("El pedido se ha actualizado correctamente.");
            }
            else
            {
                return NotFound("No se ha encontrado el pedido con el identificador especificado.");
            }
        }


        //удаление заказа из таблицы Orders по Id
        [HttpDelete("Order/Delete/OrderId", Name = "DeleteOrder")]
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

    }
}
