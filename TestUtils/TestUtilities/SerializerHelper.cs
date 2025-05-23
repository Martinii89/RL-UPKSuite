using Core.Serialization;
using Core.Serialization.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Test.TestUtilities;

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