using LinenAndBird.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;

namespace LinenAndBird.DataAccess
{
    public class OrdersRepository
    {
        const string _connectionString = "Server=localhost;Database=LinenAndBird;Trusted_Connection=True;";

        internal IEnumerable<Order> GetAll()
        {
            //create a connection
            using var db = new SqlConnection(_connectionString);

            var sql = @"SELECT *
                        FROM Orders o
	                        join Birds b
		                        on b.Id = o.BirdId
	                        join Hats h
		                        on h.Id = o.HatId";

            var results = db.Query<Order, Bird, Hat, Order>(sql, Map, splitOn:"Id");

            return results;

        }

        internal void Add(Order order)
        {
            //Create connection
            var db = new SqlConnection(_connectionString);

            var sql = @"INSERT INTO [dbo].[Orders]
                            ([BirdId]
                            ,[HatId]
                            ,[Price])
                        Output inserted.Id
                        VALUES
                            (@BirdId,
			                @HatId,
			                @Price)";

            var parameters = new
                {
                    BirdId = order.Bird.Id,
                    HatId = order.Hat.Id,
                    Price = order.Price
                };

            var id = db.ExecuteScalar<Guid>(sql, parameters);
        }

        public Order Get(Guid id)
        {
            using var db = new SqlConnection(_connectionString);

            var sql = @"SELECT *
                        FROM Orders o
	                        join Birds b
		                        on b.Id = o.BirdId
	                        join Hats h
		                        on h.Id = o.HatId
                        WHERE o.id = @id";

            var orders = db.Query<Order, Bird, Hat, Order>(sql, Map, new { id }, splitOn: "Id");

            return orders.FirstOrDefault();
        }

        Order Map(Order order,Bird bird,Hat hat)
        {
            order.Bird = bird;
            order.Hat = hat;
            return order;
        }
}
}
