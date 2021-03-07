using Application.Common.Interfaces;
using Infrastructure.Services;
using Infrastructure.Services.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IMessageSender, MessageSender>();

            return services;
        }
    }
}