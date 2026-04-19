using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SampleTracker.API.Data;
using SampleTracker.API.Services;
using static SampleTracker.API.Services.KeyVaultService;

var builder = WebApplication.CreateBuilder(args);

// ── Azure Key Vault (optional) ────────────────────────────────────────────────
// Set KeyVault:Uri in Azure App Service → Configuration to enable.
// Uses the App Service Managed Identity endpoint — no SDK required.
// Locally the block is skipped and appsettings.Development.json is used instead.
var kvUri = builder.Configuration["KeyVault:Uri"];
if (!string.IsNullOrEmpty(kvUri))
{
    var jwtSecret = await TryGetSecretAsync(kvUri, "JwtKey");
    if (jwtSecret is not null) builder.Configuration["Jwt:Key"] = jwtSecret;
}

// ── Database ──────────────────────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? "Data Source=sampletracker.db";

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(connectionString));

// ── MediatR ───────────────────────────────────────────────────────────────────
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// ── Auth ──────────────────────────────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new Exception("JWT key not configured.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer   = true,
            ValidIssuer      = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience    = builder.Configuration["Jwt:Audience"]
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<TokenService>();

// ── Swagger ───────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SampleTracker API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", Type = SecuritySchemeType.Http,
        Scheme = "Bearer", BearerFormat = "JWT", In = ParameterLocation.Header
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                    { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db     = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var tokens = scope.ServiceProvider.GetRequiredService<TokenService>();
    db.Database.EnsureCreated();

    if (!db.Users.Any(u => u.Email == "guest@sampletracker.demo"))
    {
        db.Users.Add(new SampleTracker.API.Models.User
        {
            Email        = "guest@sampletracker.demo",
            DisplayName  = "Guest",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("guest"),
            Role         = SampleTracker.API.Models.UserRole.Analyst
        });
        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
