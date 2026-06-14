using Microsoft.Extensions.DependencyInjection;

namespace EldredBrown.ProFootball.Net.Services
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddServiceLibrary(this IServiceCollection services)
        {
            return services;
        }
    }
}
