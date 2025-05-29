using Microsoft.Extensions.DependencyInjection;

using RlUpk.Core.Serialization;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Serialization.Extensions;

namespace RlUpk.TestUtils.TestUtilities;

public class SerializerHelper
{
    private static IServiceProvider GetSerializersCollection(Type assembly, string tag = "")
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSerializers(assembly, new SerializerOptions(tag));
        serviceCollection.AddSingleton<IObjectSerializerFactory, ObjectSerializerFactory>();
        var services = serviceCollection.BuildServiceProvider();
        return services;
    }

    public static IStreamSerializer<T> GetSerializerFor<T>(Type type, string tag = "")
    {
        return GetSerializersCollection(type, tag).GetRequiredService<IStreamSerializer<T>>();
    }

    public static T GetService<T>(Type type, string tag = "")
    {
        return GetSerializersCollection(type, tag).GetRequiredService<T>();
    }
}