using System.Collections.Generic;

namespace OpticsStore.Models
{
    public class User : UserLogin
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        
        public int CurrentOrderId { get; set; }
        public Order CurrentOrder { get; set; }
        public List<Order> UserOrders { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}