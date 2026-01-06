using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Payments.Domain.Entities;
using Payments.Infra.Persistence.Configurations;

namespace Payments.Infra.Persistence;

[ExcludeFromCodeCoverage]
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
    }
}