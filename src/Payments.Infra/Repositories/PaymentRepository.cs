using Microsoft.EntityFrameworkCore;
using Payments.Application.Interfaces;
using Payments.Domain.Entities;
using Payments.Infra.Persistence;

namespace Payments.Infra.Repositories;

public sealed class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        await _context.Payments.AddAsync(payment, cancellationToken);
    }

    public async Task<Payment?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Payments.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Payment?> GetByUuidAsync(Guid uuid, CancellationToken cancellationToken = default)
    {
        return await _context.Payments.FirstOrDefaultAsync(p => p.Uuid == uuid, cancellationToken);
    }

    public async Task<Payment?> GetByOrderIdAsync(string orderId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);
    }

    public async Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        _context.Payments.Update(payment);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
