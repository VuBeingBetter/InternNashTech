using Microsoft.EntityFrameworkCore;
using System.Globalization;

using Infrastructure.Data;
using Infrastructure.Repositories;
using Application.Interfaces;
using Application.Services;
using FluentValidation.AspNetCore;
using FluentValidation;
using Application.Validators;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Đăng ký DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<BankAccountValidator>(); // Register validator for BankAccount
builder.Services.AddValidatorsFromAssemblyContaining<TransactionValidator>(); // Register validator for Transaction

// For React access
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        builder => builder.WithOrigins("http://localhost:5173") // Thay đổi nếu React app chạy ở cổng khác
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

// Đăng ký Repository (Dùng Interface từ Domain, Thực thi từ Infrastructure)
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

// QUAN TRỌNG: Bạn nên dùng Unit of Work để quản lý giao dịch
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); 

// Đăng ký Services (Dùng Interface từ Application.Interfaces, Thực thi từ Application.Services)
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("AllowReact");

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
