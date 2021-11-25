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

        #region GetAll
        
        public List<User> GetUsers()
        {
            using DbConnection connection = new NpgsqlConnection(_connectionString);
            return connection.Query<User>("SELECT * FROM users ORDER BY id").ToList();
        }
        
        public List<Order> GetAllOrders()
        {
            const string sql =
                @"SELECT *
                  FROM orders AS o
                      JOIN orderStatus os ON os.id = o.orderStatusId
                      JOIN glassesFrames gf ON gf.id = o.glassesFrameId
                      JOIN clinics c ON c.id = o.clinicId
                      JOIN factories f ON f.id = c.factoryId
                  ORDER BY o.id";
            using DbConnection connection = new NpgsqlConnection(_connectionString);
            return connection.Query<Order, OrderStatus, Clinic, Factory, GlassesFrame, Order>
                (
                    sql, (order, orderStatus, clinic, factory, glassesFrame) =>
                    {
                        order.OrderStatus = orderStatus;
                        order.Clinic = clinic;  
                        clinic.Factory = factory;
                        order.GlassesFrame = glassesFrame;
                        return order;
                    }).ToList();
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
        
        #endregion
    }
}