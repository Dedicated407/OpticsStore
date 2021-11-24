using System.Collections.Generic;

namespace OpticsStore.Models
{
    public interface IStoreRepository
    {
        void CreateUser(User user);
        void Update(User user);
        void Delete(int id);
        User Get(int id);
        List<User> GetUsers();
        List<Order> GetAllOrders();
        List<Clinic> GetClinics();
        List<Factory> GetFactories();
    }
}