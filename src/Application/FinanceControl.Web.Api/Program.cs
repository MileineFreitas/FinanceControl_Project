using FinanceControl.Domain.Interfaces.AppServices.Accounts;
using FinanceControl.Domain.Interfaces.AppServices.Categories;
using FinanceControl.Domain.Interfaces.AppServices.PaymentMethods;
using FinanceControl.Domain.Interfaces.AppServices.Transactions;
using FinanceControl.Domain.Interfaces.AppServices.Users;
using FinanceControl.Domain.Interfaces.DomService.Accounts;
using FinanceControl.Domain.Interfaces.DomService.Categories;
using FinanceControl.Domain.Interfaces.DomService.PaymentMethods;
using FinanceControl.Domain.Interfaces.DomService.Transactions;
using FinanceControl.Domain.Interfaces.DomService.Users;
using FinanceControl.Domain.Interfaces.Repositories.Accounts;
using FinanceControl.Domain.Interfaces.Repositories.Categories;
using FinanceControl.Domain.Interfaces.Repositories.PaymentMethods;
using FinanceControl.Domain.Interfaces.Repositories.Transactions;
using FinanceControl.Domain.Interfaces.Repositories.Users;
using FinanceControl.Domain.Services.Accounts;
using FinanceControl.Domain.Services.Categories;
using FinanceControl.Domain.Services.PaymentMethods;
using FinanceControl.Domain.Services.Transactions;
using FinanceControl.Domain.Services.Users;
using FinanceControl.Infrastructure.Contexts;
using FinanceControl.Infrastructure.Repositories.Accounts;
using FinanceControl.Infrastructure.Repositories.Categories;
using FinanceControl.Infrastructure.Repositories.PaymentMethods;
using FinanceControl.Infrastructure.Repositories.Transactions;
using FinanceControl.Infrastructure.Repositories.Users;
using FinanceControl.Infrastructure.Seeding;
using FinanceControl.Services.Accounts;
using FinanceControl.Services.Categories;
using FinanceControl.Services.PaymentMethods;
using FinanceControl.Services.Transactions;
using FinanceControl.Services.Users;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7143")
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

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryDomService, CategoryDomService>();
builder.Services.AddScoped<ICategoryAppService, CategoryAppService>();

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionDomService, TransactionDomService>();
builder.Services.AddScoped<ITransactionAppService, TransactionAppService>();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountDomService, AccountDomService>();
builder.Services.AddScoped<IAccountAppService, AccountAppService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserDomService, UserDomService>();
builder.Services.AddScoped<IUserAppService, UserAppService>();

builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
builder.Services.AddScoped<IPaymentMethodDomService, PaymentMethodDomService>();
builder.Services.AddScoped<IPaymentMethodAppService, PaymentMethodAppService>();

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
    app.Services.ApplyMigrationsAndSeed(app.Logger);

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
app.UseCors();
if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
