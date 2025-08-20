using MediatR;
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
using System.Threading.RateLimiting;

// SuperUser
// Alexander_Stoltenberg31@hotmail.com

// Admin
// Reanna.Runte@hotmail.com

// User
// Verona_Yundt@hotmail.com

namespace OSN;

public class Program
{
    public static void Main(string[] args)
    {

        #region ConfigureServices

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


        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetTokenBucketLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: partition => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 500,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                        TokensPerPeriod = 2,
                        AutoReplenishment = true
                    }));

            options.AddPolicy("AuthPolicy", httpContext =>
                RateLimitPartition.GetTokenBucketLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: partition => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 50,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                        TokensPerPeriod = 1,
                        AutoReplenishment = true
                    }));

            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = 429;
                
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter = 
                        ((int)retryAfter.TotalSeconds).ToString();
                }

                await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", token);
            };
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
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
            // Fallback policy applied if there is [Authorize] attribute without arguments or nothing at all.
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(RoleHierarchy._hierarchy[RoleHierarchy.DefaultPolicy])
                .Build();
        });

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(
                typeof(LoginCommand).Assembly // CommandHandlers in OSN.Application
            );
        });

        // Doesn't pickup automatically.
        builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));



        builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<PasswordHasher>();
        builder.Services.AddScoped<GoogleOAuth2Service>();
        builder.Services.AddScoped<EmailService>();
        builder.Services.AddHttpClient<GoogleOAuth2Service>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

        builder.Services.AddCors(options => {
            options.AddPolicy("AllowSpecificOrigins", policyBuilder => {
                if (builder.Environment.IsDevelopment())
                {
                    policyBuilder.WithOrigins(
                        "http://localhost:3000",
                        "https://localhost:3000",
                        "https://black-field-0f7ce1903.2.azurestaticapps.net",
                        "https://opensimplenotes.com"
                    );
                }
                else
                {
                    policyBuilder.WithOrigins(
                        "https://black-field-0f7ce1903.2.azurestaticapps.net",
                        "https://opensimplenotes.com"
                    );
                }
                policyBuilder
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        var app = builder.Build();
        #endregion


        #region Configure

        app.UseCors("AllowSpecificOrigins");
        app.UseMiddleware<ExceptionHandlingMiddleware>();


        app.UseRateLimiter();

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

        #endregion
    }
}
