using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MizanERP.Domain.Entities;

namespace MizanERP.Infrastructure.Persistence.Configurations
{
    public class AccountingEntryConfiguration : IEntityTypeConfiguration<AccountingEntry>
    {
        public void Configure(EntityTypeBuilder<AccountingEntry> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Date).IsRequired();
            builder.Property(x => x.AccountId).IsRequired();
            builder.Property(x => x.Debit).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.Credit).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.Reference).HasMaxLength(100);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Property(x => x.IsDeleted).IsRequired();
        }
    }
}