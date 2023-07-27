using Data;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Web.Http.Cors;
using WebApplication1.IServices;
using WebApplication1.Services;
using System.Linq;
using Microsoft.AspNetCore.Identity;

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
        //возвращаю Ok(productId), где productId - это IActionResult для метода с атрибутом HttpPost.
        public IActionResult Post([FromQuery] string userNombreUsuario, [FromQuery] string userContraseña, [FromBody] ProductItem productItem)
        {
            try
            {
                    var seletedUser = _serviceContext.Set<UserItem>()
                                   .Where(u => u.NombreUsuario == userNombreUsuario
                                        && u.Contraseña == userContraseña
                                        && u.IdRol == 1)
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
                        Timestamp = DateTime.Now,
                        UserId = seletedUser.IdUsuario
                    });
                    _serviceContext.SaveChanges(); // Сохраняем изменения в базу данных

                    return Ok(productId); // // Возвращаем статус 200 OK с данными productId
                }
                else
                {
                    //throw new InvalidCredentialException("El ususario no esta autorizado o no existe");
                    return BadRequest("Usuario no autorizado o no encontrado"); // Возвращаем сообщение об ошибке
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al añadir el producto: " + ex.Message);
            }
        }

        //Pedir un producto de la tabla Products por su Id
        [HttpGet("{productId}", Name = "GetProduct")]
        public IActionResult Get(int productId)
        {
            try
            {
                var product = _serviceContext.Products.Include(p => p.Orders).FirstOrDefault(p => p.ProductId == productId);
                if (product != null)
                {
                    return Ok(product);
                }
                else
                {
                    return NotFound("No se ha encontrado el producto con el identificador especificado.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al obtener el producto: " + ex.Message);
            }
        }

        //Поиск по полю BrandName
        [HttpGet("SearchByBrand", Name = "SearchProductByBrand")]
        public IActionResult SearchByBrand([FromQuery] string userNombreUsuario, [FromQuery] string userContraseña, [FromQuery] string brandName)
        {
            try
            {
                var seletedUser = _serviceContext.Set<UserItem>()
                                       .FirstOrDefault(u => u.NombreUsuario == userNombreUsuario
                                            && u.Contraseña == userContraseña
                                            && u.IdRol == 1);

                if (seletedUser == null)
                {
                    return Unauthorized("El usuario no está autorizado o no existe");
                }

                var products = _serviceContext.Products
                    .Where(p => p.BrandName.Contains(brandName)) // Используем метод Contains для поиска по части значения BrandName
                    .ToList();

                if (products.Count > 0)
                {
                    return Ok(products); // Возвращаем список продуктов, удовлетворяющих условию поиска
                }
                else
                {
                    return NotFound("No se han encontrado productos con la marca especificada.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al realizar la búsqueda por marca: " + ex.Message);
            }
        }


        // modificar registros de la tabla "Products"
        // модифицировать записи в таблице "Products"
        [HttpPut("{productId}", Name = "UpdateProduct")]
        public IActionResult UpdateProduct(int productId, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña, [FromBody] ProductItem updatedProduct)
        {
            try
            {
                var seletedUser = _serviceContext.Set<UserItem>()
                                       .Where(u => u.NombreUsuario == userNombreUsuario
                                            && u.Contraseña == userContraseña
                                            && u.IdRol == 1)
                                        .FirstOrDefault();

                if (seletedUser != null)
                {
                    var product = _serviceContext.Products.FirstOrDefault(p => p.ProductId == productId);

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
                        product.BrandName = updatedProduct.BrandName;
                        product.Productstock = updatedProduct.Productstock;

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
            catch (Exception ex)
            {
                return StatusCode(500, "Error al actualizar el producto: " + ex.Message);
            }
        }


        //eliminar un producto de la tabla Products по Id
        // Удалить запись из таблицы "Products"
        [HttpDelete("{productId}", Name = "DeleteProduct")]
        public IActionResult Delete(int productId, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña)
        {
            try
            {
                var seletedUser = _serviceContext.Set<UserItem>()
                                       .FirstOrDefault(u => u.NombreUsuario == userNombreUsuario
                                            && u.Contraseña == userContraseña
                                            && u.IdRol == 1);


                if (seletedUser == null)
                {
                    return Unauthorized("El usuario no está autorizado o no existe");
                }

                var product = _serviceContext.Products.Find(productId);

                if (product == null)
                {
                    return NotFound("No se ha encontrado el producto con el identificador especificado.");
                }

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
            catch (Exception ex)
            {
                return StatusCode(500, "Error al eliminar el producto: " + ex.Message);
            }
        }
    }
}
