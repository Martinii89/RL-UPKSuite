// See https://aka.ms/new-console-template for more information

using System.Text.Json;

using CommandLine;

using Core;
using Core.Serialization;
using Core.Serialization.Abstraction;
using Core.Serialization.RocketLeague;
using Core.Types;
using Core.Utility;

using DummyPackageBuilderCli;

using Microsoft.Extensions.DependencyInjection;

// var rlServices = GetRLSerializerCollection();
//
// var rlFileSummarySerializer = rlServices.GetRequiredService<IStreamSerializer<FileSummary>>();
// var rlPackageSerializer = rlServices.GetRequiredService<IStreamSerializer<UnrealPackage>>();
// var rLobjectSerializerFactory = rlServices.GetRequiredService<IObjectSerializerFactory>();
//
// var unpacker = new PackageUnpacker(rlFileSummarySerializer, new DecryptionProvider("Keys.txt"));
//
// var tagame = @"C:\Users\m4rti\OneDrive\RL\packages\tagame.upk";
// await using var input = new FileStream(tagame, FileMode.Open);
// var output = new MemoryStream();
//
// unpacker.Unpack(input, output);
// var sw = Stopwatch.StartNew();
// output.Position = 0;
// var package = rlPackageSerializer.Deserialize(output);
//
// var actorEventsPackage = package.ExportTable.FirstOrDefault(x => package.GetName(x.ObjectName) == "FXActorEvents");
//
//
// var fxActorEventsChildren = package.ExportTable.Where(x =>
// {
//     var outerIndex = x.OuterIndex;
//     var outer = package.GetObjectReference(outerIndex);
//     while (outer != null)
//     {
//         if (outer == actorEventsPackage)
//         {
//             return true;
//         }
//         var newOuterIndex = outer.OuterIndex;
//         outer = package.GetObjectReference(newOuterIndex);
//     }
//
//     return false;
// }).Select(x =>
// {
//     var clz = package.GetObjectReference(x.ClassIndex);
//     var name = package.GetFullName(clz);
//     return $"'{name}'{package.GetFullName(x)}";
// }).ToList();
// sw.Stop();
// Console.WriteLine(sw.Elapsed.TotalMilliseconds);

var parseResult = Parser.Default.ParseArguments<DummyPacakgeBuilderOptions>(args);
await parseResult.WithParsedAsync(BatchProcess);
return;

async Task BatchProcess(DummyPacakgeBuilderOptions options)
{
    var udkServices = GetUdkSerializerCollection();

    var serializer = udkServices.GetRequiredService<IStreamSerializer<UnrealPackage>>();
    var exportFactory = udkServices.GetRequiredService<PackageExporterFactory>();

    var nativeFactory = new NativeClassFactory();

    IObjectSerializerFactory objectSerializerFactory = udkServices.GetRequiredService<IObjectSerializerFactory>();
    var packageCacheOptions = new PackageCacheOptions(serializer, nativeFactory)
    {
        SearchPaths = { options.ScriptsDirectory },
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
            builder.AddObject(objectDefinition.GetGroup(), objectDefinition.GetObjectName(),
                objectDefinition.ClassFullName);
        }

        var package = builder.BuildPackage();
        var info = new DirectoryInfo(options.OutputDirectory);
        info.Create();
        await using var output = new FileStream(Path.Combine(options.OutputDirectory, $"{def.PackageName}.upk"),
            FileMode.OpenOrCreate);
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
    serviceCollection.AddSerializers(typeof(UnrealPackage), new SerializerOptions());
    serviceCollection.AddSingleton<IObjectSerializerFactory, ObjectSerializerFactory>();
    serviceCollection.AddSingleton<PackageExporterFactory>();
    var services = serviceCollection.BuildServiceProvider();
    return services;
}

IServiceProvider GetRLSerializerCollection()
{
    var serviceCollection = new ServiceCollection();
    serviceCollection.AddSerializers(typeof(UnrealPackage), new SerializerOptions(RocketLeagueBase.FileVersion));
    serviceCollection.AddSingleton<IObjectSerializerFactory, ObjectSerializerFactory>();
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
};