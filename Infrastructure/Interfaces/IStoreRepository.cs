using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpticsStore.Models;

namespace OpticsStore.Infrastructure.Interfaces
{
    public interface IStoreRepository
    {
        Task<User> FindUser(string email);
        Task<List<User>> FindUsersByFilter(string searchString);
        Task<List<Order>> FindUserOrders(int id, CancellationToken cancellationToken);
        Task<List<User>> GetUsers(CancellationToken cancellationToken);
        Task<List<Order>> GetAllOrders(CancellationToken cancellationToken);
        Task<List<Clinic>> GetClinics(CancellationToken cancellationToken);
        Task<List<Factory>> GetFactories(CancellationToken cancellationToken);
        Task<GlassesFrame> GetGlassesFrame(int id);
        Task<List<GlassesFrame>> GetGlassesFrames(CancellationToken cancellationToken);
        Task CreateOrder(Order order, string userEmail);
    }
}