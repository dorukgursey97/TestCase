using AppTemplate.Application.DTOs.Todo;
using AppTemplate.Application.Interfaces;
using AppTemplate.Application.Services;
using AppTemplate.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AppTemplate.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITodoService, TodoService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IValidator<CreateTodoDto>, CreateTodoValidator>();
        services.AddScoped<IValidator<UpdateTodoDto>, UpdateTodoValidator>();

        return services;
    }
}
