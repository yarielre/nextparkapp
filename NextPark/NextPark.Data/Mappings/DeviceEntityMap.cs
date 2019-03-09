using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NextPark.Domain.Entities;

namespace NextPark.Data.Mappings
{
    public class DeviceEntityMap:BaseEntityMap<Device>
    {
        public override void Configure(EntityTypeBuilder<Device> builder)
        {
            builder.Property(d => d.DeviceIdentifier)
                .IsRequired();
            builder.Property(d => d.Platform)
                .IsRequired();
        }
    }
}