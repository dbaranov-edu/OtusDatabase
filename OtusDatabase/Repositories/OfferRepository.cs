using OtusDatabase.Entities;
using Dapper;
using Npgsql;

namespace OtusDatabase.Repositories
{
    internal class OfferRepository : IRepository<Offer>
    {
        private readonly string _connectionString = null!;

        public OfferRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Offer>> GetAllAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Offer>(
                @$"SELECT 
                     id AS {nameof(Offer.Id)}, 
                     user_id AS {nameof(Offer.UserId)}, 
                     hidden AS {nameof(Offer.Hidden)}, 
                     price AS {nameof(Offer.Price)}, 
                     title AS {nameof(Offer.Title)}, 
                     description AS {nameof(Offer.Description)}, 
                     created_at AS {nameof(Offer.CreatedAt)}, 
                     updated_at AS {nameof(Offer.UpdatedAt)} 
                   FROM offers");
        }

        public async Task<Offer> GetAsync(long id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Offer>(
                @$"SELECT 
                     id AS {nameof(Offer.Id)}, 
                     user_id AS {nameof(Offer.UserId)}, 
                     hidden AS {nameof(Offer.Hidden)}, 
                     price AS {nameof(Offer.Price)}, 
                     title AS {nameof(Offer.Title)}, 
                     description AS {nameof(Offer.Description)}, 
                     created_at AS {nameof(Offer.CreatedAt)}, 
                     updated_at AS {nameof(Offer.UpdatedAt)} 
                   FROM offers 
                   WHERE id = @OfferId",
                new { OfferId = id });
        }

        public async Task<long> AddAsync(Offer entity)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstAsync<long>(
                @$"INSERT INTO offers(user_id, price, title, description) 
                   VALUES(@UserId, @Price, @Title, @Description) 
                   RETURNING id",
                entity);
        }

        public async Task<int> UpdateAsync(Offer entity)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.ExecuteAsync(
                $@"UPDATE offers SET 
                     hidden = @Hidden, 
                     price = @Price, 
                     title = @Title, 
                     description = @Description, 
                     updated_at = 'now()' 
                   WHERE id = @UpdateId",
                new
                {
                    UpdateId = entity.Id,
                    entity.UserId,
                    entity.Hidden,
                    entity.Price,
                    entity.Title,
                    entity.Description
                });
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.ExecuteAsync(
                "DELETE FROM offers WHERE id = @DeleteId",
                new { DeleteId = id }) == 1;
        }
    }
}
