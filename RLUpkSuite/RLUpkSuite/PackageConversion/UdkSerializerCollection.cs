using Core.Serialization;
using Core.Serialization.Abstraction;
using Core.Types;

using Microsoft.Extensions.DependencyInjection;

namespace RLUpkSuite.PackageConversion;

public class UdkSerializerCollection
{
    public UdkSerializerCollection()
    {
        ServiceCollection serviceCollection = new();
        serviceCollection.UseSerializers(typeof(UnrealPackage), new SerializerOptions());
        serviceCollection.AddSingleton<IObjectSerializerFactory, ObjectSerializerFactory>();
        serviceCollection.AddSingleton<PackageExporterFactory>();
        Services = serviceCollection.BuildServiceProvider();

        PackageExporterFactory = Services.GetRequiredService<PackageExporterFactory>();
    }

    public ServiceProvider Services { get; }

    public PackageExporterFactory PackageExporterFactory { get; }
}