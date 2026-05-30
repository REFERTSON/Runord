using Serilog;
using Runord.Hub.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Настройка логирования через Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// =========================================================================
// REGISTRATION (DI Container)
// =========================================================================
builder.Services
    .AddAppConfigurations(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddRepositories()
    .AddApplicationServices()
    .AddJwtAuthentication(builder.Configuration)
    .AddApiDocumentation();

var app = builder.Build();

// =========================================================================
// PIPELINE (Middlewares & Endpoints)
// =========================================================================
// Настройка HTTP-конвейера
if (app.Environment.IsDevelopment())
{
    // Наш исправленный метод сам вызовет внутри и MapOpenApi(), и MapScalarApiReference()
    app.UseApiDocumentationUi();
}

app.UseHttpsRedirection();
app.UseCors(); // Корректное расположение CORS

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseSerilogRequestLogging();

// Запуск сидинга базы данных
await DatabaseInitializer.SeedDatabaseAsync(app.Services);

app.Run();