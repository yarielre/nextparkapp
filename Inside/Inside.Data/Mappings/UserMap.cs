using System;
using System.Collections.Generic;
using System.Text;
using Inside.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inside.Data.Mappings
{
    public class UserMap:BaseEntityMap<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.UserName)
                .IsRequired();
            builder.Property(u => u.Email)
                .IsRequired();
            builder.Property(u => u.PasswordHash)
                .IsRequired();
        }
    }
}
