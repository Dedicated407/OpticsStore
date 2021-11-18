using System.Linq;

namespace OpticsStore.Models
{
    public interface IStoreRepository
    {
        IQueryable<User> Users { get; }
    }
}