using FinanceControl.Domain.Interfaces.AppServices.Users;
using FinanceControl.Domain.Services.Users;
using FinanceControl.Domain.Interfaces.DomService.Users;
using FinanceControl.Domain.Interfaces.Repositories.Categories;
using FinanceControl.Domain.Interfaces.Repositories.Transactions;
using FinanceControl.Domain.Interfaces.Repositories.Users;
using FinanceControl.Infrastructure.Contexts;
using FinanceControl.Infrastructure.Repositories.Categories;
using FinanceControl.Infrastructure.Repositories.Transactions;
using FinanceControl.Infrastructure.Repositories.Users;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

/*
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7143")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
*/

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFront", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500") // porta do seu front (Live Server)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection")!;

Console.WriteLine(builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserAppService, UserDomService>();
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
app.UseCors("AllowFront");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.Run();
