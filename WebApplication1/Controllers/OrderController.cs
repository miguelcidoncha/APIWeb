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
                                        && (u.IdRol == 1 || u.IdRol == 2))

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

                    // Журналирование действия создания заказа
                    _serviceContext.AuditLogs.Add(new AuditLog
                    {
                        Action = "Insert",
                        TableName = "Orders",
                        RecordId = newOrderItem.IdOrder, // Здесь уже есть значение IdOrder из базы данных
                        Timestamp = DateTime.Now,
                        UserId = seletedUser.IdUsuario
                    });

                    _serviceContext.SaveChanges(); // Сохраняем изменения в базу данных

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


        //recuperación de pedidos de la tabla Ordens por Id
        [HttpGet("Order/Get", Name = "GetOrder")]
        public IActionResult Get(int orderId)
        {
            var order = _serviceContext.Orders.FirstOrDefault(p => p.IdOrder == orderId);
            if (order != null)
            {
                return Ok(order);
            }
            else
            {
                return NotFound("No se ha encontrado el pedido con el identificador especificado.");
            }
        }

        // Modificar registros de la tabla Orders
        [HttpPut("Order/UpdateOrder", Name = "UpdateOrder")]
        public IActionResult UpdateOrder(int orderId, [FromBody] OrderItem updatedOrder, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                                   .Where(u => u.NombreUsuario == userNombreUsuario
                                        && u.Contraseña == userContraseña
                                        && (u.IdRol == 1 || u.IdRol == 2))
                                    .FirstOrDefault();

            if (seletedUser != null)
            {
                var order = _serviceContext.Orders.FirstOrDefault(o => o.IdOrder == orderId);

                if (order != null)
                {
                    // Журналирование действия обновления заказа
                    _serviceContext.AuditLogs.Add(new AuditLog
                    {
                        Action = "Update",
                        TableName = "Orders",
                        RecordId = orderId,
                        Timestamp = DateTime.Now,
                        UserId = seletedUser.IdUsuario // Добавляем информацию о UserId в AuditLog
                    });
                    // Actualización de los valores de los campos del pedido utilizando datos del updatedOrder
                    order.CustomerName = updatedOrder.CustomerName;
                    order.Productstock = updatedOrder.Productstock;

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
        [HttpDelete("Order/Delete/{orderId}", Name = "DeleteOrder")]
        public IActionResult Delete(int orderId, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                                   .Where(u => u.NombreUsuario == userNombreUsuario
                                        && u.Contraseña == userContraseña
                                        && (u.IdRol == 1 || u.IdRol == 2))
                                    .FirstOrDefault();

            if (seletedUser != null)
            {
                // Журналирование действия удаления заказа
                _serviceContext.AuditLogs.Add(new AuditLog
                {
                    Action = "Delete",
                    TableName = "Orders",
                    RecordId = orderId,
                    Timestamp = DateTime.Now,
                    UserId = seletedUser.IdUsuario // Добавляем информацию о UserId в AuditLog
                });
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

