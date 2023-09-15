using Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Web.Http.Cors;
using WebApiEcommerce.IServices;
using WebApiEcommerce.Services;
using Entities.Entities;

namespace WebApiEcommerce.Controllers
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
        public IActionResult CreateOrder([FromBody] OrderItem orderItem)
        {



            if (orderItem != null)
            {
                int orderId = _orderService.InsertOrder(orderItem);

                return Ok(orderId);
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


        
        [HttpGet(Name = "GetAllOrders")]
        public List<OrderItem> GetAll()
        {
            return _orderService.GetAllOrders();
        }





        [HttpGet(Name = "GetOrderById")]
        public List<OrderItem> GetOrderById([FromQuery] int id)
        {
            return _orderService.GetOrderById(id);
        }



        [HttpDelete(Name = "DeleteOrder")]
        public void DeleteOrder(int id, [FromQuery] string NombreUsuario, [FromQuery] string Contraseña)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                                   .Where(u => u.UserName == NombreUsuario
                                        && u.Contraseña == Contraseña
                                        && u.Rol == "Admin")
                                    .FirstOrDefault();
            {
                _orderService.DeleteOrder(id);
            }
        }


        [HttpPut(Name = "UpdateOrder")]
        public IActionResult UpdateProduct(int OrderId, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña, [FromBody] OrderItem updatedOrder)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                                   .Where(u => u.UserName == userNombreUsuario
                                        && u.Contraseña == userContraseña
                                        && u.Rol == "Admin")
                                    .FirstOrDefault();

            if (seletedUser != null)
            {
                var order = _serviceContext.Orders.FirstOrDefault(p => p.OrderId == OrderId);

                if (order != null)
                {
                    order.OrderNumber = updatedOrder.OrderNumber;
                    order.DateOrder = updatedOrder.DateOrder;
                    order.DateExpiration = updatedOrder.DateExpiration;
                    order.TotalPrice = updatedOrder.TotalPrice;

                    _serviceContext.SaveChanges();

                    return Ok("El order se ha actualizado correctamente.");
                }
                else
                {
                    return NotFound("No se ha encontrado el order con el identificador especificado.");
                }
            }
            else
            {
                return Unauthorized("El usuario no está autorizado o no existe");
            }
        }
    }
}

