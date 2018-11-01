using Inside.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inside.Data.Mappings
{
    public class ParkingCategoryMap : BaseEntityMap<ParkingCategory>
    {
        public override void Configure(EntityTypeBuilder<ParkingCategory> builder)
        {
            builder
                .Property(pc => pc.Category)
                .IsRequired();
            builder
                 .Property(pc => pc.Price)
                 .IsRequired();
            builder
                 .Property(pc => pc.CoinPrice)
                 .IsRequired();
        }
    }
}