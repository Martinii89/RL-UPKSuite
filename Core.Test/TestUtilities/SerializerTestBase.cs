using System;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Test.TestUtilities;

public class SerializerTestBase
{
    public IServiceProvider GetSerializersCollection(Type assembly, string tag = "")
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.UseSerializers(assembly, new SerializerOptions(tag));
        var services = serviceCollection.BuildServiceProvider();
        return services;
    }
}