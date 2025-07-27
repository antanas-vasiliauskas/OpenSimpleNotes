using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OSN.Application;
using OSN.Domain.Models;
using OSN.Infrastructure;
using OSN.Infrastructure.Services;
using System.Text;

// SuperUser
// Alexane_Prohaska@hotmail.com

// Admin
// Reanna.Runte@hotmail.com

// User
// Verona_Yundt@hotmail.com


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();





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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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
