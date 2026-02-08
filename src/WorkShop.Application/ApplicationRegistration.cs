using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WorkShop.Application.Commands.Books;
using WorkShop.Application.Validators;

namespace WorkShop.Application;

public static class ApplicationRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Add FluentValidation Validators from the Application assembly
        services.AddValidatorsFromAssemblyContaining<BookRequestModelValidator>();

        // Register MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateBookCommand).Assembly);
        });

        return services;
    }
}
