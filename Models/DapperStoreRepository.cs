using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace OpticsStore.Models
{
    class DapperStoreRepository : IStoreRepository
    {
        private readonly string _connectionString;

        public DapperStoreRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        
        public void Create(User user)
        {
            using DbConnection connection = new NpgsqlConnection(_connectionString);
            connection.Execute("INSERT INTO users (name, role_id) VALUES(@name, @role_id)", user);
        }

        public void Delete(int id)
        {
            using DbConnection connection = new NpgsqlConnection(_connectionString);
            connection.Execute("DELETE FROM users WHERE id = @id", new { id });
        }

        public User Get(int id)
        {
            using DbConnection connection = new NpgsqlConnection(_connectionString);
            return connection.Query<User>("SELECT * FROM users WHERE id = @id", new { id }).FirstOrDefault();
        }

        public List<User> GetUsers()
        {
            using DbConnection connection = new NpgsqlConnection(_connectionString);
            return connection.Query<User>("SELECT * FROM users").ToList();
        }

        public void Update(User user)
        {
            using DbConnection connection = new NpgsqlConnection(_connectionString);
            connection.Execute("UPDATE users SET name=@name, surname=@surname, patronymic=@patronymic", user);
        }
    }
}