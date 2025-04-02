using Microsoft.EntityFrameworkCore;
using lab06.Models;
using lab06.Repositories;


var builder = WebApplication.CreateBuilder(args);

// Cấu hình DbContext với SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký dịch vụ
builder.Services.AddControllers();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Cấu hình Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyAllowOrigins", policy =>
    {
        // Thay bằng địa chỉ localhost khi chạy frontend (VSCode)
        policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Cấu hình middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//Đặt trên UseAuthorization
app.UseCors("MyAllowOrigins");
app.UseAuthorization();
app.MapControllers();
app.Run();