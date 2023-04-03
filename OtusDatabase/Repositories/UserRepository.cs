using OtusDatabase.Entities;
using Dapper;
using Npgsql;

namespace OtusDatabase.Repositories
{
    public class UserRepository : IRepository<User>
    {
        private readonly string _connectionString = null!;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<User>(
                @$"SELECT 
                     id AS {nameof(User.Id)}, 
                     firstname AS {nameof(User.FirstName)}, 
                     lastname AS {nameof(User.LastName)}, 
                     email AS {nameof(User.Email)}, 
                     birthdate AS {nameof(User.BirthDate)}, 
                     created_at AS {nameof(Offer.CreatedAt)}, 
                     updated_at AS {nameof(Offer.UpdatedAt)} 
                   FROM users");
        }

        public async Task<User> GetAsync(long id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<User>(
                @$"SELECT 
                     id AS {nameof(User.Id)}, 
                     firstname AS {nameof(User.FirstName)}, 
                     lastname AS {nameof(User.LastName)}, 
                     email AS {nameof(User.Email)}, 
                     birthdate AS {nameof(User.BirthDate)}, 
                     created_at AS {nameof(Offer.CreatedAt)}, 
                     updated_at AS {nameof(Offer.UpdatedAt)} 
                   FROM users 
                   WHERE id = @UserId",
                new { UserId = id });
        }

        public async Task<long> AddAsync(User entity)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstAsync<long>(
                $@"INSERT INTO users(firstname, lastname, email, birthdate) 
                   VALUES(@FirstName, @LastName, @Email, @BirthDate) 
                   RETURNING id",
                entity);
        }

        public async Task<int> UpdateAsync(User entity)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.ExecuteAsync(
                $@"UPDATE users SET 
                     firstname = @FirstName, 
                     lastname = @LastName, 
                     email = @Email, 
                     birthdate = @BirthDate, 
                     updated_at = 'now()' 
                   WHERE id = @UpdateId",
                new
                {
                    UpdateId = entity.Id,
                    entity.FirstName,
                    entity.LastName,
                    entity.Email,
                    entity.BirthDate
                });
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.ExecuteAsync(
                "DELETE FROM users WHERE id = @DeleteId",
                new { DeleteId = id }) == 1;
        }
    }
}
