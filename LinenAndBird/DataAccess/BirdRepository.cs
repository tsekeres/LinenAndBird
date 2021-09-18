using LinenAndBird.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace LinenAndBird.DataAccess
{
    public class BirdRepository
    {
        const string _connectionString = "Server=localhost;Database=LinenAndBird;Trusted_Connection=True;";
        internal IEnumerable<Bird> GetAll()
        {
            using var connection = new SqlConnection(_connectionString);

            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT *
                                FROM Birds";

            var reader = cmd.ExecuteReader();

            var birds = new List<Bird>();

            while(reader.Read())
            {
                var bird = MapFromReader(reader);

                birds.Add(bird);
            }

            return birds;
        }

        internal void Remove(Guid id)
        {
            using var connection = new SqlConnection(_connectionString);

            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"Delete
                                From Birds
                                Where Id = @id";

            cmd.Parameters.AddWithValue("id", id);

            cmd.ExecuteNonQuery();
        }

        internal Bird Update(Guid id, Bird bird)
        {
            using var connection = new SqlConnection(_connectionString);

            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"update Bird
                                Set Color = @color,
	                                Name = @name,
	                                Type = @type,
	                                Size = @size
                                output inserted.*
                                Where id = @id;";

            cmd.Parameters.AddWithValue("Type", bird.Type);
            cmd.Parameters.AddWithValue("Color", bird.Color);
            cmd.Parameters.AddWithValue("Size", bird.Size);
            cmd.Parameters.AddWithValue("Name", bird.Name);
            cmd.Parameters.AddWithValue("id", id);

            var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return MapFromReader(reader);
            }

            return null;

        }

        internal void Add(Bird newBird)
        {
            using var connection = new SqlConnection(_connectionString);

            connection.Open();

            var cmd = connection.CreateCommand();

            cmd.CommandText = @"insert into birds(Type, Color, Size, Name)
                                output inserted.Id
                                values (@Type,@Color,@Size,@Name)";

            cmd.Parameters.AddWithValue("Type", newBird.Type);
            cmd.Parameters.AddWithValue("Color", newBird.Color);
            cmd.Parameters.AddWithValue("Size", newBird.Size);
            cmd.Parameters.AddWithValue("Name", newBird.Name);

            //execute the query and just return number of rows affected
            var numberOfRowsAffected = cmd.ExecuteNonQuery();

            //execute the query and return the top left most entry (in this case the Guid Id)
            var newId = (Guid) cmd.ExecuteScalar();

            newBird.Id = newId;

        }

        internal Bird GetById(Guid birdId)
        {
            using var connection = new SqlConnection(_connectionString);

            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT *
                                FROM Birds
                                WHERE id = @id";

            cmd.Parameters.AddWithValue("id", birdId);

            var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return MapFromReader(reader);
            }

            return null;

            //return _birds.FirstOrDefault(bird => bird.Id == birdId);
        }

        Bird MapFromReader(SqlDataReader reader)
        {
            //Mapping from the relational model to the object model

            var bird = new Bird();
            bird.Id = reader.GetGuid(0);
            bird.Size = reader["Size"].ToString(); //convert to string to be the right form in our object
            bird.Type = (BirdType)reader["Type"]; //explicit casting to make sure it knows it should be read as is
            bird.Color = reader["Color"].ToString();
            bird.Name = reader["Name"].ToString();

            return bird;
        }
    }
}
