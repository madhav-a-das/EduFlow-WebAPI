using EduFlow.Shared.Auth;
using EduFlow.Shared.Logging;
using EduFlow.Shared.Middleware;
using IdentityService.Data;
using IdentityService.Helpers;
using IdentityService.Repositories.Implementations;
using IdentityService.Repositories.Interfaces;
using IdentityService.Services.Implementations;
using IdentityService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// =============================================================================
//  EduFlow Service Bootstrap — Identity & Access Management (M1)
// =============================================================================
//  Most cross-cutting setup lives in EduFlow.Shared. This file should ONLY
//  contain things that are unique to M1: its own DbContext, its own DI bindings,
//  Swagger config (because of the JWT button), and EF migrations on startup.
//  
//  When in doubt, push code into EduFlow.Shared so other services inherit it.
// =============================================================================

// --- 1. Logging (Serilog with console + rolling file) ---
builder.AddEduFlowSerilog();

// --- 2. Database ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 3. JWT Authentication & Authorization ---
//   Reads "JwtSettings" from appsettings.json. Validates issuer, audience,
//   lifetime, and signing key on every request with an Authorization header.
builder.Services.AddEduFlowJwtAuth(builder.Configuration);

// Required for JwtForwardingHandler if M1 ever calls another service
builder.Services.AddHttpContextAccessor();

// --- 4. DI: Repositories, Services, Helpers ---
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<JwtHelper>();   // token issuance — M1-specific

// --- 5. Controllers + Swagger with JWT 'Authorize' button ---
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Suppress ASP.NET's default ProblemDetails response for ModelState errors.
        // Our controllers throw ValidationException, and ExceptionMiddleware
        // converts it to the standard EduFlow ApiErrorResponse shape.
        options.SuppressModelStateInvalidFilter = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EduFlow — Identity & Access Management",
        Version = "v1",
        Description = "Handles user registration, login, RBAC, and audit logs."
    });

    // Bearer token scheme — adds the green Authorize button to Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Paste your JWT token here. Example: eyJhbGci..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// --- 6. Auto-migrate database on startup ---
//   Equivalent to "dotnet ef database update" but runs every launch so a fresh
//   clone "just works" — no manual migration step needed.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// --- 7. Pipeline (ORDER MATTERS — DO NOT REORDER) ---
app.UseMiddleware<ExceptionMiddleware>();          // 1st — catches everything below
app.UseMiddleware<RequestLoggingMiddleware>();     // 2nd — logs everything below

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();   // reads JWT → populates HttpContext.User
app.UseAuthorization();    // enforces [Authorize] attributes

app.MapControllers();

Log.Information("EduFlow IdentityService starting on {Urls}",
    string.Join(", ", app.Urls.DefaultIfEmpty("default ports")));

app.Run();
