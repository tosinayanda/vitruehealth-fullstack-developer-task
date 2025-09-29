using Serilog;
using Suggestions.API.Database.Interceptors;
using Suggestions.API.Extensions;

// =======================================================
// ⭐ LOGGING SETUP ⭐
// =======================================================
var configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

var configuration = configurationBuilder.Build();

Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.WithProperty("ApplicationContext", "Suggestions.API")
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .ReadFrom.Configuration(configuration)
        .CreateLogger();

// =======================================================
// ⭐ DI SETUP⭐
// =======================================================
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<AuditingInterceptor>();

builder.Services.AddDbContext<SuggestionsContext>((sp, options) =>
{
    var interceptor = sp.GetRequiredService<AuditingInterceptor>();
    var provider = builder.Configuration["DatabaseProvider"];
    var connectionString = builder.Configuration.GetConnectionString(provider!);

    switch (provider)
    {
        case "SQLite":
            options.UseSqlite(connectionString);
            break;
        case "PostgreSQL":
            options.UseNpgsql(connectionString);
            break;
        default: // Default to InMemory
            options.UseInMemoryDatabase("SuggestionDb");
            break;
    }

    options.AddInterceptors(interceptor);
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Add standard services
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// Add HttpClientFactory for Keycloak communication
builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "Backend API is running!");

app.MapAllEndpoints();

// =======================================================
// ⭐ DATABASE MIGRATION & SEEDING SECTION ⭐
// =======================================================

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<SuggestionsContext>();
        // context.Database.Migrate();

        await DatabaseSeeder.SeedDatabaseAsync(app);

    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
