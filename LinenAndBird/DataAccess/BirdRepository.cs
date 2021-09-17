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
        static List<Bird> _birds = new List<Bird>
        {
            new Bird
            {
                Id = Guid.NewGuid(),
                Name = "Jimmy",
                Color = "Red",
                Size = "Small",
                Type = BirdType.Dead,
                Accessories = new List<string> { "Beanie", "gold wing tips" }
            }
        };

        internal IEnumerable<Bird> GetAll()
        {
            using var connection = new SqlConnection("Server=localhost;Database=LinenAndBird;Trusted_Connection=True;");

            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT *
                                FROM Birds";

            var reader = cmd.ExecuteReader();

            var birds = new List<Bird>();

            while(reader.Read())
            {
                //Mapping from the relational model to the object model
                var bird = new Bird();
                bird.Id = reader.GetGuid(0);
                bird.Size = reader["Size"].ToString(); //convert to string to be the right form in our object
                bird.Type = (BirdType)reader["Type"]; //explicit casting to make sure it knows it should be read as is
                bird.Color = reader["Color"].ToString();
                bird.Name = reader["Name"].ToString();

                birds.Add(bird);
            }

            return birds;
        }

        internal void Add(Bird newBird)
        {
            newBird.Id = Guid.NewGuid();

            _birds.Add(newBird);
        }

        internal Bird GetById(Guid birdId)
        {
            using var connection = new SqlConnection("Server=localhost;Database=LinenAndBird;Trusted_Connection=True;");

            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT *
                                FROM Birds
                                WHERE id = @id";

            cmd.Parameters.AddWithValue("id", birdId);

            var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                var bird = new Bird();
                bird.Id = reader.GetGuid(0);
                bird.Size = reader["Size"].ToString(); //convert to string to be the right form in our object
                bird.Type = (BirdType)reader["Type"]; //explicit casting to make sure it knows it should be read as is
                bird.Color = reader["Color"].ToString();
                bird.Name = reader["Name"].ToString();

                return bird;
            }

            return null;

            //return _birds.FirstOrDefault(bird => bird.Id == birdId);
        }
    }
}
