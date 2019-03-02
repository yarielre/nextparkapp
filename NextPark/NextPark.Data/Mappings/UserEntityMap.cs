using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NextPark.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NextPark.Data.Mappings
{
 
    public class UserEntityMap : BaseEntityMap<ApplicationUser>
    {
        public override void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasMany(u => u.Orders)
               .WithOne(uo => uo.User)
               .HasForeignKey(uo => uo.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Parkings)
               .WithOne(uo => uo.User)
               .HasForeignKey(uo => uo.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Transactions)
                .WithOne(t=>t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
