// See https://aka.ms/new-console-template for more information

using System.Text.Json;

using CommandLine;

using Core;
using Core.Serialization;
using Core.Serialization.Abstraction;
using Core.Types;
using Core.Utility;

using DummyPackageBuilderCli;

using Microsoft.Extensions.DependencyInjection;

var parseResult = Parser.Default.ParseArguments<DummyPacakgeBuilderOptions>(args);
await parseResult.WithParsedAsync(BatchProcess);

async Task BatchProcess(DummyPacakgeBuilderOptions options)
{
    var udkServices = GetUdkSerializerCollection();

    var serializer = udkServices.GetRequiredService<IStreamSerializer<UnrealPackage>>();
    var exportFactory = udkServices.GetRequiredService<PackageExporterFactory>();

    var nativeFactory = new NativeClassFactory();

    IObjectSerializerFactory objectSerializerFactory = udkServices.GetRequiredService<IObjectSerializerFactory>();
    var packageCacheOptions = new PackageCacheOptions(serializer, nativeFactory)
    {
        SearchPaths =
        {
            options.ScriptsDirectory
        },
        GraphLinkPackages = true,
        ObjectSerializerFactory = objectSerializerFactory
    };
    var packageCache = new PackageCache(packageCacheOptions);

    await foreach (var def in ReadPackageDefinitions(options.PackageDefinitionFiles))
    {
        var builder = new DummyPackageBuilder(def.PackageName)
            .WithPackageCache(packageCache)
            .WithNativeFactory(nativeFactory);

        foreach (ObjectDefinition objectDefinition in def.PackageObjects)
        {
            builder.AddObject(objectDefinition.GetGroup(), objectDefinition.GetObjectName(), objectDefinition.ClassFullName);
        }

        var package = builder.BuildPackage();
        var info = new DirectoryInfo(options.OutputDirectory);
        info.Create();
        await using var output = new FileStream(Path.Combine(options.OutputDirectory, $"{def.PackageName}.upk"), FileMode.OpenOrCreate);
        var exporter = exportFactory.Create(package, output);
        exporter.ExportPackage();
        
    }
}

async IAsyncEnumerable<PackageDefinition> ReadPackageDefinitions(IEnumerable<string> optionsPackageDefinitionFiles)
{
    foreach (string file in optionsPackageDefinitionFiles)
    {
        var jsonString = File.Open(file, FileMode.Open);
        var def = await JsonSerializer.DeserializeAsync<PackageDefinition>(jsonString);
        if (def != null)
        {
            yield return def;
        }
    }
}



IServiceProvider GetUdkSerializerCollection()
{
    var serviceCollection = new ServiceCollection();
    serviceCollection.UseSerializers(typeof(UnrealPackage), new SerializerOptions());
    serviceCollection.AddSingleton<IObjectSerializerFactory, ObjectSerializerFactory>();
    serviceCollection.AddSingleton<PackageExporterFactory>();
    var services = serviceCollection.BuildServiceProvider();
    return services;
}

public class PackageDefinition
{
    public string PackageName { get; set; }

    public List<ObjectDefinition> PackageObjects { get; set; }
}

public record ObjectDefinition(string ObjectFullName, string ClassFullName)
{
    public string? GetGroup()
    {
        var groupSep = ObjectFullName.LastIndexOf('.');
        return groupSep == -1 ? null : ObjectFullName[..groupSep];
    }

    public string GetObjectName()
    {
        var groupSep = ObjectFullName.LastIndexOf('.');
        return ObjectFullName[(groupSep + 1)..];
    }

    // public string? ClassPackage
    // {
    //     get
    //     {
    //         var groupSep = ClassFullName.LastIndexOf('.');
    //         return groupSep == -1 ? null : ClassFullName[..groupSep];
    //     }
    // }
    //
    // public string ClassName
    // {
    //     get
    //     {
    //         var groupSep = ClassFullName.LastIndexOf('.');
    //         return ClassFullName[(groupSep + 1)..];
    //     }
    // }
};