using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFM.Domain.Entities;

namespace PFM.Infrastructure.Persistence.Configurations
{
    public class TransactionSplitConfiguration : IEntityTypeConfiguration<TransactionSplit>
    {
        public void Configure(EntityTypeBuilder<TransactionSplit> builder)
        {
            builder.ToTable("transaction_splits");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .HasColumnName("id")
                   .IsRequired();

            builder.Property(x => x.Amount)
                   .HasColumnName("amount")
                   .HasColumnType("numeric(18,2)")
                   .IsRequired();

            builder.Property(x => x.CatCode)
                   .HasColumnName("cat_code")
                   .HasMaxLength(15)
                   .IsRequired();

            builder.Property(x => x.TransactionId)
                   .HasColumnName("transaction_id")
                   .HasMaxLength(50)
                   .IsRequired();

            // FK to Transaction
            builder.HasOne(x => x.Transaction)
                   .WithMany(t => t.Splits)
                   .HasForeignKey(x => x.TransactionId)
                   .OnDelete(DeleteBehavior.Cascade);

            // FK to Category (CatCode)
            builder.HasOne(x => x.Category)
                   .WithMany()
                   .HasForeignKey(x => x.CatCode)
                   .HasPrincipalKey(c => c.Code)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
