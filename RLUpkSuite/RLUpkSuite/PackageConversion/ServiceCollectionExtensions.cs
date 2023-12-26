using Core.RocketLeague.Decryption;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace RLUpkSuite.PackageConversion;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPackageConversion(this IServiceCollection services)
    {
        services.TryAddSingleton<UdkSerializerCollection>();
        services.TryAddSingleton<RlSerializerCollection>();
        services.TryAddTransient<IDecrypterProvider, DecryptionProvider>();

        // services.AddSingleton<Func<NativeClassFactory>>(() => new NativeClassFactory());
        services.AddSingleton<Func<IDecrypterProvider>>((provider => provider.GetRequiredService<IDecrypterProvider>));
        services.TryAddSingleton<PackageConverterFactory>();

        return services;
    }
}