using System.Collections.Generic;
using System.Linq;

namespace OpticsStore.Models
{
    public class OpticsStoreRepository : IStoreRepository
    {
        public IQueryable<User> Users => new List<User>
        {
            new() {Id = 1, Name = "Илья", Surname = "Цыпин", Patronymic = "Павлович", RoleId = 1}, 
            new() {Id = 2, Name = "Павел", Surname = "Иноземцев", RoleId = 1}, 
            new() {Id = 3, Name = "Максим", Surname = "Бабышев", RoleId = 1}, 
        }.AsQueryable();
    }
}