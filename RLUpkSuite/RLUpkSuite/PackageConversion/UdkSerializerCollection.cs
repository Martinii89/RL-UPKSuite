using Microsoft.Extensions.DependencyInjection;

using RlUpk.Core.Serialization;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Serialization.Extensions;
using RlUpk.Core.Types;

namespace RlUpk.RLUpkSuite.PackageConversion;

public class UdkSerializerCollection
{
    public UdkSerializerCollection()
    {
        ServiceCollection serviceCollection = new();
        serviceCollection.AddSerializers(typeof(UnrealPackage), new SerializerOptions());
        serviceCollection.AddSingleton<IObjectSerializerFactory, ObjectSerializerFactory>();
        serviceCollection.AddSingleton<PackageExporterFactory>();
        Services = serviceCollection.BuildServiceProvider();

        PackageExporterFactory = Services.GetRequiredService<PackageExporterFactory>();
    }

    public ServiceProvider Services { get; }

    public PackageExporterFactory PackageExporterFactory { get; }
}