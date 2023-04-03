using OtusDatabase.Entities;
using Dapper;
using Npgsql;

namespace OtusDatabase.Repositories
{
    internal class CommentRepository : IRepository<Comment>
    {
        private readonly string _connectionString = null!;

        public CommentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Comment>> GetAllAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Comment>(
                @$"SELECT 
                     id AS {nameof(Comment.Id)}, 
                     offer_id AS {nameof(Comment.OfferId)}, 
                     user_id AS {nameof(Comment.UserId)}, 
                     hidden AS {nameof(Comment.Hidden)}, 
                     content AS {nameof(Comment.Content)}, 
                     created_at AS {nameof(Comment.CreatedAt)}, 
                     updated_at AS {nameof(Comment.UpdatedAt)} 
                   FROM comments");
        }

        public async Task<Comment> GetAsync(long id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Comment>(
                @$"SELECT 
                     id AS {nameof(Comment.Id)}, 
                     offer_id AS {nameof(Comment.OfferId)}, 
                     user_id AS {nameof(Comment.UserId)}, 
                     hidden AS {nameof(Comment.Hidden)}, 
                     content AS {nameof(Comment.Content)}, 
                     created_at AS {nameof(Comment.CreatedAt)}, 
                     updated_at AS {nameof(Comment.UpdatedAt)} 
                   FROM comments 
                   WHERE id = @CommentId",
                new { CommentId = id });
        }

        public async Task<long> AddAsync(Comment entity)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstAsync<long>(
                @$"INSERT INTO comments(offer_id, user_id, content) 
                   VALUES(@OfferId, @UserId, @Content) 
                   RETURNING id",
                entity);
        }

        public async Task<int> UpdateAsync(Comment entity)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.ExecuteAsync(
                $@"UPDATE offers SET 
                     hidden = @Hidden, 
                     content = @Content, 
                     updated_at = 'now()' 
                   WHERE id = @UpdateId",
                new
                {
                    UpdateId = entity.Id,
                    entity.Hidden,
                    entity.Content
                });
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.ExecuteAsync(
                "DELETE FROM comments WHERE id = @DeleteId",
                new { DeleteId = id }) == 1;
        }
    }
}
