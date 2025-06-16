using Microsoft.OpenApi.Models;
using OSN.Persistence;
using OSN.Infrastructure;
using OSN.Swagger.Filters;
using Microsoft.EntityFrameworkCore;


namespace OSN.Configuration;

public class Program()
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        //builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
        //{
        //    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
        //}));
        builder.Services.AddSwaggerGen(c =>
        {
            c.OperationFilter<SwaggerRequestTypeOperationFilter>();

            c.AddSecurityDefinition("Bearer",
                new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });


        builder.Services.AddInfrastructureLayer();
        builder.Services.AddPersistenceLayer(builder.Configuration.GetConnectionString("DefaultConnection"), configure =>
        {
            configure.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorCodesToAdd: null);
        });


        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<AppDbContext>();

            if(app.Environment.IsDevelopment())
            {
                context.Database.Migrate(); // automatically migrates, no need for dotnet ef database update
            }

            if (!context.Users.Any())
            {
                var dataSeeder = new DataSeeder(context);
                dataSeeder.Seed();
            }
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();
        app.UseExceptionHandler();
        app.UseCors("corspolicy");

        app.MapControllers();
        app.Run();
    }    
}

