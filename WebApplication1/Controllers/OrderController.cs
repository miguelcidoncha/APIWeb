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
        public IActionResult CreateOrder(ushort productId, [FromBody] OrderItem orderItem, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                                    .Where(u => u.NombreUsuario == userNombreUsuario
                                        && u.Contraseña == userContraseña)
                                    .FirstOrDefault();

            if (seletedUser != null)
            {
                if (orderItem != null)
                {
                    // Comprobar si el pedido ya tiene un IdOrder, y si lo tiene, comprobar que este pedido no existe en la base de datos.
                    if (orderItem.IdOrder != 0)
                    {
                        var existingOrder = _serviceContext.Orders.FirstOrDefault(o => o.IdOrder == orderItem.IdOrder);
                        if (existingOrder != null)
                        {
                            return BadRequest("Order with the specified IdOrder already exists.");
                        }
                    }

                    // Añadir una orden al contexto de datos
                    _serviceContext.Orders.Add(orderItem);

                    // Registro de la acción de crear una orden
                    _serviceContext.AuditLogs.Add(new AuditLog
                    {
                        Action = "Insert",
                        TableName = "Orders",
                        RecordId = orderItem.IdOrder,
                        Timestamp = DateTime.Now,
                        UserId = seletedUser.IdUsuario
                    });

                    // Guardar los cambios en la base de datos
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


        //recuperación de pedidos de la tabla Ordens por Id
        [HttpGet("Order/Get", Name = "GetOrder")]
        public IActionResult Get(ushort orderId)
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
        public IActionResult UpdateOrder(ushort orderId, [FromBody] OrderItem updatedOrder, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña)
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
                    // Registro de la acción de actualización del pedido
                    _serviceContext.AuditLogs.Add(new AuditLog
                    {
                        Action = "Update",
                        TableName = "Orders",
                        RecordId = orderId,
                        Timestamp = DateTime.Now,
                        UserId = seletedUser.IdUsuario // Añadir información de UserId a AuditLog
                    });
                    // Actualización de los valores de los campos del pedido utilizando datos del updatedOrder
                    order.CustomerName = updatedOrder.CustomerName;
                    order.ProductStock = updatedOrder.ProductStock;

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
        public IActionResult Delete(ushort orderId, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                                   .Where(u => u.NombreUsuario == userNombreUsuario
                                        && u.Contraseña == userContraseña
                                        && (u.IdRol == 1 || u.IdRol == 2))
                                    .FirstOrDefault();

            if (seletedUser != null)
            {
                // Registro de la acción de supresión de pedidos
                _serviceContext.AuditLogs.Add(new AuditLog
                {
                    Action = "Delete",
                    TableName = "Orders",
                    RecordId = orderId,
                    Timestamp = DateTime.Now,
                    UserId = seletedUser.IdUsuario // Añadir información de UserId a AuditLog
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

