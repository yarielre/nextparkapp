using System;
using System.Collections.Generic;
using System.Text;
using Inside.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inside.Data.Mappings
{
   public class EventMap:BaseEntityMap<Event>
    {
        public override void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.Property(e => e.StartTime)
                .IsRequired();
            builder.Property(e => e.EndTime)
                .IsRequired();
            builder.Property(e => e.MonthRepeat)
                .IsRequired(false);
            builder.Property(e => e.WeekRepeat)
                .IsRequired(false);
        }
    }
}
