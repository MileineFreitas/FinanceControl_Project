using FinanceControl.Domain.Interfaces.AppServices.Users;
using FinanceControl.Domain.Services.Users;
using FinanceControl.Domain.Interfaces.DomService.Users;
using FinanceControl.Domain.Interfaces.Repositories.Categories;
using FinanceControl.Domain.Interfaces.Repositories.Transactions;
using FinanceControl.Domain.Interfaces.Repositories.Users;
using FinanceControl.Infrastructure.Contexts;
using FinanceControl.Infrastructure.Seeding;
using FinanceControl.Infrastructure.Repositories.Categories;
using FinanceControl.Infrastructure.Repositories.Transactions;
using FinanceControl.Infrastructure.Repositories.Users;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;


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


builder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserAppService, UserDomService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

var app = builder.Build();

/*
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
    FinanceDbContextSeed.EnsureDemoUserAccountAndCategories(db);
}
*/

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
