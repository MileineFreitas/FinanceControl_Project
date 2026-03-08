using System.Text.Json.Serialization;
using FinanceControl.Infrastructure.Contexts;
using FinanceControl.Infrastructure.Repositories.Users;
using FinanceControl.Infrastructure.Repositories.Categories;
using FinanceControl.Infrastructure.Repositories.Transactions;
using FinanceControl.Domain.Interfaces.Repositories.Users;
using FinanceControl.Domain.Interfaces.Repositories.Categories;
using FinanceControl.Domain.Interfaces.Repositories.Transactions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FinanceControl API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

// Endpoint para testar conexão com MySQL (banco local / MySQL Workbench)
app.MapGet("/api/test-db", async (FinanceDbContext db) =>
{
    try
    {
        var canConnect = await db.Database.CanConnectAsync();
        return Results.Ok(new
        {
            success = true,
            message = "Conexão com MySQL (FinanceControl) OK.",
            connected = canConnect,
            server = "localhost",
            database = "FinanceControl"
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new
        {
            success = false,
            message = "Falha ao conectar no banco.",
            error = ex.Message,
            hint = "Verifique: MySQL em execução, ConnectionStrings:DefaultConnection em appsettings, usuário/senha e nome do banco."
        }, statusCode: 503);
    }
})
.WithName("TestDbConnection")
.WithTags("Health");

app.Run();
