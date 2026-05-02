using Entities.Concrete;

namespace Business.Abstract
{
    public interface IUserService
    {
        List<User> GetAll();
        User GetById(int id);
        User GetByEmail(string email);
        void Add(User user);
        void Update(User user);
        void Delete(User user);
        User GetByMail(string email);
        List<User> GetAllByRole(string role);
    }
}