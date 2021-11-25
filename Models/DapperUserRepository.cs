using System.Data.Common;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace OpticsStore.Models
{
    public class DapperUserRepository : IUserRepository
    {
        private readonly string _connectionString;
        
        public DapperUserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public User FindUser(string email)
        {
            using DbConnection connection = new NpgsqlConnection(_connectionString);
            return connection.Query<User>
            (
            @"SELECT * FROM users 
                 WHERE (
                     users.email = @email
                     )",
            new { email }
            ).FirstOrDefault();
        }
    }
}