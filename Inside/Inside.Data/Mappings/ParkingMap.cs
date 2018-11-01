using Inside.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inside.Data.Mappings
{
    public class ParkingMap : BaseEntityMap<Parking>
    {
        public override void Configure(EntityTypeBuilder<Parking> builder)
        {
            builder
                .Property(p => p.Latitude)
                .IsRequired();
            builder
                .Property(p => p.Longitude)
                .IsRequired();
            builder
                .Property(p => p.IsRented)
                .IsRequired();
            builder
                .HasOne(p => p.ParkingCategory)
                .WithMany(pc => pc.Parkings)
                .HasForeignKey(p => p.ParkingCategoryId);

            builder
                .HasOne(p => p.ParkingType)
                .WithMany(pc => pc.Parkings)
                .HasForeignKey(p => p.ParkingTypeId);

            builder
                .HasOne(p => p.User)
                .WithMany(u => u.Parkings)
                .HasForeignKey(p => p.UserId);

            builder.HasOne(p => p.ParkingEvent)
                .WithMany()
                .HasForeignKey(p => p.ParkingEventId);
        }
    }
}