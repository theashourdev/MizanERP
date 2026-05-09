using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MizanERP.Domain.Entities;
using MizanERP.Domain.ValueObjects;

namespace MizanERP.Infrastructure.Persistence.Configurations
{
    public class ProductionOrderConfiguration : IEntityTypeConfiguration<ProductionOrder>
    {
        public void Configure(EntityTypeBuilder<ProductionOrder> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ProductId).IsRequired();
            builder.Property(x => x.OrderDate).IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Property(x => x.IsDeleted).IsRequired();
            builder.HasMany(typeof(ProductionOrderLine), "Lines").WithOne().HasForeignKey("ProductionOrderId");
            builder.OwnsOne(x => x.Quantity, q =>
            {
                q.Property(p => p.Value).IsRequired();
                q.Property(p => p.Unit).IsRequired().HasMaxLength(50);
            });
        }
    }

    public class ProductionOrderLineConfiguration : IEntityTypeConfiguration<ProductionOrderLine>
    {
        public void Configure(EntityTypeBuilder<ProductionOrderLine> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ProductionOrderId).IsRequired();
            builder.Property(x => x.RawMaterialProductId).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Property(x => x.IsDeleted).IsRequired();
            builder.OwnsOne(x => x.Quantity, q =>
            {
                q.Property(p => p.Value).IsRequired();
                q.Property(p => p.Unit).IsRequired().HasMaxLength(50);
            });
        }
    }
}