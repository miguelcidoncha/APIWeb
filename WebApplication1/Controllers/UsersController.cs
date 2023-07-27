using Data;
using Entities;
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

        [HttpPost(Name = "insertUsers")]
        public int Post([FromQuery] string userNombreUsuario, [FromQuery] string userContraseña, [FromBody] UserItem userItem)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                               .Where(u => u.NombreUsuario == userNombreUsuario
                                    && u.Contraseña == userContraseña
                                    && u.Rol == 1)
                                .FirstOrDefault();
            if ( seletedUser != null)
            {
                return _userService.insertUsers(userItem);
            }
            else
            {
                throw new InvalidCredentialException("El ususario no esta autorizado o no existe");
            }
        }


        [HttpPut(Name = "UpdateUser")]
        public IActionResult UpdateUser(int IdUsuario, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña, [FromBody] UserItem updatedUser)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                                   .Where(u => u.NombreUsuario == userNombreUsuario
                                        && u.Contraseña == userContraseña
                                        && u.Rol == 1)
                                    .FirstOrDefault();

            if (seletedUser != null)
            {
                var user = _serviceContext.Users.FirstOrDefault(p => p.IdUsuario == IdUsuario);

                if (user != null)
                {
                    user.NombreUsuario = updatedUser.NombreUsuario;
                    user.Contraseña = updatedUser.Contraseña;
                    user.Rol = updatedUser.Rol;

                    _serviceContext.SaveChanges();

                    return Ok("El ususario se ha actualizado correctamente.");
                }
                else
                {
                    return NotFound("No se ha encontrado el usuario con el identificador especificado.");
                }
            }
            else
            {
                return Unauthorized("El usuario no está autorizado o no existe");
            }
        }



        [HttpDelete("{IdUsuario}", Name = "DeleteUser")]
        public IActionResult Delete(int IdUsuario, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                                   .Where(u => u.NombreUsuario == userNombreUsuario
                                        && u.Contraseña == userContraseña
                                        && u.Rol == 1)
                                    .FirstOrDefault();

            if (seletedUser != null)
            {
                var user = _serviceContext.Users.Find(IdUsuario);

                if (user != null)
                {
                    bool isDeleted = _serviceContext.RemoveUserById(IdUsuario);

                    if (isDeleted)
                    {
                        return Ok("El ususario se ha eliminado correctamente.");
                    }
                    else
                    {
                        return BadRequest("Error al eliminar un usuario.");
                    }
                }
                else
                {
                    return NotFound("No se ha encontrado el usuario con el identificador especificado.");
                }
            }
            else
            {
                 return Unauthorized("El usuario no está autorizado o no existe");
            }
        }

    }
}
