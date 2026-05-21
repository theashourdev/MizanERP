using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MizanERP.Domain.Entities;

namespace MizanERP.Infrastructure.Persistence.Configurations
{
    public class CapitalTransactionConfiguration : IEntityTypeConfiguration<CapitalTransaction>
    {
        public void Configure(EntityTypeBuilder<CapitalTransaction> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Date).IsRequired();
            builder.Property(x => x.Amount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.BalanceBefore).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.BalanceAfter).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.Currency).HasMaxLength(10).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.Property(x => x.IsDeleted).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            // Store enum as int
            builder.Property(x => x.Type).HasConversion<int>().IsRequired();

            // Optional FK to PurchaseOrder is not enforced here (just metadata)
        }
    }
}
