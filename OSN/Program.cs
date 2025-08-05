using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OSN.Application;
using OSN.Application.Features.Auth.Login;
using OSN.Domain.Models;
using OSN.Infrastructure;
using OSN.Infrastructure.Services;
using System.Text;

// SuperUser
// Alexander_Stoltenberg31@hotmail.com

// Admin
// Reanna.Runte@hotmail.com

// User
// Verona_Yundt@hotmail.com


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
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





builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// For controller level authorization
builder.Services.AddAuthorization(options =>
{
    foreach (var (policyName, allowedRoles) in RoleHierarchy._hierarchy)
    {
        options.AddPolicy(policyName, policy =>
            policy.RequireRole(allowedRoles));
    }
    // Set fallback policy to be DefaultPolicy (UserPolicy)
    // Fallback policy still requires [Authorize]
    // just not need to specify [Authorize(Policy = RoleHierarchy.UserPolicy)]
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole(RoleHierarchy._hierarchy[RoleHierarchy.DefaultPolicy])
        .Build();
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(Program).Assembly,      // OSN
    typeof(LoginCommand).Assembly, // OSN.Application
    typeof(AppDbContext).Assembly, // OSN.Infrastructure
    typeof(User).Assembly          // OSN.Domain
));



builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", builder => {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()  // Explicitly allow OPTIONS
               .AllowAnyHeader();
    });
});



var app = builder.Build();

app.UseCors("AllowAll");
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.ConfigObject.AdditionalItems["persistAuthorization"] = true;
    });
    app.MapGet("/", () => Results.Redirect("/swagger"));
}


app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    var passwordHasher = services.GetRequiredService<PasswordHasher>();
    if (app.Environment.IsDevelopment())
    {
        context.Database.Migrate();
    }
    if (!context.Users.Any())
    {
        var dataSeeder = new DataSeeder(context, passwordHasher);
        dataSeeder.Seed();
    }
}


app.MapControllers();

app.Run();
