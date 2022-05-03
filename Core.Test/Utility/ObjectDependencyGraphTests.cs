using System.IO;
using Core.Serialization;
using Core.Types;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Core.Utility.Tests;

public class ObjectDependencyGraphTests
{
    [Fact]
    public void AddExportTest()
    {
        Assert.True(false, "This test needs an implementation");
    }

    [Fact]
    public void GetDotFileTest()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.UseSerializers(typeof(UnrealPackage), new SerializerOptions());
        var services = serviceCollection.BuildServiceProvider();
        var serializer = services.GetRequiredService<IStreamSerializerFor<UnrealPackage>>();
        var packageStream = File.OpenRead(@"TestData/UDK/Core.u");
        var package = serializer.Deserialize(packageStream);
        // Act

        var graph = new ObjectDependencyGraph();
        for (var i = 0; i < package.ExportTable.Count; i++)
        {
            graph.AddExport(package.ExportTable[i], i + 1);
        }

        File.WriteAllText("test.dot", graph.GetDotFile());

        // Assert
    }
}