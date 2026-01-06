using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payments.Application.Events;
using Payments.Application.Interfaces;
using Payments.Application.UseCases;
using Payments.Infra.Events;
using Payments.Infra.Events.Handlers;
using Payments.Infra.Persistence;
using Payments.Infra.Repositories;
using Payments.Tests.Bdd.Context;
using Payments.Tests.Bdd.Factories;
using Reqnroll;
using Reqnroll.Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace Payments.Tests.Bdd;

[Binding]
public class SetupTestDependencies
{
    private static MsSqlContainer? _msSqlContainer;
    private static string? _connectionString;

    [BeforeTestRun]
    public static async Task BeforeTestRun()
    {
        // Inicializa o container SQL Server antes de todos os testes
        _msSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("Strong_Password_123!")
            .WithCleanUp(true)
            .Build();

        await _msSqlContainer.StartAsync();
        _connectionString = _msSqlContainer.GetConnectionString();
    }

    [ScenarioDependencies]
    public static IServiceCollection CreateServices()
    {
        var services = new ServiceCollection();

        // Configuration properties.
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"ConnectionStrings:DefaultConnection", _connectionString},
                {"MercadoPago:AccessToken", "test-token"},
                {"MercadoPago:UserId", "test-user-id"},
                {"MercadoPago:PosId", "test-pos-id"},
                {"MercadoPago:BaseUrl", "https://api.mercadopago.com"},
                {"MercadoPago:NotificationUrl", "https://test.com/webhook"}
            })
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // Database - usando SQL Server via Testcontainers
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(_connectionString);
            options.EnableSensitiveDataLogging();
        });

        // Use Cases
        services.AddScoped<CreatePaymentUseCase>();
        services.AddScoped<ConfirmPaymentUseCase>();

        // Repositories
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        // Payment Provider - MOCK para não fazer requisições reais
        services.AddScoped<IPaymentProviderFactory, MockPaymentProviderFactory>();

        // Event System
        services.AddScoped<IEventDispatcher, InMemoryEventDispatcher>();
        services.AddScoped<IEventHandler<PaymentApprovedEvent>, NotifyOrdersServiceHandler>();

        // Scenario Context para compartilhar dados entre steps
        services.AddScoped<PaymentScenarioContext>();

        return services;
    }

    [AfterTestRun]
    public static async Task AfterTestRun()
    {
        // Limpa o container após todos os testes
        if (_msSqlContainer != null)
        {
            await _msSqlContainer.DisposeAsync();
            _msSqlContainer = null;
        }
    }
}