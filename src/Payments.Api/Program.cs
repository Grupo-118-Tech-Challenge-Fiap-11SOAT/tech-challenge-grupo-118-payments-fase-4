using Microsoft.EntityFrameworkCore;
using Payments.Api.Endpoints;
using Payments.Api.Handlers;
using Payments.Application.Interfaces;
using Payments.Application.UseCases;
using Payments.Infra.Configuration;
using Payments.Infra.ExternalServices.MercadoPago;
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

// HTTP Client for MercadoPago
builder.Services.AddHttpClient<MercadoPagoPaymentProviderService>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var options = configuration.GetSection(MercadoPagoOptions.SectionName).Get<MercadoPagoOptions>()!;

    client.BaseAddress = new Uri(options.BaseUrl);
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.AccessToken}");
});

// Application Services
builder.Services.AddScoped<CreatePaymentUseCase>();

// Infrastructure Services
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentProviderFactory, PaymentProviderFactory>();

var app = builder.Build();

app.UseExceptionHandler();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(s =>
    {
        s.SwaggerEndpoint("../swagger/v1/swagger.json", "Tech Challenge - Payments API");
        s.RoutePrefix = string.Empty;
        s.DocumentTitle = "Tech Challenge - Payments API - Fase 4 | Swagger";
    });
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
   .WithName("Health").WithTags("Health");

app.MapPaymentsEndpoints();

app.Run();