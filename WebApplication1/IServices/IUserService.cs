using Entities.Entities;

namespace WebApiEcommerce.IServices
{
    public interface IUserService
    {
        int InsertUser(UserItem usertItem);
        void UpdateUser(UserItem userItem);
        void DeleteUser(int id);
        List<UserItem> GetAllUsers();
        List<UserItem> GetUsersById(int UserId);
    }
}

