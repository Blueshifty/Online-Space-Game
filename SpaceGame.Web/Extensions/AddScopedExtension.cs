using Microsoft.Extensions.DependencyInjection;

namespace SpaceGame.Web.Extensions
{
    public static class AddScopedExtensions
    {
        public static IServiceCollection AddMyScoped(this IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }
    }
}