namespace OpticsStore.Models
{
    public interface IUserRepository
    {
        public User FindUser(string email);
    }
}