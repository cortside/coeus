using Microsoft.Extensions.DependencyInjection;

namespace Cortside.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a startup task
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IServiceCollection AddStartupTask<T>(this IServiceCollection services) where T : class, IStartupTask =>
            services.AddTransient<IStartupTask, T>();
    }
}
