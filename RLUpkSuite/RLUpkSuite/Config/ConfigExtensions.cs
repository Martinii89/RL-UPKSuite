using Microsoft.Extensions.DependencyInjection;

namespace RLUpkSuite.Config
{
    public static class ConfigExtensions
    {
        public static IServiceCollection AddAppConfigs(this IServiceCollection services)
        {
            services.AddSingleton<AppConfigStore>();
            Type configBaseType = typeof(AppConfig);

            IEnumerable<Type> configImplementations = typeof(App).Assembly.GetTypes()
                .Where(x => x is { IsAbstract: false, IsInterface: false } && x.IsAssignableTo(configBaseType));

            foreach (Type configImplementation in configImplementations)
            {
                ServiceDescriptor service = new ServiceDescriptor(configBaseType, configImplementation, ServiceLifetime.Singleton);
                services.AddSingleton(service);
                services.AddSingleton(configImplementation);
            }

            return services;
        }

        public static IServiceCollection AddAppConfig<T>(this IServiceCollection services) where T : AppConfig
        {
            services.AddSingleton<T>();
            services.AddSingleton<AppConfig, T>(provider => provider.GetRequiredService<T>());

            return services;
        }
    }
}