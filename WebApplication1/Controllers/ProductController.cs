﻿using Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Web.Http.Cors;
using WebApiEcommerce.IServices;
using WebApiEcommerce.Services;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Entities.Entities;

namespace WebApiEcommerce.Controllers
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
                    .Where(u => u.UserName == userNombreUsuario
                        && u.Contraseña == userContraseña
                        && u.Rol == "Admin")
                    .FirstOrDefault();

                if (selectedUser != null)
                {
                    // Выполняем добавление продукта
                    int productId = _productService.InsertProduct(productItem);

                    return Ok("El producto " + productId + " añadido"); // Возвращаем статус 200 OK с данными productId
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
     



        [HttpGet(Name = "GetAllProducts")]
        public List<ProductItem> GetAllProducts([FromQuery] string NombreUsuario, [FromQuery] string Contraseña)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                               .Where(u => u.UserName == NombreUsuario
                                    && u.Contraseña == Contraseña
                                    && u.Rol == "Admin")
                                .FirstOrDefault();

            {
                return _productService.GetAllProducts();
            }
        }

        [HttpGet(Name = "GetProductById")]
        public List<ProductItem> GetProductById([FromQuery] string NombreUsuario, [FromQuery] string Contraseña, [FromQuery] int id)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                               .Where(u => u.UserName == NombreUsuario
                                    && u.Contraseña == Contraseña
                                    && u.Rol == "Admin")
                                .FirstOrDefault();
            {
                return _productService.GetProductById(id);
            }
        }




        //eliminar un producto de la tabla Products по Id
        [HttpDelete(Name = "DeleteProduct")]
        public void DeleteProduct([FromQuery] string NombreUsuario, [FromQuery] string Contraseña, int id)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                                   .Where(u => u.UserName == NombreUsuario
                                        && u.Contraseña == Contraseña
                                        && u.Rol == "Admin")
                                    .FirstOrDefault();

                {
                _productService.DeleteProduct(id);
            }
        }






        //modificar registros de la tabla "Products"
        [HttpPut(Name = "UpdateProduct")]
        public IActionResult UpdateProduct([FromQuery] string userNombreUsuario, [FromQuery] string userContraseña, [FromBody] ProductItem updatedProduct, int CourseId)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                                   .Where(u => u.UserName == userNombreUsuario
                                        && u.Contraseña == userContraseña
                                        && u.Rol == "Admin")
                                    .FirstOrDefault();

            if (seletedUser != null)
            {
                var product = _serviceContext.Products.FirstOrDefault(p => p.CourseId == CourseId);

                if (product != null)
                {
                    product.CourseName = updatedProduct.CourseName;
                    product.CourseTeacher = updatedProduct.CourseTeacher;
                    product.CourseTime = updatedProduct.CourseTime;
                    product.CoverURL = updatedProduct.CoverURL;

                    _serviceContext.SaveChanges();

                    return Ok("El course se ha actualizado correctamente.");
                }
                else
                {
                    return NotFound("No se ha encontrado el course con el identificador especificado.");
                }
            }
            else
            {
                return Unauthorized("El usuario no está autorizado o no existe");
            }
        }
    }

}
