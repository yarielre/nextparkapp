using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NextPark.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NextPark.Data.Mappings
{
  public class ScheduleEntityMap : BaseEntityMap<Schedule>
    {
        public override void Configure(EntityTypeBuilder<Schedule> builder)
        {
            builder.Property(sch => sch.ScheduleId)
                .IsRequired();
            builder.Property(sch => sch.ScheduleType)
              .IsRequired();
            builder.Property(sch => sch.TimeOfExecution)
            .IsRequired();
        }
    }
}
