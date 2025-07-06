using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OSN.Application.Features.Auth.Commands.Login;
using OSN.Application.Interfaces.ModelsResolvers;
using OSN.Application.Interfaces.Services;
using OSN.Application.Mappings.ModelsResolvers;
using OSN.Infrastructure.Exceptions.Handlers;
using OSN.Infrastructure.Interfaces;
using OSN.Infrastructure.Services;
using OSN.Infrastructure.Utilities;
using OSN.Infrastructure.Utilities.Behaviours;

namespace OSN.Infrastructure;


public static class ServiceExtensions
{
    public static void AddInfrastructureLayer(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthenticationBehavior<,>));
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(HttpContextBehaviour<,>));
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PermissionBehaviour<,>));
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        services.AddHttpContextAccessor();

        services.AddSingleton<IHttpContentResolver, HttpContentResolver>();
        services.AddSingleton<ITokenService, TokenService>();
        services.AddScoped<IUserContext, HttpUserContext>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginCommandHandler).Assembly));
        services.AddValidatorsFromAssembly(typeof(LoginCommandValidator).Assembly);
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.RegisterExceptionHandling();
        services.RegisterMappers();
        services.RegisterFactories();
    }

    private static void RegisterExceptionHandling(this IServiceCollection services)
    {
        services.AddExceptionHandler<UnauthorizedExceptionHandler>();
        services.AddExceptionHandler<ForbiddenExceptionHandler>();
        services.AddExceptionHandler<BadRequestExceptionHandler>();
        services.AddExceptionHandler<NotFoundExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddProblemDetails();
    }

    private static void RegisterMappers(this IServiceCollection services)
    {
        services.AddSingleton<IAuthModelsResolver, AuthModelsResolver>();
        services.AddSingleton<INoteModelsResolver, NoteModelsResolver>();
    }

    private static void RegisterFactories(this IServiceCollection services)
    {
        // .
    }
}