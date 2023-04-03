using OtusDatabase.Repositories;

namespace OtusDatabase.Entities
{
    public class Offer : IEntity
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public bool Hidden { get; set; }
        public decimal Price { get; set; }
        public string Title { get; set; } = null!; 
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public override string ToString()
        {
            return $"Offer[Id={Id},UserId={UserId},Hidden={Hidden},Price='{Price}',Title='{Title}',Description='{Description}',CreatedAt='{CreatedAt}',UpdatedAt='{UpdatedAt?.ToString() ?? "N/A"}']";
        }
    }
}
