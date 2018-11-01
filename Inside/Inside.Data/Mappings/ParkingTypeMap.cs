using System;
using System.Collections.Generic;
using System.Text;
using Inside.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inside.Data.Mappings
{
   public class ParkingTypeMap:BaseEntityMap<ParkingType>
    {
        public override void Configure(EntityTypeBuilder<ParkingType> builder)
        {
            builder
                .Property(pt => pt.Type)
                .IsRequired();
        }
    }
}
