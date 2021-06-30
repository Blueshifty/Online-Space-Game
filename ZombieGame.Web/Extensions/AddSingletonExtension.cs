using Microsoft.Extensions.DependencyInjection;
using ZombieGame.Business.Utilities.Mapper;

namespace ZombieGame.Web.Extensions
{
    public static class AddSingletonExtensions
    {
        public static IServiceCollection AddMySingleton(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IMapper, Mapper>();
            return serviceCollection;
        }
    }
}