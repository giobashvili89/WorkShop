using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WorkShop.Application.Commands.Books;
using WorkShop.Application.Validators;

namespace WorkShop.Application;

public static class ApplicationRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<BookRequestModelValidator>();
        services.AddValidatorsFromAssemblyContaining<LoginRequestModelValidator>();
        services.AddValidatorsFromAssemblyContaining<OrderRequestModelValidator>();
        services.AddValidatorsFromAssemblyContaining<RegisterRequestModelValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateDeliveryInfoRequestModelValidator>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateBookCommand).Assembly);
        });

        return services;
    }
}
