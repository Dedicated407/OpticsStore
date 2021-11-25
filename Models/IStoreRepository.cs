using System.Collections.Generic;

namespace OpticsStore.Models
{
    public interface IStoreRepository
    {
        List<User> GetUsers();
        List<Order> GetAllOrders();
        List<Clinic> GetClinics();
        List<Factory> GetFactories();
    }
}