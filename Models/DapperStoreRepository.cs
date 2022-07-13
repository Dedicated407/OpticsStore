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

        public User FindUser(string email)
        {
            const string sql = "SELECT * FROM users WHERE email = @email";
            using DbConnection connection = new NpgsqlConnection(_connectionString);
            return connection.Query<User>(sql, new { email }).FirstOrDefault();
        }

        public List<User> FindUsersByFilter(string searchString)
        {
            searchString = '%' + searchString + '%';
            const string sql = 
                @"SELECT * FROM users AS u
                  WHERE concat(u.email, u.name, u.surname, u.patronymic) LIKE @searchString";
            using DbConnection connection = new NpgsqlConnection(_connectionString);
            return connection.Query<User>(sql, new {searchString}).ToList();
        }
        
        public List<Order> FindUserOrders(int id)
        {
            const string sql = 
                @"SELECT * 
                  FROM orders AS o
                      JOIN users u ON u.id = o.userid
                      JOIN orderStatus os ON os.id = o.orderStatusId
                      JOIN glassesFrames gf ON gf.id = o.glassesFrameId
                      JOIN clinics c ON c.id = o.clinicId
                      JOIN factories f ON f.id = c.factoryId
                  WHERE u.id = @id";
            using DbConnection connection = new NpgsqlConnection(_connectionString);
            return connection.Query<Order, OrderStatus, GlassesFrame, Clinic, Factory, Order>(
                sql,(order, orderStatus, glassesFrame, clinic, factory) =>
                {
                    order.OrderStatus = orderStatus;
                    order.GlassesFrame = glassesFrame;
                    order.Clinic = clinic;
                    clinic.Factory = factory;
                    return order;
                }, new { id }).ToList(); 
        }
        
        #region GetAll
        
        public List<User> GetUsers()
        {
            const string sql =
                @"SELECT * 
                  FROM users AS u
                      JOIN roles r ON r.id = u.roleid 
                  ORDER BY u.id";
            using DbConnection connection = new NpgsqlConnection(_connectionString);
            return connection.Query<User, Role, User>(
                sql, (user, role) =>
                {
                    user.Role = role;
                    return user;
                }).ToList();
        }
        
        public List<Order> GetAllOrders()
        {
            const string sql =
                @"SELECT *
                  FROM orders AS o
                      JOIN users u ON u.id = o.userid 
                      JOIN orderStatus os ON os.id = o.orderStatusId
                      JOIN glassesFrames gf ON gf.id = o.glassesFrameId
                      JOIN clinics c ON c.id = o.clinicId
                      JOIN factories f ON f.id = c.factoryId
                  ORDER BY o.id";
            using DbConnection connection = new NpgsqlConnection(_connectionString);
            return connection.Query<Order, User, OrderStatus, GlassesFrame, Clinic, Factory, Order>
                (
                    sql, (order, user, orderStatus, glassesFrame, clinic, factory) =>
                    {
                        order.User = user;
                        order.OrderStatus = orderStatus;
                        order.GlassesFrame = glassesFrame;
                        order.Clinic = clinic;
                        clinic.Factory = factory;
                        return order;
                    }).ToList();
        }
        
        public List<Clinic> GetClinics()
        {
            const string sql = 
                @"SELECT * 
                  FROM clinics AS cl
                      JOIN cities c on cl.cityid = c.id
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
                    JOIN cities c on f.cityid = c.id
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
        
        public List<GlassesFrame> GetGlassesFrames()
        {
            const string sql = "SELECT * FROM glassesFrames ORDER BY id";
            using DbConnection connection = new NpgsqlConnection(_connectionString);
            return connection.Query<GlassesFrame>(sql).ToList();
        }

        #endregion
        
        public GlassesFrame GetGlassesFrame(int id)
        {
            const string sql = "SELECT * FROM glassesFrames WHERE Id = @id";
            using DbConnection connection = new NpgsqlConnection(_connectionString);
            return connection.Query<GlassesFrame>(sql, new { id }).FirstOrDefault();
        }

        public void CreateOrder(Order order, string userEmail)
        {
            order.UserId = FindUser(userEmail).Id;
            order.Price = GetGlassesFrame(order.GlassesFrameId).Price + 1000M;
            const string sql = 
                @"INSERT INTO orders
                      (userId, glassesFrameId, userRecipe, price, clinicId) 
                  VALUES 
                      (@userId, @glassesFrameId, @userRecipe, @price, @clinicId)";
            using DbConnection connection = new NpgsqlConnection(_connectionString);
            connection.Execute
            (
                sql, 
                order
            );
        }
    }
}