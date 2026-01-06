using Microsoft.EntityFrameworkCore;
using Payments.Infra.Persistence;
using Reqnroll;

namespace Payments.Tests.Bdd.Hooks;

[Binding]
public class DatabaseHooks
{
    private readonly AppDbContext _context;

    public DatabaseHooks(AppDbContext context)
    {
        _context = context;
    }

    [BeforeScenario]
    public async Task BeforeScenario()
    {
        // Garante que o banco de dados está criado com as migrations aplicadas
        await _context.Database.MigrateAsync();
    }

    [AfterScenario]
    public async Task AfterScenario()
    {
        // Limpa os dados após cada cenário para manter isolamento
        if (_context.Database.IsSqlServer())
        {
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM Payments");
        }
    }
}

