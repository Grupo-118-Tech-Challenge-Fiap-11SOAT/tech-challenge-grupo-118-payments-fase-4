using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payments.Domain.Entities;

namespace Payments.Infra.Persistence.Configurations;

[ExcludeFromCodeCoverage]
public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.OrderId)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(p => p.OrderId)
            .IsUnique();

        builder.OwnsOne(p => p.Value, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Value")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });

        builder.Property(p => p.Provider)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.Uuid)
            .IsRequired();

        builder.HasIndex(p => p.Uuid)
            .IsUnique();

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(p => p.UserPaymentCode)
            .HasMaxLength(500);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt);
    }
}
