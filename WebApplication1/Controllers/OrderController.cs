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
        //private readonly IProductService _productService;

        public OrderController(IOrderService orderService, IProductService productService, ServiceContext serviceContext)
        {
            _serviceContext = serviceContext;
            _orderService = orderService;
            //_productService = productService;

        }

        // Añadir pedidos
        [HttpPost(Name = "InsertOrder")]
        public IActionResult CreateOrder([FromBody] OrderItem orderItem, [FromBody] OrderDetal orderDetal, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña)
        {
            var selectedUser = _serviceContext.Set<UserItem>()
            .Where(u => u.NombreUsuario == userNombreUsuario && u.Contraseña == userContraseña 
            && u.RolId == 1).FirstOrDefault();

            if (selectedUser != null)
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
                int orderId = _orderService.InsertOrder(orderItem);
                //_serviceContext.Orders.Add(orderItem);


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
                    //_serviceContext.SaveChanges(); это уже есть в сервисе, поэтому закоментарено

                    // Создаем запись в таблице OrderProduct для установления связей между заказом, продуктом и пользователем

                    int detalId = _orderService.InsertDetal(orderDetal);

                    //_serviceContext.OrderDetal.Add(new OrderDetal
                    //{
                    //    OrderId = orderItem.OrderId,
                    //    //ProductId = productItem.ProductId,
                    //    UsuarioId = selectedUser.UsuarioId,
                    //    //ProductStock =
                    //    //TotalPrice =
                    //}); 

                // Сохраняем изменения в базе данных
                //_serviceContext.SaveChanges();

                return Ok(orderId, detalId);
                }
                else
                {
                    return NotFound("No se ha encontrado el pedido con el identificador especificado.");
                }
            }
            else
            {
                return BadRequest("Usuario no autorizado o no encontrado");
            }
        }

        private IActionResult Ok(int orderId, int detalId)
        {
            throw new NotImplementedException();
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

