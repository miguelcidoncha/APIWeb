using Data;
using Entities;
using Microsoft.AspNetCore.Mvc;
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

        //Unidad de verificación de los derechos de acceso
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
    }
}
