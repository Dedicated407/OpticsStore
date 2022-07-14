using System.Threading.Tasks;
using OpticsStore.Models;

namespace OpticsStore.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        Task CreateUser(User user);
        Task<User> FindUser(string email);
    }
}