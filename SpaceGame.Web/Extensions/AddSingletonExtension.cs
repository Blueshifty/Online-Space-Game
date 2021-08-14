using Microsoft.Extensions.DependencyInjection;
using SpaceGame.Business.Application;
using SpaceGame.Business.Utilities.Mapper;

namespace SpaceGame.Web.Extensions
{
    public static class AddSingletonExtensions
    {
        public static IServiceCollection AddMySingleton(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IMapper, Mapper>();
            serviceCollection.AddSingleton<ApplicationContext>();
            return serviceCollection;
        }
    }
}