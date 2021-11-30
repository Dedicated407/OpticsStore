using System.Collections.Generic;

namespace OpticsStore.Models
{
    public interface IStoreRepository
    {
        User FindUser(string email);
        List<User> GetUsers();
        List<Order> GetAllOrders();
        List<Clinic> GetClinics();
        List<Factory> GetFactories();
        GlassesFrame GetGlassesFrame(int id);
        List<GlassesFrame> GetGlassesFrames();
        void CreateOrder(Order order, string userEmail);
    }
}