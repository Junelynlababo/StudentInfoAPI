using Microsoft.EntityFrameworkCore;
using Npgsql;
using StudentApi.Data;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration["DefaultConnection"]
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
connectionString = NormalizePostgresConnectionString(connectionString);
builder.Services.AddDbContext<StudentDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("GitHubPages", policy =>
    {
        policy.WithOrigins("https://junelynlababo.github.io")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("GitHubPages");

app.UseAuthorization();

app.MapControllers();

app.Run();

static string NormalizePostgresConnectionString(string connectionString)
{
    if (!Uri.TryCreate(connectionString, UriKind.Absolute, out var uri)
        || (uri.Scheme != "postgres" && uri.Scheme != "postgresql"))
    {
        return connectionString;
    }

    var credentials = uri.UserInfo.Split(':', 2);
    var builder = new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port > 0 ? uri.Port : 5432,
        Database = Uri.UnescapeDataString(uri.AbsolutePath.Trim('/')),
        Username = credentials.Length > 0 ? Uri.UnescapeDataString(credentials[0]) : string.Empty
    };

    if (credentials.Length > 1)
    {
        builder.Password = Uri.UnescapeDataString(credentials[1]);
    }

    foreach (var parameter in uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
    {
        var parts = parameter.Split('=', 2);
        var key = Uri.UnescapeDataString(parts[0]).ToLowerInvariant();
        var value = parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : string.Empty;

        switch (key)
        {
            case "sslmode":
                builder["SSL Mode"] = value;
                break;
            case "channel_binding":
                builder["Channel Binding"] = value;
                break;
            case "application_name":
                builder["Application Name"] = value;
                break;
            default:
                throw new InvalidOperationException($"Unsupported PostgreSQL URL parameter '{key}'.");
        }
    }

    return builder.ConnectionString;
}
