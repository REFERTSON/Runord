using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace Runord.Hub.Extensions
{
    public static class OpenApiExtensions
    {
        public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
        {
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    // 1. Создаем схему аутентификации
                    var securityScheme = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        BearerFormat = "JWT",
                        Description = "Введите ваш JWT Access Token в формате: Bearer {токен}"
                    };

                    // 2. В .NET 10 для добавления схем используется встроенный метод AddComponent.
                    document.AddComponent(JwtBearerDefaults.AuthenticationScheme, securityScheme);

                    // 3. Инициализируем глобальные требования к безопасности документа
                    document.Security ??= new List<OpenApiSecurityRequirement>();

                    // 4. Создаем требование: 
                    // Ключ - это ссылка на схему (OpenApiSecuritySchemeReference),
                    // Значение - это List<string> (а не строковый массив)
                    var securityRequirement = new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document)] = new List<string>()
                    };

                    document.Security.Add(securityRequirement);

                    return Task.CompletedTask;
                });
            });

            return services;
        }

        public static IEndpointRouteBuilder UseApiDocumentationUi(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapOpenApi();

            endpoints.MapScalarApiReference(options =>
            {
                options.WithTitle("Runord Distributed Hub API");
                options.Theme = ScalarTheme.Moon;
            });

            return endpoints;
        }
    }
}