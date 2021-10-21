using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
namespace MiSmart.Infrastructure.Swagger
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration, OpenApiInfo openApiInfo)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", openApiInfo);
            });
            return services;
        }
    }
}