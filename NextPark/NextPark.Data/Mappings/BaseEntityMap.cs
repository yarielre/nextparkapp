using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NextPark.Domain.Core;

namespace NextPark.Data.Mappings
{
    public abstract class BaseEntityMap<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : class, IBaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(baseEntity => baseEntity.Id);
            builder
                .Property(baseEntity => baseEntity.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
