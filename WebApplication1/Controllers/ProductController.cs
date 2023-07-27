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
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ServiceContext _serviceContext;
        

        public ProductController(IProductService productService, ServiceContext serviceContext)
        {
            _productService = productService;
            _serviceContext = serviceContext;
        }
     
        [HttpPost(Name = "InsertProduct")]
        public int Post([FromQuery] string userNombreUsuario, [FromQuery] string userContraseña, [FromBody] ProductItem productItem)
        {

            var seletedUser = _serviceContext.Set<UserItem>()
                               .Where(u => u.NombreUsuario == userNombreUsuario
                                    && u.Contraseña == userContraseña
                                    && u.Rol == 1)
                                .FirstOrDefault();

            if (seletedUser != null)
            {
                return _productService.insertProduct(productItem);
            }
            else
            {
                throw new InvalidCredentialException("El ususario no esta autorizado o no existe");
            }

        }
        
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
                    product.name = updatedProduct.name;
                    product.brand = updatedProduct.brand;
                    product.sneakerModel = updatedProduct.sneakerModel;
                    product.typeOfFootwear = updatedProduct.typeOfFootwear;
                    product.recipient = updatedProduct.recipient;
                    product.size = updatedProduct.size;
                    product.color = updatedProduct.color;
                    product.brand = updatedProduct.brand;
                    product.stock = updatedProduct.stock;
                    product.price = updatedProduct.price;
                    product.discount = updatedProduct.discount;


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
