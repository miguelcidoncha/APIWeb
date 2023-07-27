using Data;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Web.Http.Cors;
using WebApplication1.IServices;
using WebApplication1.Services;
using System.Linq;

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
                // Выполняем добавление продукта
                int productId = _productService.insertProduct(productItem);

                // Журналирование действия добавления продукта
                _serviceContext.AuditLogs.Add(new AuditLog
                {
                    Action = "Insert",
                    TableName = "Products",
                    RecordId = productId,
                    Timestamp = DateTime.Now,
                    UserId = seletedUser.IdUsuario
                });
                _serviceContext.SaveChanges(); // Сохраняем изменения в базу данных
                return productId; // Возвращаем ID добавленного продукта
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
                    // Журналирование действия обновления продукта
                    _serviceContext.AuditLogs.Add(new AuditLog
                    {
                        Action = "Update",
                        TableName = "Products",
                        RecordId = productId,
                        Timestamp = DateTime.Now,
                        UserId = seletedUser.IdUsuario // Добавляем информацию о UserId в AuditLog
                    });
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
                    bool isDeleted = _serviceContext.RemoveUserById(productId);
                    // Журналирование действия удаления продукта
                    _serviceContext.AuditLogs.Add(new AuditLog
                    {
                        Action = "Delete",
                        TableName = "Products",
                        RecordId = productId,
                        Timestamp = DateTime.Now,
                        UserId = seletedUser.IdUsuario // Добавляем информацию о UserId в AuditLog
                    });
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
 }
