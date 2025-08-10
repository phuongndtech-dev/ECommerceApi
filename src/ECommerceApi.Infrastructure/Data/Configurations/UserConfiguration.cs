using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.Enums;

namespace ECommerceApi.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Table name
            builder.ToTable("Users");

            // Primary key
            builder.HasKey(u => u.Id);

            // Properties
            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Role)
                .IsRequired()
                .HasConversion<int>(); // Store enum as int

            builder.Property(u => u.IsEmailConfirmed)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(u => u.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(u => u.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(u => u.LastLoginAt)
                .IsRequired(false);

            // Indexes
            builder.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");

            builder.HasIndex(u => u.CreatedAt)
                .HasDatabaseName("IX_Users_CreatedAt");

            // Relationships
            builder.HasMany(u => u.Products)
                .WithOne(p => p.CreatedBy)
                .HasForeignKey(p => p.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}