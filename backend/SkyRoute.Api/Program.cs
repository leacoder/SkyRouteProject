using System.Text.Json.Serialization;
using SkyRoute.Api;
using SkyRoute.Domain;
using SkyRoute.Domain.Providers;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "frontend";

builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// RFC-9457 ProblemDetails for both framework errors and our domain exceptions.
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<DomainExceptionHandler>();

// Swagger UI at /swagger (Development) for exploring and demoing the API.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options => options.AddPolicy(FrontendCorsPolicy, policy =>
    policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod()));

// Domain / application services.
builder.Services.AddSingleton<AirportCatalog>();
builder.Services.AddSingleton<FlightSearchService>();
builder.Services.AddSingleton<BookingService>();

// Flight providers. Onboarding an airline is exactly ONE line here — nothing else changes (OCP).
builder.Services.AddSingleton<IFlightProvider, GlobalAirProvider>();
builder.Services.AddSingleton<IFlightProvider, BudgetWingsProvider>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(FrontendCorsPolicy);
app.MapControllers();

app.Run();
