
using Inside.Domain.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inside.Data.Mappings
{
    public abstract class BaseEntityMap<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity: class, IBaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder
                .HasKey(be => be.Id);
            builder
                .Property(be => be.Id)
                .ValueGeneratedOnAdd();
        }
    }
}