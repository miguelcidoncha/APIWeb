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

        //Pedir un producto de la tabla Products por su Id
        //[HttpGet("{productId}", Name = "GetProduct")]
        //public IActionResult Get(int productId)
        //{
        //    try
        //    {
        //        var product = _serviceContext.Products.Include(p => p.Orders).FirstOrDefault(p => p.ProductId == productId);
        //        if (product != null)
        //        {
        //            return Ok(product);
        //        }
        //        else
        //        {
        //            return NotFound("No se ha encontrado el producto con el identificador especificado.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Error al obtener el producto: " + ex.Message);
        //    }
        //}


        //[HttpGet("GetProductsInOrder/{orderId}", Name = "GetProductsInOrder")]
        //public IActionResult GetProductsInOrder(int orderId)
        //{
        //    try
        //    {
        //        var order = _serviceContext.Orders
        //            .Include(o => o.OrderProduct)
        //                .ThenInclude(op => op.Product)
        //            .FirstOrDefault(o => o.OrderId == orderId);

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


        //Поиск по полю BrandName
        // Добавление продукта
        [HttpPost(Name = "InsertProduct")]
        public IActionResult InsertProduct([FromQuery] string userNombreUsuario, [FromQuery] string userContraseña, [FromBody] ProductItem productItem)
        {
            try
            {
                var selectedUser = _serviceContext.Set<UserItem>()
                    .Where(u => u.NombreUsuario == userNombreUsuario
                        && u.Contraseña == userContraseña
                        && u.IdRol == 1)
                    .FirstOrDefault();

                if (selectedUser != null)
                {
                    // Выполняем добавление продукта
                    int productId = _productService.InsertProduct(productItem);

                    // Журналирование действия добавления продукта
                    _serviceContext.AuditLogs.Add(new AuditLog
                    {
                        Action = "Insert",
                        TableName = "Products",
                        RecordId = productId,
                        Timestamp = DateTime.Now,
                        UserId = selectedUser.IdUsuario
                    });

                    // Сохраняем изменения в базе данных
                    _serviceContext.SaveChanges();

                    return Ok(productId); // Возвращаем статус 200 OK с данными productId
                }
                else
                {
                    return BadRequest("Usuario no autorizado o no encontrado"); // Возвращаем сообщение об ошибке
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al añadir el producto: " + ex.Message);
            }
        }
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


        [HttpGet("{id}", Name = "Insert-Product-Image")]
        public IActionResult GetImage(ushort id)
        {
            var image = _serviceContext.Images.FirstOrDefault(i => i.IdImage == id);
            if (image != null)
            {
                return File(image.ImageData, "image/jpeg"); // Возвращаем файл изображения
            }
            else
            {
                return NotFound("Imagen no encontrada");
            }
        }

        [HttpPost]
        public IActionResult UploadImage([FromForm] IFormFile file, [FromForm] ushort productId)
        //public IActionResult UploadImage([FromBody] IFormFile file, [FromForm] ushort productId)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Error al cargar el archivo");
                }

                // Читаем данные файла в массив байтов
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    var imageData = memoryStream.ToArray();

                    // Создаем новую сущность ImageItem
                    var image = new ImageItem
                    {
                        ImageData = imageData,
                        ProductId = productId
                    };

                    // Добавляем изображение в таблицу Images
                    _serviceContext.Images.Add(image);
                    _serviceContext.SaveChanges();

                    return Ok("La imagen se ha cargado y guardado correctamente");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al cargar una imagen: " + ex.Message);
            }
        }


        // modificar registros de la tabla "Products"
        [HttpPut("{productId}", Name = "UpdateProduct")]
        public IActionResult UpdateProduct(ushort productId, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña, [FromBody] ProductItem updatedProduct)
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
                        product.ProductStock = updatedProduct.ProductStock;

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
        public IActionResult Delete(ushort productId, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña)
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
