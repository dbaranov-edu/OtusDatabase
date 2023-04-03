using OtusDatabase.Repositories;

namespace OtusDatabase.Entities
{
    public class User : IEntity
    {
        public long Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime? BirthDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public override string ToString()
        {
            return $"User[Id={Id},FirstName='{FirstName}',LastName='{LastName}',Email='{Email}',BirthDate='{BirthDate?.ToShortDateString() ?? "N/A"}',CreatedAt='{CreatedAt}',UpdatedAt='{UpdatedAt?.ToString() ?? "N/A"}']";
        }
    }
}
