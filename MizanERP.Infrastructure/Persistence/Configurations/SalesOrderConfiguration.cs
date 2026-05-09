using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MizanERP.Domain.Entities;

namespace MizanERP.Infrastructure.Persistence.Configurations
{
    public class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrder>
    {
        public void Configure(EntityTypeBuilder<SalesOrder> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.OrderDate).IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Property(x => x.IsDeleted).IsRequired();
            builder.HasMany(typeof(SalesOrderLine), "Lines").WithOne().HasForeignKey("SalesOrderId");
        }
    }

    public class SalesOrderLineConfiguration : IEntityTypeConfiguration<SalesOrderLine>
    {
        public void Configure(EntityTypeBuilder<SalesOrderLine> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.SalesOrderId).IsRequired();
            builder.Property(x => x.ProductId).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Property(x => x.IsDeleted).IsRequired();
            builder.OwnsOne(x => x.Quantity, q =>
            {
                q.Property(p => p.Value).IsRequired();
                q.Property(p => p.Unit).IsRequired().HasMaxLength(50);
            });
            builder.OwnsOne(x => x.Price, p =>
            {
                p.Property(pp => pp.Amount).HasColumnType("decimal(18,2)");
                p.Property(pp => pp.Currency).HasMaxLength(10);
            });
        }
    }
}