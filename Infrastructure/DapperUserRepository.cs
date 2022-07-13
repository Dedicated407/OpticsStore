using System.Data.Common;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using OpticsStore.Infrastructure.Interfaces;
using OpticsStore.Models;

namespace OpticsStore.Infrastructure
{
    public class DapperUserRepository : IUserRepository
    {
        private readonly string _connectionString;
        
        public DapperUserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void CreateUser(User user)
        {
            using DbConnection connection = new NpgsqlConnection(_connectionString);
            connection.Execute
            (
                @"INSERT INTO users 
                     (email, password, name) 
                 VALUES 
                     (@email, @password, @name)", 
                user
            );
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