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
    app.UseApiDocumentationUi();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseSerilogRequestLogging();

// Запуск сидинга базы данных
await DatabaseInitializer.SeedDatabaseAsync(app.Services);

app.Run();