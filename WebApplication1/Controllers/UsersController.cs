using Data;
using Entities.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;
using System.Web.Http.Cors;
using WebApplication1.IServices;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Route("[controller]/[action]")]
    public class UsersController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly ServiceContext _serviceContext;

        public UsersController(IUserService userService, ServiceContext serviceContext)
        {
            _userService = userService;
            _serviceContext = serviceContext;

        }

        [HttpPost(Name = "InsertUser")]

        //Unidad de verificación de los derechos de acceso
        public int Post([FromQuery] string userNombreUsuario, [FromQuery] string userContraseña, [FromBody] UserItem userItem)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                               .Where(u => u.NombreUsuario == userNombreUsuario
                                    && u.Contraseña == userContraseña
                                    && u.IdRol == 1)
                                .FirstOrDefault();
            if ( seletedUser != null)
            {

                return _userService.InsertUser(userItem);
            }
            else
            {
                throw new InvalidCredentialException("El ususario no esta autorizado o no existe");
            }
        }


        //[HttpPut(Name = "UpdateUser")]
        //public IActionResult UpdateUser(int userId, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña, [FromBody] UserItem updatedUser)
        //{
        //    var seletedUser = _serviceContext.Set<UserItem>()
        //                           .Where(u => u.NombreUsuario == userNombreUsuario
        //                                && u.Contraseña == userContraseña
        //                                && u.IdRol == 1)
        //                            .FirstOrDefault();

        //    if (seletedUser != null)
        //    {
        //        var user = _serviceContext.Users.FirstOrDefault(p => p.Usuario == userId);

        //        if (user != null)
        //        {
        //            user.NombreUsuario = updatedUser.NombreUsuario;
        //            user.IdRol = updatedUser.IdRol;
        //            user.Contraseña = updatedUser.Contraseña;
        //            user.Email = updatedUser.Email;

        //            _serviceContext.SaveChanges();

        //            return Ok("El ususario se ha actualizado correctamente.");
        //        }
        //        else
        //        {
        //            return NotFound("No se ha encontrado el usuario con el identificador especificado.");
        //        }
        //    }
        //    else
        //    {
        //        return Unauthorized("El usuario no está autorizado o no existe");
        //    }
        //}



       

    }
}
