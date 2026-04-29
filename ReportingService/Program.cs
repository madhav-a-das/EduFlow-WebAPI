using Microsoft.EntityFrameworkCore;
using Serilog;
using ReportingService.Data;
using ReportingService.Clients;
using ReportingService.Middleware;
using ReportingService.Repositories.Interfaces;
using ReportingService.Repositories.Implementations;
using ReportingService.Services.Interfaces;
using ReportingService.Services.Implementations;

// ── Serilog Bootstrap ──
var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting ReportingService...");

    // ── Database Context ──
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // ── Repositories ──
    builder.Services.AddScoped<IReportRepository, ReportRepository>();
    builder.Services.AddScoped<IKPIRepository, KPIRepository>();
    builder.Services.AddScoped<IAuditPackageRepository, AuditPackageRepository>();

    // ── Services ──
    builder.Services.AddScoped<IReportService, ReportService>();
    builder.Services.AddScoped<IKPIService, KPIService>();
    builder.Services.AddScoped<IAuditPackageService, AuditPackageService>();

    // ── HTTP Clients for cross-service communication ──
    builder.Services.AddHttpClient<StudentClient>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:IdentityStudentService"]!);
    });

    builder.Services.AddHttpClient<AcademicClient>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:AcademicService"]!);
    });

    builder.Services.AddHttpClient<AttendanceClient>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:AttendanceService"]!);
    });

    builder.Services.AddHttpClient<GradingClient>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:GradingService"]!);
    });

    builder.Services.AddHttpClient<FinanceClient>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:FinanceService"]!);
    });

    // ── CORS ──
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    // ── Controllers & Swagger ──
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "EduFlow - Reporting Service",
            Version = "v1",
            Description = "Reporting, KPIs & Audit Packages for the EduFlow platform."
        });
    });

    var app = builder.Build();

    // ── Middleware Pipeline (ORDER MATTERS!) ──

    // 1. Global exception handler — catches ALL errors
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // 2. Request logging — logs every request with duration
    app.UseMiddleware<RequestLoggingMiddleware>();

    // 3. Swagger — only in development
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ReportingService v1");
            c.RoutePrefix = "swagger";
        });
    }

    // 4. CORS — allow cross-origin requests
    app.UseCors("AllowAll");

    // 5. HTTPS redirect
    app.UseHttpsRedirection();

    // 6. Authorization
    app.UseAuthorization();

    // 7. Map controllers
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly!");
}
finally
{
    Log.CloseAndFlush();
}