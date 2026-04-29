using AttendanceTracking.Clients;
using AttendanceTracking.Data;
using AttendanceTracking.Middleware;
using AttendanceTracking.Repositories.Implementations;
using AttendanceTracking.Repositories.Interfaces;
using AttendanceTracking.Services.Implementations;
using AttendanceTracking.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =======================
// Database
// =======================
builder.Services.AddDbContext<AttendanceDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// =======================
// Repositories
// =======================
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();

// =======================
// Services
// =======================
builder.Services.AddScoped<IAttendanceService, AttendanceService>();

// =======================
// HTTP Clients (Microservice calls)
// =======================
builder.Services.AddHttpClient<StudentClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["Services:IdentityService"]);
    client.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddHttpClient<AcademicClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["Services:AcademicService"]);
    client.Timeout = TimeSpan.FromSeconds(5);
});

// =======================
// Controllers & Swagger
// =======================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// =======================
// Middleware pipeline
// =======================

// Exception handling FIRST
app.UseMiddleware<ExceptionMiddleware>();

// Request logging NEXT
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
