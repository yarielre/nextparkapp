using Inside.Data.Infrastructure;
using Inside.Domain.Entities;

namespace Inside.Data.Repositories
{
    public class ParkingCategoryRepository : BaseRepository<ParkingCategory>
    {
        public ParkingCategoryRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}