# Contributing to EduFlow

> **Read this before creating a new service or PR.** Following these conventions keeps the 9 services consistent so they actually work together.

---

## 1. Project Conventions (Locked-In)

These are not suggestions. Diverging from them breaks cross-service calls.

| Topic | Decision |
|---|---|
| **.NET version** | `net8.0` (set in `Directory.Build.props` at root — inherited automatically) |
| **Spelling** | British: `Enrolment`, `Programme` (matches design doc) — **never** Enrollment |
| **Route casing** | Lowercase: `[Route("api/student")]` — **never** `api/[controller]` (which produces PascalCase) |
| **DTO naming** | `XxxDto`, `CreateXxxDto`, `UpdateXxxDto` |
| **Async methods** | Always end with `Async` (enforced by `.editorconfig`) |
| **Interfaces** | Always start with `I` (`IUserService`, not `UserServiceInterface`) |
| **Private fields** | Always start with `_` (`_repository`, not `repository` or `m_repository`) |
| **Auth** | JWT pass-through. Single signing key shared across all services. No API keys. |
| **Migrations** | `YYYYMMDD_DescriptiveName.cs` — e.g. `20260428_AddStudentTable.cs` |
| **Audit logging** | Every state change writes an AuditLog entry with `userId` from JWT claims |

---

## 2. Service Port Map

| Module | Service | Port | Status |
|---|---|---|---|
| M1 | IdentityService | 5216 | ✅ Built |
| M2 | StudentService | 5002 | ❌ Pending |
| M3 | AttendanceTracking | 5261 | ✅ Built |
| M4 | FinanceService | 5004 | ❌ Pending |
| M5 | _(merged with M4)_ | — | — |
| M6 | GradingService | 5006 | ❌ Pending |
| M7 | _(reserved)_ | — | — |
| M8 | ReportingService | 5005 | ✅ Built |
| M9 | NotificationService | 5007 | ❌ Pending |
| **Gateway** | Ocelot | 5000 | 🔜 Phase 2 |

When adding a new service, claim a port from this table and update it in the same PR.

---

## 3. How to Create a New EduFlow Service

### Step 1 — Generate the project

```bash
cd EduFlow-Web
dotnet new webapi -n YourService -f net8.0
cd YourService
rm WeatherForecast.cs Controllers/WeatherForecastController.cs   # remove template junk
```

### Step 2 — Reference the shared library

```bash
dotnet add reference ../EduFlow.Shared/EduFlow.Shared.csproj
```

### Step 3 — Add common NuGet packages

```bash
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
dotnet add package Swashbuckle.AspNetCore --version 6.5.0
```

(Serilog and JWT come transitively from `EduFlow.Shared`.)

### Step 4 — Folder structure

```
YourService/
├── Controllers/
├── Services/
│   ├── Interfaces/
│   └── Implementations/
├── Repositories/
│   ├── Interfaces/
│   └── Implementations/
├── Models/                    (EF entities, owned by this service)
├── DTOs/                      (request/response types specific to this service)
├── Data/
│   └── AppDbContext.cs
├── Migrations/
├── Properties/
│   └── launchSettings.json    (set applicationUrl to your assigned port)
├── appsettings.json
├── Program.cs
└── YourService.csproj
```

> Note: cross-service DTOs (UserDto, StudentDto, EnrolmentDto, etc.) live in `EduFlow.Shared.Contracts`, not in `YourService/DTOs/`. Don't redefine them locally — that defeats the entire shared-library setup.

### Step 5 — `Program.cs` template

Copy this exactly and adjust namespaces:

