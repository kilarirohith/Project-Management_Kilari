using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using server.Data;
using server.Services.Interfaces;
using server.Services.Implementations;
using server.Models;
using server.Utils;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ---------------------- CONFIGURATION ----------------------
var configuration = builder.Configuration;

// ---------------------- DATABASE ----------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// ---------------------- DEPENDENCY INJECTION ----------------
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<IMilestoneMasterService, MilestoneMasterService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IProjectMasterService, ProjectMasterService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IVendorWorkService, VendorWorkService>();
builder.Services.AddScoped<IApprovalDeskService, ApprovalDeskService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// ---------------------- CORS -------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// ---------------------- JWT AUTH ----------------------------
var jwtKey = configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key not configured in appsettings.json!");

var issuer = configuration["Jwt:Issuer"];
var audience = configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("JWT auth failed: " + context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("JWT token validated for: " + context.Principal?.Identity?.Name);
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// ---------------------- SWAGGER -----------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PMS System API",
        Version = "v1",
        Description = "Backend API for Project Management System"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter JWT token in the format: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// ---------------------- APP PIPELINE -------------------------
var app = builder.Build();

// --- DATABASE MIGRATION & SEEDING ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // apply all migrations
    db.Database.Migrate();

    // 🔹 Ensure base roles exist
    Role? adminRole = db.Roles.FirstOrDefault(r => r.Name == "Admin");

    if (adminRole == null)
    {
        adminRole = new Role { Name = "Admin", Description = "System Administrator" };
        db.Roles.Add(adminRole);
    }

    if (!db.Roles.Any(r => r.Name == "User"))
    {
        db.Roles.Add(new Role { Name = "User", Description = "Standard User" });
    }

    if (!db.Roles.Any(r => r.Name == "Manager"))
    {
        db.Roles.Add(new Role { Name = "Manager", Description = "Project Manager" });
    }

    // 🔹 Seed default modules (only if none yet)
    if (!db.AppModules.Any())
    {
        db.AppModules.AddRange(
            new AppModule { Name = "Dashboard" },
            new AppModule { Name = "Projects" },
            new AppModule { Name = "Ticket Tracker" },
            new AppModule { Name = "Task Tracker" },
            new AppModule { Name = "Masters" },
            new AppModule { Name = "VendorWork" },   
            new AppModule { Name = "ApprovalDesk" }  
        );
    }

    db.SaveChanges(); 

    // 🔹 Ensure default admin user exists
    if (!db.Users.Any(u => u.Email == "admin@admin.com"))
    {
        adminRole = db.Roles.First(r => r.Name == "Admin");

        var adminUser = new User
        {
            FullName = "System Admin",
            Username = "admin",
            Email = "admin@admin.com",
            PasswordHash = AuthHelper.HashPassword("admin123"),
            RoleId = adminRole.Id,
            Role = adminRole
        };

        db.Users.Add(adminUser);
        db.SaveChanges();
    }

    // 🔹 Give Admin FULL permissions for all modules (once)
    adminRole = db.Roles.First(r => r.Name == "Admin");

    if (!db.RolePermissions.Any(rp => rp.RoleId == adminRole.Id))
    {
        var adminModules = new[]
        {
            "Dashboard",
            "Masters",
            "Projects",
            "Task Tracker",
            "Ticket Tracker",
            "VendorWork",
            "Approval Desk"
        };

        foreach (var m in adminModules)
        {
            db.RolePermissions.Add(new RolePermission
            {
                RoleId   = adminRole.Id,
                Module   = m,
                CanCreate = true,
                CanRead   = true,
                CanUpdate = true,
                CanDelete = true
            });
        }

        db.SaveChanges();
    }
}

// ---------------------- MIDDLEWARE ---------------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// just for debugging auth header
app.Use(async (ctx, next) =>
{
    if (ctx.Request.Headers.ContainsKey("Authorization"))
    {
        Console.WriteLine("AUTH HEADER: " + ctx.Request.Headers["Authorization"].ToString());
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllers();

app.Run();
