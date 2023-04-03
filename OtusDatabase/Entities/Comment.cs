using OtusDatabase.Repositories;

namespace OtusDatabase.Entities
{
    public class Comment : IEntity
    {
        public long Id { get; set; }
        public long OfferId { get; set; }
        public long UserId { get; set; }
        public bool Hidden { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public override string ToString()
        {
            return $"Comment[Id={Id},OfferId={OfferId},UserId={UserId},Hidden={Hidden},Content='{Content}',CreatedAt='{CreatedAt}',UpdatedAt='{UpdatedAt?.ToString() ?? "N/A"}']";
        }
    }
}
