﻿using Data;
using Entities.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;
using System.Web.Http.Cors;
using WebApiEcommerce.IServices;
using WebApiEcommerce.Services;

namespace WebApiEcommerce.Controllers
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
        public int Post([FromQuery] string NombreUsuario, [FromQuery] string Contraseña, [FromBody] UserItem userItem)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                               .Where(u => u.UserName == NombreUsuario
                                    && u.Contraseña == Contraseña
                                    && u.Rol == "Admin")
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


        [HttpGet(Name = "GetAllUsers")]
        public List<UserItem> GetAllUsers([FromQuery] string NombreUsuario, [FromQuery] string Contraseña)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                               .Where(u => u.UserName == NombreUsuario
                                    && u.Contraseña == Contraseña
                                    && u.Rol == "Admin")
                                .FirstOrDefault();

            {
                return _userService.GetAllUsers();
            }
        }

        [HttpDelete(Name = "DeleteUser")]
        public void Delete([FromQuery] string NombreUsuario, [FromQuery] string Contraseña, [FromQuery] int id)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                               .Where(u => u.UserName == NombreUsuario
                                    && u.Contraseña == Contraseña
                                    && u.Rol == "Admin")
                                .FirstOrDefault();
            {
                _userService.DeleteUser(id);
            }
        }

        [HttpGet(Name = "GetUsersById")]
        public List<UserItem> GetUserById([FromQuery] string NombreUsuario, [FromQuery] string Contraseña, [FromQuery] int id)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                               .Where(u => u.UserName == NombreUsuario
                                    && u.Contraseña == Contraseña
                                    && u.Rol == "Admin")
                                .FirstOrDefault();
                {
                return _userService.GetUsersById(id);
            }
        }


        [HttpPut(Name = "UpdateUser")]
        public IActionResult UpdateUser(int userId, [FromQuery] string NombreUsuario, [FromQuery] string Contraseña, [FromBody] UserItem updatedUser)
        {
            var seletedUser = _serviceContext.Set<UserItem>()
                                   .Where(u => u.UserName == NombreUsuario
                                        && u.Contraseña == Contraseña
                                        && u.Rol == "Admin")
                                    .FirstOrDefault();

            //if (seletedUser != null)
            //{
            //    var user = _serviceContext.Users.FirstOrDefault(p => p.UserId == userId);

            //    if (user != null)
            //    {
            //        user.UserName = updatedUser.UserName;
            //        user.Rol = updatedUser.Rol;
            //        user.Contraseña = updatedUser.Contraseña;
            //        user.Email = updatedUser.Email;

            //        _serviceContext.SaveChanges();

            //        return Ok("El ususario se ha actualizado correctamente.");
            //    }
            //    else
            //    {
            //        return NotFound("No se ha encontrado el usuario con el identificador especificado.");
            //    }
            //}
            //else
            //{
            //    return Unauthorized("El usuario no está autorizado o no existe");
            //}
            if (seletedUser != null)
            {
                var user = _serviceContext.Users.FirstOrDefault(p => p.UserId == userId);

                if (user != null)
                {
                    // Теперь вы можете обновить только те поля, которые необходимо изменить.
                    // Например, если вы хотите обновить только UserName и Email:

                    if (!string.IsNullOrEmpty(updatedUser.UserName))
                    {
                        user.UserName = updatedUser.UserName;
                    }

                    if (!string.IsNullOrEmpty(updatedUser.Email))
                    {
                        user.Email = updatedUser.Email;
                    }

                    _serviceContext.SaveChanges();

                    return Ok("Пользователь успешно обновлен.");
                }
                else
                {
                    return NotFound("Пользователь с указанным идентификатором не найден.");
                }
            }
            else
            {
                return Unauthorized("Пользователь не авторизован или не существует.");
            }

        }



        //[HttpDelete("{userId}", Name = "DeleteUser")]
        //public IActionResult DeleteUser(int userId, [FromQuery] string userNombreUsuario, [FromQuery] string userContraseña)
        //{
        //    var seletedUser = _serviceContext.Set<UserItem>()
        //                           .Where(u => u.NombreUsuario == userNombreUsuario
        //                                && u.Contraseña == userContraseña
        //                                && u.RolId == 1)
        //                            .FirstOrDefault();

        //    if (seletedUser != null)
        //    {
        //        var user = _serviceContext.Users.FirstOrDefault(p => p.UsuarioId == userId);

        //        if (user != null)
        //        {
        //            _serviceContext.Users.Remove(user); // Удаляем пользователя
        //            _serviceContext.SaveChanges(); // Сохраняем изменения в базе данных

        //            return Ok("El ususario se ha eliminado correctamente.");
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
