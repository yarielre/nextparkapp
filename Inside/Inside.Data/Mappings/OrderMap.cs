using System;
using System.Collections.Generic;
using System.Text;
using Inside.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inside.Data.Mappings
{
   public class OrderMap:BaseEntityMap<Order>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            builder
                .Property(o => o.Date)
                .IsRequired();
            builder
                .Property(o => o.StartTime)
                .IsRequired();
            builder
                .Property(o => o.EndTime)
                .IsRequired();
            builder
                .Property(o => o.Price)
                .IsRequired();

            builder.HasOne(o => o.Parking)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.ParkingId)
                .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade);
            builder.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
