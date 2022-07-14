using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task CreateUser(User user)
        {
            await using DbConnection connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync
            (
                @"INSERT INTO users 
                     (email, password, name) 
                 VALUES 
                     (@email, @password, @name)",
                user
            );
        }

        public async Task<User> FindUser(string email)
        {
            await using DbConnection connection = new NpgsqlConnection(_connectionString);
            return connection.QueryAsync<User>
                (
                    @"SELECT * FROM users 
                 WHERE (
                     users.email = @email
                     )",
                    new {email}
                )
                .Result
                .FirstOrDefault();
        }
    }
}