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
        //Añadir un product
        [HttpPost(Name = "InsertProduct")]
        //Unidad de verificación de los derechos de acceso
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
        //Pedir un producto de la tabla Products por su Id
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

        // modificar registros de la tabla "Products"
        // модифицировать записи в таблице "Products"
        [HttpPut(Name = "UpdateProduct")]
        public IActionResult UpdateProduct(int productId, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña, [FromBody] ProductItem updatedProduct)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                                   .Where(u => u.NombreUsuario == userNombreUsuario
                                        && u.Contraseña == userContraseña
                                        && u.Rol == 1)
                                    .FirstOrDefault();

            if (seletedUser != null)
            {
                var product = _serviceContext.Products.FirstOrDefault(p => p.Id == productId);

                if (product != null)
                {
                    // Обновляем значения полей продукта с помощью данных из updatedProduct
                    product.ProductName = updatedProduct.ProductName;
                    product.Quantity = updatedProduct.Quantity;
                    product.Manufacturer = updatedProduct.Manufacturer;

                    _serviceContext.SaveChanges();

                    return Ok("El producto se ha actualizado correctamente.");
                }
                else
                {
                    return NotFound("No se ha encontrado el producto con el identificador especificado.");
                }
            }
            else
            {
                return Unauthorized("El usuario no está autorizado o no existe");
            }
        }


        //eliminar un producto de la tabla Products по Id
        // Удалить запись из таблицы "Products"
        [HttpDelete("{productId}", Name = "DeleteProduct")]
        public IActionResult Delete(int productId, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                                   .Where(u => u.NombreUsuario == userNombreUsuario
                                        && u.Contraseña == userContraseña
                                        && u.Rol == 1)
                                    .FirstOrDefault();

            if (seletedUser != null)
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
                        return BadRequest("Error al eliminar un producto.");
                    }
                }
                else
                {
                    return NotFound("No se ha encontrado el producto con el identificador especificado.");
                }
            }
            else
            {
                return Unauthorized("El usuario no está autorizado o no existe");
            }
        }

    }

    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ServiceContext _serviceContext;

        public OrderController(IOrderService orderService, ServiceContext serviceContext)
        {
            _orderService = orderService;
            _serviceContext = serviceContext;
        }

        // Añadir pedidos
        [HttpPost("Order/Post", Name = "InsertOrder")]
        public IActionResult CreateOrder(int productId, [FromBody] OrderItem orderItem, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                                   .Where(u => u.NombreUsuario == userNombreUsuario
                                        && u.Contraseña == userContraseña
                                        && u.Rol == 1)
                                    .FirstOrDefault();

            if (seletedUser != null)
            {
                if (orderItem != null)
                {
                    var newOrderItem = new OrderItem();
                    newOrderItem.ProductId = productId;
                    newOrderItem.CustomerName = orderItem.CustomerName;
                    // Просто устанавливаем ProductId, не создавая новый экземпляр OrderItem
                    _serviceContext.Orders.Add(newOrderItem);
                    _serviceContext.SaveChanges();

                    return Ok("El pedido se ha creado correctamente.");
                }
                else
                {
                    return NotFound("No se ha encontrado el pedido con el identificador especificado.");
                }
            }
            else
            {
                return Unauthorized("El usuario no está autorizado o no existe");
            }
        }

        //recuperación de pedidos de la tabla Ordens по Id
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
                return NotFound("No se ha encontrado el pedido con el identificador especificado.");
            }
        }
        ////modificar registros de la tabla Orders
        //[HttpPut("Order/UpdateOrder", Name = "UpdateOrder")]
        //public IActionResult UpdateOrder(int orderId, [FromBody] OrderItem updatedOrder)
        //{
        //    var order = _serviceContext.Orders.FirstOrDefault(o => o.Id == orderId);

        //    if (order != null)
        //    {
        //        // Actualización de los valores de los campos del pedido utilizando datos del updatedOrder
        //        order.CustomerName = updatedOrder.CustomerName;
        //        order.Quantity = updatedOrder.Quantity;

        //        _serviceContext.SaveChanges();

        //        return Ok("El pedido se ha actualizado correctamente.");
        //    }
        //    else
        //    {
        //        return NotFound("No se ha encontrado el pedido con el identificador especificado.");
        //    }
        //}
        // Modificar registros de la tabla Orders
        [HttpPut("Order/UpdateOrder", Name = "UpdateOrder")]
        public IActionResult UpdateOrder(int orderId, [FromBody] OrderItem updatedOrder, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                                   .Where(u => u.NombreUsuario == userNombreUsuario
                                        && u.Contraseña == userContraseña
                                        && u.Rol == 1)
                                    .FirstOrDefault();

            if (seletedUser != null)
            {
                var order = _serviceContext.Orders.FirstOrDefault(o => o.Id == orderId);

                if (order != null)
                {
                    // Actualización de los valores de los campos del pedido utilizando datos del updatedOrder
                    order.CustomerName = updatedOrder.CustomerName;
                    order.Quantity = updatedOrder.Quantity;

                    _serviceContext.SaveChanges();

                    return Ok("El pedido se ha actualizado correctamente.");
                }
                else
                {
                    return NotFound("No se ha encontrado el pedido con el identificador especificado.");
                }
            }
            else
            {
                return Unauthorized("El usuario no está autorizado o no existe");
            }
        }


        //eliminar una orden de la tabla Orders по Id
        //[HttpDelete("Order/Delete/OrderId", Name = "DeleteOrder")]
        //public IActionResult Delete(int orderId)
        //{
        //    var order = _serviceContext.Orders.Find(orderId);
        //    if (order != null)
        //    {
        //        // Llamar al método para eliminar un producto por identificador
        //        bool isDeleted = _serviceContext.RemoveOrderById(orderId);

        //        if (isDeleted)
        //        {
        //            return Ok("El order se ha eliminado correctamente.");
        //        }
        //        else
        //        {
        //            return BadRequest("Error al eliminar un pedido.");
        //        }
        //    }
        //    else
        //    {
        //        return NotFound("No se ha encontrado el !order! con el identificador especificado.");
        //    }
        //}
        // Eliminar registros de la tabla Orders
        [HttpDelete("Order/Delete/{orderId}", Name = "DeleteOrder")]
        public IActionResult Delete(int orderId, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                                   .Where(u => u.NombreUsuario == userNombreUsuario
                                        && u.Contraseña == userContraseña
                                        && u.Rol == 1)
                                    .FirstOrDefault();

            if (seletedUser != null)
            {
                var order = _serviceContext.Orders.Find(orderId);

                if (order != null)
                {
                    // Llamar al método para eliminar un pedido por identificador
                    bool isDeleted = _serviceContext.RemoveOrderById(orderId);

                    if (isDeleted)
                    {
                        return Ok("El pedido se ha eliminado correctamente.");
                    }
                    else
                    {
                        return BadRequest("Error al eliminar el pedido.");
                    }
                }
                else
                {
                    return NotFound("No se ha encontrado el pedido con el identificador especificado.");
                }
            }
            else
            {
                return Unauthorized("El usuario no está autorizado o no existe");
            }
        }

    }
}
