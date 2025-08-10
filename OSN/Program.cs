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
using OSN.Infrastructure.Authentication;
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
    c.AddSecurityDefinition("Cookie", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Cookie,
        Name = "auth-token",
        Description = "JWT token stored in HTTP-only cookie"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Cookie"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Replace JWT Bearer authentication with custom JWT Cookie authentication
builder.Services.AddAuthentication(JwtCookieAuthenticationOptions.DefaultScheme)
    .AddScheme<JwtCookieAuthenticationOptions, JwtCookieAuthenticationHandler>(
        JwtCookieAuthenticationOptions.DefaultScheme, options => { });

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
builder.Services.AddScoped<GoogleOAuth2Service>();
builder.Services.AddScoped<CookieService>();
builder.Services.AddHttpClient<GoogleOAuth2Service>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Update CORS to allow credentials (cookies)
builder.Services.AddCors(options => {
    options.AddPolicy("AllowCredentials", builder => {
        builder.WithOrigins("http://localhost:3000", "https://yourdomain.com") // Specify exact origins
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials(); // Allow cookies
    });
});

var app = builder.Build();

app.UseCors("AllowCredentials");
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.ConfigObject.AdditionalItems["persistAuthorization"] = true;
    });
    app.MapGet("/", () => Results.Redirect("/swagger")).AllowAnonymous();
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
        // dotnet ef database update --connection "your-remote-connection-string"
        // to update remote. Can't do it in code, it is security vulnerability.
        context.Database.Migrate();
        if (!context.Users.Any())
        {
            var dataSeeder = new DataSeeder(context, passwordHasher);
            dataSeeder.Seed();
        }
    }
}

app.MapControllers();

app.Run();
