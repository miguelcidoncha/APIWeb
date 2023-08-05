using Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Web.Http.Cors;
using WebApplication1.IServices;
using WebApplication1.Services;
using Entities.Entities;

namespace WebApplication1.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Route("[controller]/[action]")]

    public class OrderController : ControllerBase
    {
        private readonly ServiceContext _serviceContext;
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService, ServiceContext serviceContext)
        {
            _serviceContext = serviceContext;
            _orderService = orderService;
        }

        // Añadir pedidos
        [HttpPost(Name = "InsertOrder")]
        public IActionResult CreateOrder(int productId, [FromBody] OrderItem orderItem, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña)
        {

                if (orderItem != null)
                {
                    //// Проверяем, есть ли у заказа уже IdOrder, и если есть, проверяем, что такой заказ не существует в базе данных
                    //if (orderItem.OrderId != 0)
                    //{
                    //    var existingOrder = _serviceContext.OrderItems.FirstOrDefault(o => o.OrderId == orderItem.OrderId);
                    //    if (existingOrder != null)
                    //    {
                    //        return BadRequest("Order with the specified IdOrder already exists.");
                    //    }
                    //}

                    // Добавляем заказ в контекст данных
                    _serviceContext.Orders.Add(orderItem);

                    // Журналирование действия создания заказа
                    //_serviceContext.AuditLogs.Add(new AuditLog
                    //{
                    //    Action = "Insert",
                    //    TableName = "Orders",
                    //    RecordId = orderItem.OrderId,
                    //    Timestamp = DateTime.Now,
                    //    UserId = seletedUser.IdUsuario
                    //});

                    // Сохраняем изменения в базе данных
                    _serviceContext.SaveChanges();

                // Создаем запись в таблице OrderProduct для установления связей между заказом, продуктом и пользователем
                    _serviceContext.OrderProducts.Add(new OrderProduct
                    {
                        OrderId = orderItem.OrderId,
                        ProductId = productId,
                        //UserId = seletedUser.UsuarioId // Вам нужно получить соответствующего пользователя из базы данных
                    });

                // Сохраняем изменения в базе данных
                _serviceContext.SaveChanges();

                return Ok("El pedido se ha creado correctamente.");
                }
                else
                {
                    return NotFound("No se ha encontrado el pedido con el identificador especificado.");
                }
        }


        //recuperación de pedidos de la tabla Ordens por Id

        //[HttpPost("Order/Post", Name = "InsertOrder")]







        //[HttpGet("Order/Get", Name = "GetOrder")]
        //public IActionResult Get(ushort orderId)
        //{
        //    var order = _serviceContext.Orders.FirstOrDefault(p => p.OrderId == orderId);
        //    if (order != null)
        //    {
        //        return Ok(order);
        //    }
        //    else
        //    {
        //        return NotFound("No se ha encontrado el pedido con el identificador especificado.");
        //    }
        //}

        //список всех продуктов в заказе по его Id
        //[HttpGet("GetProductsInOrder/orderId", Name = "GetProductsInOrder")]
        //public IActionResult GetProductsInOrder(ushort orderId)
        //{
        //    try
        //    {
        //        var order = _serviceContext.Orders
        //            .Include(o => o.OrderProduct)
        //                .ThenInclude(op => op.Product)
        //            .FirstOrDefault(o => o.IdOrder == orderId);

        //        if (order != null)
        //        {
        //            var productsInOrder = order.OrderProduct.Select(op => op.Product).ToList();
        //            return Ok(productsInOrder);
        //        }
        //        else
        //        {
        //            return NotFound("No se ha encontrado el pedido con el identificador especificado.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Error al obtener los productos del pedido: " + ex.Message);
        //    }
        //}


        //[HttpGet("{userId}/{orderId}", Name = "GetOrderWithProducts")]
        //public IActionResult GetOrderWithProducts(int userId, int orderId)
        //{
        //    try
        //    {
        //        // Проверяем, существует ли пользователь с указанным идентификатором
        //        var user = _serviceContext.Users.FirstOrDefault(u => u.IdUsuario == userId);
        //        if (user == null)
        //        {
        //            return NotFound("Usuario no encontrado.");
        //        }

        //        // Получаем информацию о заказе по его идентификатору
        //        var order = _orderService.GetOrderById(orderId);
        //        if (order == null)
        //        {
        //            return NotFound("Pedido no encontrado.");
        //        }

        //        // Получаем список продуктов в заказе по его идентификатору
        //        var productsInOrder = _productService.GetProductsInOrder(orderId);

        //        // Создаем объект, который содержит информацию о заказе и список продуктов в нем
        //        var orderWithProducts = new
        //        {
        //            Order = order,
        //            Products = productsInOrder
        //        };

        //        return Ok(orderWithProducts);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Error al obtener el pedido: " + ex.Message);
        //    }
        //}


        // Modificar registros de la tabla Orders



        //eliminar una orden de la tabla Orders по Id
        //[HttpDelete("Order/Delete/{orderId}", Name = "DeleteOrder")]
        //public IActionResult Delete(ushort orderId, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña)
        //{
        //    var seletedUser = _serviceContext.Set<UserItem>()
        //                           .Where(u => u.NombreUsuario == userNombreUsuario
        //                                && u.Contraseña == userContraseña)
        //                           .FirstOrDefault();

        //    if (seletedUser != null)
        //    {
        //        // Журналирование действия удаления заказа
        //        _serviceContext.AuditLogs.Add(new AuditLog
        //        {
        //            Action = "Delete",
        //            TableName = "Orders",
        //            RecordId = orderId,
        //            Timestamp = DateTime.Now,
        //            UserId = seletedUser.IdUsuario // Добавляем информацию о UserId в AuditLog
        //        });
        //        var order = _serviceContext.Orders.Find(orderId);

        //        if (order != null)
        //        {
        //            // Llamar al método para eliminar un pedido por identificador
        //            bool isDeleted = _serviceContext.RemoveOrderById(orderId);

        //            if (isDeleted)
        //            {
        //                return Ok("El pedido se ha eliminado correctamente.");
        //            }
        //            else
        //            {
        //                return BadRequest("Error al eliminar el pedido.");
        //            }
        //        }
        //        else
        //        {
        //            return NotFound("No se ha encontrado el pedido con el identificador especificado.");
        //        }
        //    }
        //    else
        //    {
        //        return Unauthorized("El usuario no está autorizado o no existe");
        //    }
        //}
    }
}

