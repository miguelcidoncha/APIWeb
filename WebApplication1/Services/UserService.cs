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

        public int InsertUser(UserItem userItem)
        {
            _serviceContext.Users.Add(userItem);
            _serviceContext.SaveChanges();
            return userItem.UsuarioId;
        }


        public void DeleteUser(int id)
        {

            var userToDelete = _serviceContext.Set<UserItem>()
                 .Where(u => u.UsuarioId == id).First();

            _serviceContext.SaveChanges();

        }

        public List<UserItem> GetAllUsers()
        {
            return _serviceContext.Set<UserItem>()
                .ToList();

        }

        public List<UserItem> GetUsersById(int id)
        {
            var resultList = _serviceContext.Set<UserItem>()

                                .Where(u => u.UsuarioId == id);


            return resultList.ToList();
        }

        public int InsertUser2(UserItem userItem)
        {
            if (userItem.RolId == 1)
            {
                throw new InvalidOperationException("Acción no autorizada");
            };

            var existingUser = _serviceContext.Set<UserItem>()
                               .Where(u => u.NombreUsuario == userItem.NombreUsuario)
                               .FirstOrDefault();
            if (existingUser != null)
            {
                throw new InvalidOperationException("El nombre de usuario ya existe");
            };


            _serviceContext.Users.Add(userItem);
            _serviceContext.SaveChanges();

            return userItem.UsuarioId;
        }

        public void UpdateUser(UserItem userItem)
        {
            _serviceContext.Users.Update(userItem);
            _serviceContext.SaveChanges();
        }


    }
}
