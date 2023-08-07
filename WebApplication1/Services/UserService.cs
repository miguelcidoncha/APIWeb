using Data;
using Entities.Entities;
using WebApplication1.IServices;

namespace WebApplication1.Services
{
    public class UserService : BaseContextService, IUserService
    {
        public UserService(ServiceContext serviceContext) : base(serviceContext)
        {
        }

        public int InsertUsers(UserItem userItem)
        {
            _serviceContext.Users.Add(userItem);
            _serviceContext.SaveChanges();
            return userItem.IdUsuario;
        }
    }
}
