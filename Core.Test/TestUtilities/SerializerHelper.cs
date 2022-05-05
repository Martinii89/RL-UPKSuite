using System;
using Core.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Test.TestUtilities;

public class SerializerHelper
{
    private static IServiceProvider GetSerializersCollection(Type assembly, string tag = "")
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.UseSerializers(assembly, new SerializerOptions(tag));
        var services = serviceCollection.BuildServiceProvider();
        return services;
    }

    public static IStreamSerializerFor<T> GetSerializerFor<T>(Type type, string tag = "")
    {
        return GetSerializersCollection(type, tag).GetRequiredService<IStreamSerializerFor<T>>();
    }
}