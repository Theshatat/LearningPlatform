using LearningPlatform.Data;
using LearningPlatform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LearningPlatform.Controllers;
using LearningPlatform.Middleware;
using LearningPlatform.Filters;
using LearningPlatform.Services;
using LearningPlatform.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// Database
// ==========================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// ==========================
// MEMORY Caching
// ==========================
builder.Services.AddMemoryCache();

// ==========================
// RESPONSE Caching
// ==========================
builder.Services.AddResponseCaching();


// ==========================
// Controllers
// ==========================
builder.Services.AddControllers(
    options=>
    {
        options.Filters.Add<ActionLoggingFilter>();
        options.Filters.Add<ValidationFilter>();
    }
);

builder.Services.AddScoped<InstructorOnlyFilter>();
builder.Services.AddScoped<ApiExceptionFilter>();
// ==========================
// Identity
// ==========================
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


// ==========================
// JWT Authentication
// ==========================
var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();

builder.Services.AddSingleton(jwtOptions);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
.AddJwtBearer("Bearer", options =>
{
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtOptions.Issuer,

        ValidateAudience = true,
        ValidAudience = jwtOptions.Audience,

        ValidateLifetime = true,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.SigningKey)
            )
    };
});


// ==========================
// Swagger (ONLY Swagger)
// ==========================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization(options=>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("InstructorsOnly", policy =>
        policy.RequireRole("Instructor"));

    options.AddPolicy("StudentOnly", policy =>
        policy.RequireRole("Student"));

    options.AddPolicy("VerifiedUser", policy =>
        policy.RequireClaim("EmailConfirmed", "True"));
});

builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddOpenApi();


// ==========================
// Build App
// ==========================
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await RoleSeeder.SeedAsync(roleManager);
}
// ==========================
// Middleware Pipeline
// ==========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
};
app.UseExceptionHandlingMiddleware();  // Custom Middleware for handling exceptions globally
app.UseHttpsRedirection();
app.UseRequestLoggingMiddleware();  // Custom Middleware for logging requests
app.UsePerformanceMiddleware();  // Custom Middleware for measuring performance of requests
app.UseResponseCaching();
app.UseAuthentication();   // IMPORTANT (before Authorization)
app.UseAuthorization();

app.MapControllers();


app.Run();