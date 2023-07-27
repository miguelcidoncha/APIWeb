using Data;
using Entities;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.IServices;

namespace WebApplication1.Controllers
{
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ServiceContext _serviceContext;

        public OrderController(IOrderService orderService, ServiceContext serviceContext)
        {
            _orderService = orderService;
            _serviceContext = serviceContext;
        }


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