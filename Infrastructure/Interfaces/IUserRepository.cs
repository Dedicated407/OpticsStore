using OpticsStore.Models;

namespace OpticsStore.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        public void CreateUser(User user);
        public User FindUser(string email);
    }
}