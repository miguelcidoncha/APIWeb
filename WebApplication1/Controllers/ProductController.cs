using Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Web.Http.Cors;
using WebApplication1.IServices;
using WebApplication1.Services;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Entities.Entities;

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


        // Добавление продукта
        [HttpPost(Name = "InsertProduct")]
        public IActionResult InsertProduct([FromQuery] string userNombreUsuario, [FromQuery] string userContraseña, [FromBody] ProductItem productItem)
        {
            try
            {
                var selectedUser = _serviceContext.Set<UserItem>()
                    .Where(u => u.NombreUsuario == userNombreUsuario
                        && u.Contraseña == userContraseña
                        && u.RolId == 1)
                    .FirstOrDefault();

                if (selectedUser != null)
                {
                    // Выполняем добавление продукта
                    int productId = _productService.InsertProduct(productItem);


                    // Сохраняем изменения в базе данных
                    _serviceContext.SaveChanges();

                    return Ok(productId); // Возвращаем статус 200 OK с данными productId
                }
                else
                {
                    return BadRequest("Usuario no autorizado o no encontrado");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al añadir el producto: " + ex.Message);
            }
        }



        // modificar registros de la tabla "Products"
        //[HttpPut("{productId}", Name = "UpdateProduct")]
        //public IActionResult UpdateProduct(ushort productId, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña, [FromBody] ProductItem updatedProduct)
        //{
        //    try
        //    {
        //        var seletedUser = _serviceContext.Set<UserItem>()
        //                               .Where(u => u.NombreUsuario == userNombreUsuario
        //                                    && u.Contraseña == userContraseña
        //                                    && u.IdRol == 1)
        //                                .FirstOrDefault();

        //        if (seletedUser != null)
        //        {
        //            var product = _serviceContext.Products.FirstOrDefault(p => p.ProductId == productId);

        //            if (product != null)
        //            {

        //                // Обновляем значения полей продукта с помощью данных из updatedProduct
        //                product.ProductName = updatedProduct.ProductName;
        //                product.BrandName = updatedProduct.BrandName;
        //                product.ProductStock = updatedProduct.ProductStock;

        //                _serviceContext.SaveChanges();

        //                return Ok("El producto se ha actualizado correctamente.");
        //            }
        //            else
        //            {
        //                return NotFound("No se ha encontrado el producto con el identificador especificado.");
        //            }
        //        }
        //        else
        //        {
        //            return Unauthorized("El usuario no está autorizado o no existe");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Error al actualizar el producto: " + ex.Message);
        //    }
        //}


        //eliminar un producto de la tabla Products по Id
        // Удалить запись из таблицы "Products"
        //[HttpDelete("{productId}", Name = "DeleteProduct")]
        //public IActionResult Delete(ushort productId, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña)
        //{
        //    try
        //    {
        //        var seletedUser = _serviceContext.Set<UserItem>()
        //                               .FirstOrDefault(u => u.NombreUsuario == userNombreUsuario
        //                                    && u.Contraseña == userContraseña
        //                                    && u.IdRol == 1);


        //        if (seletedUser == null)
        //        {
        //            return Unauthorized("El usuario no está autorizado o no existe");
        //        }

        //        var product = _serviceContext.Products.Find(productId);

        //        if (product == null)
        //        {
        //            return NotFound("No se ha encontrado el producto con el identificador especificado.");
        //        }

        //// Журналирование действия удаления продукта
        //_serviceContext.AuditLogs.Add(new AuditLog
        //{
        //    Action = "Delete",
        //    TableName = "Products",
        //    RecordId = productId,
        //    Timestamp = DateTime.Now,
        //    UserId = seletedUser.IdUsuario // Добавляем информацию о UserId в AuditLog
        //});

        // Вызываем метод для удаления продукта по идентификатору
        //bool isDeleted = _serviceContext.RemoveProductById(productId);

        //    if (isDeleted)
        //    {
        //        return Ok("El producto se ha eliminado correctamente.");
        //    }
        //    else
        //    {
        //        return BadRequest("Error al eliminar un producto.");
        //    }
        //}
        //catch (Exception ex)
        //{
        //    return StatusCode(500, "Error al eliminar el producto: " + ex.Message);
        //}
        //}
    }
}
