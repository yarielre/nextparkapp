using NextPark.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NextPark.Data.Mappings
{
    public class OrdersEntityMap : BaseEntityMap<Order>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasOne(or => or.Parking)
              .WithMany(u => u.Orders)
              .HasForeignKey(p => p.ParkingId);

            builder.HasOne(p => p.User)
             .WithMany(u => u.Orders)
             .HasForeignKey(p => p.UserId);
        }
    }
}

