using NextPark.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NextPark.Data.Mappings
{
    
    public class EventsEntityMap : BaseEntityMap<Event>
    {
        public override void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasOne(ev => ev.Parking)
              .WithMany(u => u.Events)
              .HasForeignKey(p => p.ParkingId);

        }
    }
}
