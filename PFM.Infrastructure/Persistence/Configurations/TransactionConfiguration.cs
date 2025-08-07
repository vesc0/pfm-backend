using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PFM.Domain.Entities;
using PFM.Domain.Enums;

namespace PFM.Infrastructure.Persistence.Configurations
{
    public class TransactionConfiguration
        : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> b)
        {
            b.ToTable("transactions");

            b.HasKey(t => t.Id);
            b.Property(t => t.Id)
             .HasColumnName("id")
             .HasMaxLength(50);

            b.Property(t => t.BeneficiaryName)
             .HasColumnName("beneficiary_name");

            b.Property(t => t.Date)
             .HasColumnName("date")
             .HasColumnType("date")
             .IsRequired();

            // map enum → “d”/“c” and back
            var dirConverter = new ValueConverter<TransactionDirectionEnum, string>(
                v => v == TransactionDirectionEnum.d ? "d" : "c",
                v => v == "d" ? TransactionDirectionEnum.d : TransactionDirectionEnum.c
            );
            b.Property(t => t.Direction)
             .HasConversion(dirConverter)
             .HasColumnName("direction")
             .HasMaxLength(1)
             .IsRequired();

            b.Property(t => t.Amount)
             .HasColumnName("amount")
             .HasColumnType("numeric(18,2)")
             .IsRequired();

            b.Property(t => t.Description)
             .HasColumnName("description");

            b.Property(t => t.Currency)
             .HasColumnName("currency")
             .HasMaxLength(3)
             .IsRequired();

            b.Property(t => t.Mcc)
             .HasColumnName("mcc");

            // TransactionKind maps to its 3-letter name
            var kindConverter = new EnumToStringConverter<TransactionKindEnum>();
            b.Property(t => t.Kind)
             .HasConversion(kindConverter)
             .HasColumnName("kind")
             .HasMaxLength(3)
             .IsRequired();

            b.Property(t => t.CatCode)
             .HasColumnName("cat_code")
             .HasMaxLength(15);

            b.HasMany(t => t.Splits)
             .WithOne(s => s.Transaction)
             .HasForeignKey(s => s.TransactionId);

        }
    }
}
