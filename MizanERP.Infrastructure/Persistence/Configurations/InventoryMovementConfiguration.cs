using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MizanERP.Domain.Entities;

namespace MizanERP.Infrastructure.Persistence.Configurations
{
    public class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
    {
        public void Configure(EntityTypeBuilder<InventoryMovement> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ProductId).IsRequired();
            builder.Property(x => x.MovementType).IsRequired();
            builder.Property(x => x.Date).IsRequired();
            builder.Property(x => x.Reference).HasMaxLength(100);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.IsDeleted).IsRequired();
            builder.OwnsOne(x => x.Quantity, q =>
            {
                q.Property(p => p.Value).IsRequired();
                q.Property(p => p.Unit).IsRequired().HasMaxLength(50);
            });
            builder.OwnsOne(x => x.Cost, c =>
            {
                c.Property(p => p.Amount).HasColumnType("decimal(18,2)");
                c.Property(p => p.Currency).HasMaxLength(10);
            });
        }
    }
}