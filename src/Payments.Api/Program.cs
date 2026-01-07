using Microsoft.EntityFrameworkCore;
using Payments.Api.Endpoints;
using Payments.Api.Handlers;
using Payments.Application.Events;
using Payments.Application.Interfaces;
using Payments.Application.UseCases;
using Payments.Infra.Configuration;
using Payments.Infra.Events;
using Payments.Infra.Events.Handlers;
using Payments.Infra.ExternalServices.MercadoPago;
using Payments.Infra.ExternalServices.Orders;
using Payments.Infra.Factories;
using Payments.Infra.Persistence;
using Payments.Infra.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// MercadoPago Configuration
builder.Services.Configure<MercadoPagoOptions>(
    builder.Configuration.GetSection(MercadoPagoOptions.SectionName));

// OrdersApi Configuration
builder.Services.Configure<OrdersApiOptions>(
    builder.Configuration.GetSection(OrdersApiOptions.SectionName));

// HTTP Client for MercadoPago
builder.Services.AddHttpClient<MercadoPagoPaymentProviderService>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var options = configuration.GetSection(MercadoPagoOptions.SectionName).Get<MercadoPagoOptions>()!;

    client.BaseAddress = new Uri(options.BaseUrl);
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.AccessToken}");
});

// HTTP Client for OrdersApi
builder.Services.AddHttpClient<IOrdersApiClient, OrdersApiClient>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var options = configuration.GetSection(OrdersApiOptions.SectionName).Get<OrdersApiOptions>()!;

    client.BaseAddress = new Uri(options.BaseUrl);
});

// Application Services
builder.Services.AddScoped<CreatePaymentUseCase>();
builder.Services.AddScoped<ConfirmPaymentUseCase>();

// Infrastructure Services
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentProviderFactory, PaymentProviderFactory>();
builder.Services.AddScoped<IEventDispatcher, InMemoryEventDispatcher>();
builder.Services.AddScoped<IEventHandler<PaymentApprovedEvent>, NotifyOrdersServiceHandler>();

var app = builder.Build();

app.UseExceptionHandler();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.UseSwagger();
app.UseSwaggerUI(s =>
{
    s.SwaggerEndpoint("../swagger/v1/swagger.json", "Tech Challenge - Payments API");
    s.RoutePrefix = string.Empty;
    s.DocumentTitle = "Tech Challenge - Payments API - Fase 4 | Swagger";
});

app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
   .WithName("Health").WithTags("Health");

app.MapPaymentsEndpoints();
app.MapMercadoPagoWebhookEndpoints();

app.Run();