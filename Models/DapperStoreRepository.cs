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

        public List<Clinic> GetClinics()
        {
            const string sql = @"SELECT * 
                                 FROM clinics AS cl
                                    JOIN city c on cl.cityid = c.id
                                    JOIN countries cs on c.countryid = cs.id
                                    JOIN factories f on cl.factoryid = f.id";
            using DbConnection connection = new NpgsqlConnection(_connectionString);
            return connection.Query<Clinic, City, Country, Factory, Clinic>(
                sql,(clinic, city, country, factory) =>
                {
                    clinic.City = city;
                    city.Country = country;
                    clinic.Factory = factory;
                    return clinic;
                }).ToList();
        }
        
        public List<Factory> GetFactories()
        {
            const string sql = 
                @"SELECT * 
                  FROM factories AS f 
                    JOIN city c on f.cityid = c.id
                    JOIN countries cs on c.countryid = cs.id
                ";
            using DbConnection connection = new NpgsqlConnection(_connectionString);
            return connection.Query<Factory, City, Country, Factory>(
                sql, (factory, city, country) =>
                {
                    factory.City = city;
                    city.Country = country;
                    return factory;
                }).ToList();
        }
    }
}