```csharp
using EduFlow.Shared.Auth;
using EduFlow.Shared.Logging;
using EduFlow.Shared.Middleware;
using Microsoft.EntityFrameworkCore;
using YourService.Data;
// ... your interfaces / implementations

var builder = WebApplication.CreateBuilder(args);

// --- Logging ---
builder.AddEduFlowSerilog();

// --- Database ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Authentication (JWT pass-through) ---
builder.Services.AddEduFlowJwtAuth(builder.Configuration);
builder.Services.AddHttpContextAccessor();

// --- DI: Repositories & Services ---
builder.Services.AddScoped<IYourRepository, YourRepository>();
builder.Services.AddScoped<IYourService, YourServiceImpl>();

// --- HttpClients to other EduFlow services (with JWT forwarding) ---
builder.Services.AddTransient<JwtForwardingHandler>();
builder.Services.AddHttpClient<SomeOtherClient>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["ServiceUrls:SomeOtherService"]!);
})
.AddHttpMessageHandler<JwtForwardingHandler>();

// --- API surface ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- Pipeline (ORDER MATTERS — DO NOT REORDER) ---
app.UseMiddleware<ExceptionMiddleware>();         // 1. catches everything below
app.UseMiddleware<RequestLoggingMiddleware>();    // 2. logs everything below

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();                          // 3. reads JWT
app.UseAuthorization();                           // 4. enforces [Authorize]
app.MapControllers();                             // 5. routes to actions

app.Run();
```

### Step 6 — `appsettings.json` template

```json
{
  "Logging": { "LogLevel": { "Default": "Information" } },
  "AllowedHosts": "*",
  "JwtSettings": {
    "Issuer":   "EduFlow",
    "Audience": "EduFlowClients",
    "SecretKey": "ThisIsADevOnlySecretKeyAtLeast32CharactersLong!!",
    "ExpiryMinutes": 60
  },
  "ServiceUrls": {
    "IdentityService":   "http://localhost:5216",
    "StudentService":    "http://localhost:5002",
    "AttendanceService": "http://localhost:5261",
    "FinanceService":    "http://localhost:5004",
    "GradingService":    "http://localhost:5006",
    "ReportingService":  "http://localhost:5005",
    "NotificationService": "http://localhost:5007"
  }
}
```

`appsettings.Development.json` holds the `ConnectionStrings` section and is gitignored.

---

## 4. Controller Conventions

```csharp
[ApiController]
[Route("api/yourthing")]   // ← lowercase, explicit, NEVER api/[controller]
[Authorize]                 // ← default to authenticated; relax per-action with [AllowAnonymous]
public class YourThingController : ControllerBase
{
    private readonly IYourService _service;

    public YourThingController(IYourService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "Administrator,Registrar")]   // role checks at action level
    public async Task<IActionResult> GetAllAsync()
    {
        var items = await _service.GetAllAsync();
        return Ok(items);
    }

    [HttpPost]
    [Authorize(Roles = "Faculty")]
    public async Task<IActionResult> CreateAsync(CreateThingDto dto)
    {
        var userId = User.GetUserId();   // ← from EduFlow.Shared.Auth, NEVER hardcode
        var created = await _service.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = created.Id }, created);
    }
}
```

---

## 5. Cross-Service Validation Rules

When you receive a foreign key in a request, validate it against the **service that owns the table**, not just any service that happens to have a related ID.

| FK in your request | Validate against |
|---|---|
| `UserID` | M1 IdentityService (`User` table) |
| `StudentID` | M2 StudentService (`Student` table) |
| `CourseID` | M2/M3 AcademicService (`Course` table) |
| `EnrolmentID` | M2 StudentService (`Enrolment` table) |
| `InvoiceID` | M4 FinanceService (`Invoice` table) |

**Common mistake:** validating `StudentID` against the User table (M1). A `StudentID` is NOT a `UserID` — they are separate ID spaces. Faculty have `UserID` but no `StudentID`. Always go to the right service.

---

## 6. Branching & PR Etiquette

1. Branch off `main` only **after** the standardization PR (#1) is merged. Don't fork off a half-done state.
2. One service per PR when possible.
3. Run `dotnet build` and `dotnet ef migrations add <Name>` before pushing. CI is not in place yet — manual diligence is required.
4. Never commit `appsettings.Development.json`, `bin/`, `obj/`, or `Logs/`. They are in `.gitignore` for a reason.
5. PR title format: `[M<n>] Short description` — e.g. `[M2] Add Student CRUD endpoints`.

---

## 7. When in Doubt

Ask in the team channel before inventing a new pattern. The whole point of `EduFlow.Shared` is that we solve a problem **once** and every service inherits the solution. If you find yourself copy-pasting middleware or DTOs between services, stop and lift it into the shared library instead.
