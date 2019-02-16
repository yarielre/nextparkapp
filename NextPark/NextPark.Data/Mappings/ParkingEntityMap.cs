using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NextPark.Domain.Entities;

namespace NextPark.Data.Mappings
{
    public class ParkingEntityMap : BaseEntityMap<Parking>
    {
        public override void Configure(EntityTypeBuilder<Parking> builder)
        {
            builder.HasMany(p => p.Orders)
                .WithOne(o => o.Parking)
                .HasForeignKey(us => us.ParkingId);

            builder.HasMany(p => p.Events)
               .WithOne(o => o.Parking)
               .HasForeignKey(us => us.ParkingId);

            builder.HasOne(p => p.User)
              .WithMany(u => u.Parkings)
              .HasForeignKey(p => p.UserId);
        }
    }
}
