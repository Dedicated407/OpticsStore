using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using OpticsStore.Infrastructure.Interfaces;
using OpticsStore.Models;

namespace OpticsStore.Infrastructure
{
    class DapperStoreRepository : IStoreRepository
    {
        private readonly string _connectionString;
        private const decimal LensPrice = 1000M;

        public DapperStoreRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<User> FindUser(string email)
        {
            const string sql = "SELECT * FROM users WHERE email = @email";
            await using DbConnection connection = new NpgsqlConnection(_connectionString);
            return connection.QueryAsync<User>(sql, new {email})
                .Result
                .FirstOrDefault();
        }

        public async Task<List<User>> FindUsersByFilter(string searchString)
        {
            searchString = '%' + searchString + '%';
            const string sql =
                @"SELECT * FROM users AS u
                  WHERE concat(u.email, u.name, u.surname, u.patronymic) LIKE @searchString";
            await using DbConnection connection = new NpgsqlConnection(_connectionString);
            return connection.QueryAsync<User>(
                    sql,
                    new {searchString})
                .Result
                .ToList();
        }

        public async Task<List<Order>> FindUserOrders(int userId, CancellationToken cancellationToken)
        {
            const string sql =
                @"SELECT * 
                  FROM orders AS o
                      JOIN orderStatus os ON os.id = o.orderStatusId
                      JOIN glassesFrames gf ON gf.id = o.glassesFrameId
                      JOIN clinics c ON c.id = o.clinicId
                      JOIN factories f ON f.id = c.factoryId
                  WHERE o.userId = @userId
                  ORDER BY o.id DESC";
            await using DbConnection connection = new NpgsqlConnection(_connectionString);
            return connection.QueryAsync<Order, OrderStatus, GlassesFrame, Clinic, Factory, Order>(
                    sql, (order, orderStatus, glassesFrame, clinic, factory) =>
                    {
                        order.OrderStatus = orderStatus;
                        order.GlassesFrame = glassesFrame;
                        order.Clinic = clinic;
                        clinic.Factory = factory;
                        return order;
                    },
                    cancellationToken)
                .Result
                .ToList();
        }

        #region GetAll

        public async Task<List<User>> GetUsers(CancellationToken cancellationToken)
        {
            const string sql =
                @"SELECT * 
                  FROM users AS u
                      JOIN roles r ON r.id = u.roleid 
                  ORDER BY u.id";
            await using DbConnection connection = new NpgsqlConnection(_connectionString);
            return connection.QueryAsync<User, Role, User>(
                    sql, (user, role) =>
                    {
                        user.Role = role;
                        return user;
                    },
                    cancellationToken)
                .Result
                .ToList();
        }

        public async Task<List<Order>> GetAllOrders(CancellationToken cancellationToken)
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
            await using DbConnection connection = new NpgsqlConnection(_connectionString);
            return connection.QueryAsync<Order, User, OrderStatus, GlassesFrame, Clinic, Factory, Order>
                (
                    sql, (order, user, orderStatus, glassesFrame, clinic, factory) =>
                    {
                        order.User = user;
                        order.OrderStatus = orderStatus;
                        order.GlassesFrame = glassesFrame;
                        order.Clinic = clinic;
                        clinic.Factory = factory;
                        return order;
                    },
                    cancellationToken)
                .Result
                .ToList();
        }

        public async Task<List<Clinic>> GetClinics(CancellationToken cancellationToken)
        {
            const string sql =
                @"SELECT * 
                  FROM clinics AS cl
                      JOIN cities c on cl.cityid = c.id
                      JOIN countries cs on c.countryid = cs.id
                      JOIN factories f on cl.factoryid = f.id";
            await using DbConnection connection = new NpgsqlConnection(_connectionString);
            return connection.QueryAsync<Clinic, City, Country, Factory, Clinic>(
                    sql, (clinic, city, country, factory) =>
                    {
                        clinic.City = city;
                        city.Country = country;
                        clinic.Factory = factory;
                        return clinic;
                    },
                    cancellationToken)
                .Result
                .ToList();
        }

        public async Task<List<Factory>> GetFactories(CancellationToken cancellationToken)
        {
            const string sql =
                @"SELECT * 
                  FROM factories AS f 
                    JOIN cities c on f.cityid = c.id
                    JOIN countries cs on c.countryid = cs.id
                ";
            await using DbConnection connection = new NpgsqlConnection(_connectionString);
            return connection.QueryAsync<Factory, City, Country, Factory>(
                    sql, (factory, city, country) =>
                    {
                        factory.City = city;
                        city.Country = country;
                        return factory;
                    },
                    cancellationToken)
                .Result
                .ToList();
        }

        public async Task<List<GlassesFrame>> GetGlassesFrames(CancellationToken cancellationToken)
        {
            const string sql = "SELECT * FROM glassesFrames ORDER BY id";
            await using DbConnection connection = new NpgsqlConnection(_connectionString);
            return connection.QueryAsync<GlassesFrame>(
                    sql,
                    cancellationToken)
                .Result
                .ToList();
        }

        #endregion

        public async Task<GlassesFrame> GetGlassesFrame(int id)
        {
            const string sql = "SELECT * FROM glassesFrames WHERE Id = @id";
            await using DbConnection connection = new NpgsqlConnection(_connectionString);
            return connection.QueryAsync<GlassesFrame>(
                    sql,
                    new {id})
                .Result
                .FirstOrDefault();
        }

        public async Task CreateOrder(Order order, string userEmail)
        {
            var user = await FindUser(userEmail);
            var glassesFrame = await GetGlassesFrame(order.GlassesFrameId);

            order.UserId = user.Id;
            order.FullPrice = glassesFrame.Price + LensPrice;

            const string sql =
                @"INSERT INTO orders
                      (userId, glassesFrameId, userRecipe, fullPrice, clinicId) 
                  VALUES 
                      (@userId, @glassesFrameId, @userRecipe, @fullPrice, @clinicId)";
            await using DbConnection connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync
            (
                sql,
                order
            );
        }
    }
}