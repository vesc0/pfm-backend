using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFM.Domain.Entities;

namespace PFM.Infrastructure.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("categories");

            // Primary key
            builder.HasKey(c => c.Code);
            builder.Property(c => c.Code)
                   .HasColumnName("code")
                   .HasMaxLength(50)
                   .IsRequired();

            // Name
            builder.Property(c => c.Name)
                   .HasColumnName("name")
                   .IsRequired();

            // Parent code FK
            builder.Property(c => c.ParentCode)
                   .HasColumnName("parent_code");

            builder.HasOne(c => c.Parent)
                   .WithMany(c => c.Children)
                   .HasForeignKey(c => c.ParentCode)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}