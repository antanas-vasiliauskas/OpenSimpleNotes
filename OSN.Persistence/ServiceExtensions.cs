using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using OSN.Domain.Interfaces;
using OSN.Persistence.Repositories;

namespace OSN.Persistence;

public static class ServiceExtensions
{
    public static void AddPersistenceLayer(this IServiceCollection services, string? connectionString, Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptions = null)
    {
        services.AddDbContext<AppDbContext>(options => options.UseLazyLoadingProxies().UseNpgsql(connectionString, npgsqlOptions));

        services.AddScoped<IUsersRepository, UsersRepository>();
    }
}