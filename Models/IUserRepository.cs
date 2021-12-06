namespace OpticsStore.Models
{
    public interface IUserRepository
    {
        public void CreateUser(User user);
        public User FindUser(string email);
    }
}