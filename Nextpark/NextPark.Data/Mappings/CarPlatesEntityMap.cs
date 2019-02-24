using NextPark.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NextPark.Data.Mappings
{
    
    public class CarPlatesEntityMap : BaseEntityMap<CarPlate>
    {
        public override void Configure(EntityTypeBuilder<CarPlate> builder)
        {
            builder.HasOne(u => u.User)
              .WithMany(cp => cp.CarPlates)
              .HasForeignKey(ur => ur.UserId);
        }
    }
}
