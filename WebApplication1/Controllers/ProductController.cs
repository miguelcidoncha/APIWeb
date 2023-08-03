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
        //devuelve Ok(productId), donde productId es el IActionResult del método con el atributo HttpPost.
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
                    // Añadir un producto
                    int productId = _productService.insertProduct(productItem);
                    // Registro de la acción de añadir un producto
                    _serviceContext.AuditLogs.Add(new AuditLog
                    {
                        Action = "Insert",
                        TableName = "Products",
                        RecordId = productId,
                        Timestamp = DateTime.Now,
                        UserId = seletedUser.IdUsuario
                    });
                    _serviceContext.SaveChanges(); // Guardar los cambios en la base de datos

                    return Ok(productId);  // Devuelve el estado 200 OK con los datos productId
                }
                else
                {
                    return BadRequest("Usuario no autorizado o no encontrado"); // Mensaje de error
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al añadir el producto: " + ex.Message);
            }
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

        //Buscar por marca
        [HttpGet("SearchByBrand", Name = "SearchProductByBrand")]
        public IActionResult SearchByBrand([FromQuery] string userNombreUsuario, [FromQuery] string userContraseña, [FromQuery] string brandName)
        {
            try
            {
                var seletedUser = _serviceContext.Set<UserItem>()
                                       .FirstOrDefault(u => u.NombreUsuario == userNombreUsuario
                                            && u.Contraseña == userContraseña);

                if (seletedUser == null)
                {
                    return Unauthorized("El usuario no existe");
                }

                var products = _serviceContext.Products
                    .Where(p => p.BrandName.Contains(brandName)) // Utilice el método Contains para buscar una parte del valor BrandName
                    .ToList();

                if (products.Count > 0)
                {
                    return Ok(products); // Devuelve la lista de productos que cumplen la condición de búsqueda
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
        // modificar los registros de la tabla "Productos
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
                        // Registro de la acción de actualización de un producto
                        _serviceContext.AuditLogs.Add(new AuditLog
                        {
                            Action = "Update",
                            TableName = "Products",
                            RecordId = productId,
                            Timestamp = DateTime.Now,
                            UserId = seletedUser.IdUsuario // Añadir información de UserId a AuditLog
                        });
                        // Actualizar los valores de los campos del producto utilizando los datos de updatedProduct
                        product.ProductName = updatedProduct.ProductName;
                        product.BrandName = updatedProduct.BrandName;
                        product.ProductModel = updatedProduct.ProductModel;
                        product.TypeOfFootwear = updatedProduct.TypeOfFootwear;
                        product.Recipient = updatedProduct.Recipient;
                        product.ProductSize = updatedProduct.ProductSize; 
                        product.ProductColor = updatedProduct.ProductColor;
                        product.ProductStock = updatedProduct.ProductStock;
                        product.ProductPrice = updatedProduct.ProductPrice;
                        product.Discount = updatedProduct.Discount;

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
        // Eliminar un registro de la tabla "Productos
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

                // Registro de la acción de retirada del producto
                _serviceContext.AuditLogs.Add(new AuditLog
                {
                    Action = "Delete",
                    TableName = "Products",
                    RecordId = productId,
                    Timestamp = DateTime.Now,
                    UserId = seletedUser.IdUsuario // Añadir información de UserId a AuditLog
                });

                // Llamar al método para eliminar un producto por identificador
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
