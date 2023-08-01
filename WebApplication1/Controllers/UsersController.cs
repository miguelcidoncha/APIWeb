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

        [HttpPost(Name = "InsertUsers")]

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
                // Журналирование действия
                _serviceContext.AuditLogs.Add(new AuditLog
                {
                    Action = "Insert",
                    TableName = "Users",
                    //RecordId = userItem.IdRol,
                    Timestamp = DateTime.Now,
                    UserId = seletedUser.IdUsuario
                });
                return _userService.InsertUsers(userItem);
            }
            else
            {
                throw new InvalidCredentialException("El ususario no esta autorizado o no existe");
            }
        }


        [HttpPut(Name = "UpdateUser")]
        public IActionResult UpdateUser(int userId, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña, [FromBody] UserItem updatedUser)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                                   .Where(u => u.NombreUsuario == userNombreUsuario
                                        && u.Contraseña == userContraseña
                                        && u.IdRol == 1)
                                    .FirstOrDefault();

            if (seletedUser != null)
            {
                var user = _serviceContext.Users.FirstOrDefault(p => p.IdUsuario == userId);

                if (user != null)
                {
                    user.NombreUsuario = updatedUser.NombreUsuario;
                    user.IdRol = updatedUser.IdRol;
                    user.Contraseña = updatedUser.Contraseña;
                    user.Email = updatedUser.Email;

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



        [HttpDelete("{userId}", Name = "DeleteUser")]
        public IActionResult Delete(int userId, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                                   .Where(u => u.NombreUsuario == userNombreUsuario
                                        && u.Contraseña == userContraseña
                                        && u.IdRol == 1)
                                    .FirstOrDefault();

            if (seletedUser != null)
            {
                // Журналирование действия
                _serviceContext.AuditLogs.Add(new AuditLog
                {
                    Action = "Delete",
                    TableName = "Users",
                    //RecordId = userId,
                    Timestamp = DateTime.Now,
                    UserId = seletedUser.IdUsuario // Добавляем информацию о UserId в AuditLog
                });
                var user = _serviceContext.Users.Find(userId);

                if (user != null)
                {
                    bool isDeleted = _serviceContext.RemoveUserById(userId);

                    if (isDeleted)
                    {
                        return Ok("El usuario se ha eliminado correctamente.");
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
