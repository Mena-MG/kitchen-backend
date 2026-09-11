using System.Text;
using KitchenApi.Data;
using KitchenApi.Models;
using KitchenApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ══════════════════════════════════════════════════════════════
// 1. DATABASE — EF Core + SQLite
// ══════════════════════════════════════════════════════════════
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ══════════════════════════════════════════════════════════════
// 2. IDENTITY — ASP.NET Core Identity with AppUser
// ══════════════════════════════════════════════════════════════
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    // Password requirements
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;

    // User requirements
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ══════════════════════════════════════════════════════════════
// 3. JWT CONFIGURATION
// ══════════════════════════════════════════════════════════════
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Allow HTTP in dev
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // No tolerance for token expiry
    };
});

// ══════════════════════════════════════════════════════════════
// 4. APPLICATION SERVICES
// ══════════════════════════════════════════════════════════════
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// ══════════════════════════════════════════════════════════════
// 5. CONTROLLERS + SWAGGER
// ══════════════════════════════════════════════════════════════
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Aura Kitchen Market API",
        Version = "v1",
        Description = "Comprehensive E-Commerce Backend API for Aura Kitchen Market — Auth, Materials/Products, Categories, Orders, and Promos."
    });

    // Add JWT auth to Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter your JWT token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ══════════════════════════════════════════════════════════════
// 6. CORS — Allow React frontend
// ══════════════════════════════════════════════════════════════
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",         // Vite dev server
            "http://localhost:5174",
            "http://localhost:3000",          // Alternative dev
            "https://kitchen-mena-mg.vercel.app", // Vercel production
            "https://kitchen-2i5f460yw-mmms-projects-41242de5.vercel.app" // Vercel preview
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// ══════════════════════════════════════════════════════════════
// BUILD THE APP
// ══════════════════════════════════════════════════════════════
var app = builder.Build();

// ══════════════════════════════════════════════════════════════
// 7. SEED DATABASE — Roles + Initial Owner + E-Commerce Data
// ══════════════════════════════════════════════════════════════
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var config = builder.Configuration;
    var logger = services.GetRequiredService<ILogger<Program>>();

    // Apply pending migrations
    await db.Database.MigrateAsync();

    // Seed roles
    string[] roles = { "Owner", "Customer" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
            logger.LogInformation("Seeded role: {Role}", role);
        }
    }

    // Seed initial Owner account
    var ownerEmail = config["SeedOwner:Email"] ?? "owner@aura.kitchen";
    if (await userManager.FindByEmailAsync(ownerEmail) is null)
    {
        var owner = new AppUser
        {
            UserName = ownerEmail,
            Email = ownerEmail,
            FirstName = config["SeedOwner:FirstName"] ?? "Aura",
            LastName = config["SeedOwner:LastName"] ?? "Admin",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(owner, config["SeedOwner:Password"] ?? "Owner@123456");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(owner, "Owner");
            logger.LogInformation("Seeded initial Owner account: {Email}", ownerEmail);
        }
        else
        {
            logger.LogError("Failed to seed Owner: {Errors}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    // Seed E-commerce Catalog (Categories, Products, Promos)
    await DbInitializer.SeedEcommerceDataAsync(db, logger);
}

// ══════════════════════════════════════════════════════════════
// 8. MIDDLEWARE PIPELINE
// ══════════════════════════════════════════════════════════════
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Aura Kitchen Market API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("AllowFrontend");

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
