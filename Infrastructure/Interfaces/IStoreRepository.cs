using System.Collections.Generic;
using OpticsStore.Models;

namespace OpticsStore.Infrastructure.Interfaces
{
    public interface IStoreRepository
    {
        User FindUser(string email);
        List<User> FindUsersByFilter(string searchString);
        List<Order> FindUserOrders(int id);
        List<User> GetUsers();
        List<Order> GetAllOrders();
        List<Clinic> GetClinics();
        List<Factory> GetFactories();
        GlassesFrame GetGlassesFrame(int id);
        List<GlassesFrame> GetGlassesFrames();
        void CreateOrder(Order order, string userEmail);
    }
}