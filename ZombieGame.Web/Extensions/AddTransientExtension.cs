using Microsoft.Extensions.DependencyInjection;

namespace ZombieGame.Web.Extensions
{
    public static class AddTransientExtension
    {
        public static IServiceCollection AddMyTransient(this IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }
    }
}