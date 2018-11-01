using System;
using System.Collections.Generic;
using System.Text;
using Inside.Data.Infrastructure;
using Inside.Domain.Entities;

namespace Inside.Data.Repositories
{
   public class ParkingRepository : BaseRepository<Parking>
    {
        public ParkingRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
