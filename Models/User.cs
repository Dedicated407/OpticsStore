namespace OpticsStore.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; } = null;
        public string Patronymic { get; set; } = null;
        public int CurrentOrderId { get; set; }
        public int RoleId { get; set; } = 1;
    }
}