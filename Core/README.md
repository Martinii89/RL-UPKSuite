# RL-UPKSuite Core

The Core library for the RL-UPKSuite toolset, which provides functionality for unpacking, analyzing, and manipulating Rocket League UPK (Unreal Package) files.

## Overview

This library serves as the foundation for all RL-UPKSuite tools, providing classes and utilities to:

- Load and unpack Rocket League package files
- Deserialize and serialize Unreal Engine data structures
- Detect game versions
- Work with native UE3 classes
- Handle binary I/O operations for UPK files

## Key Components

- **PackageLoader**: Handles loading and unpacking of Rocket League UPK files
- **VersionDetector**: Identifies the version of Rocket League associated with package files
- **IPackageUnpacker**: Interface for different unpacking strategies
- **INativeClassFactory**: Factory interface for creating native class instances

## Usage

### Decrypt a package:
```csharp
// Example usage of the Core library

var decryptionProvider = new DecryptionProvider("keys.txt");
var decrptor = new RLPackageUnpacker(fileStream, decryptionProvider, FileSummarySerializer.GetDefaultSerializer());
upkFile.Unpack(outputStream);
```

### Load package

TODO: Simplify creation of PackageLoader
```csharp

var rlServices = GetRLSerializerCollection();
var rlFileSummarySerializer = rlServices.GetRequiredService<IStreamSerializer<FileSummary>>();
var rlPackageSerializer = rlServices.GetRequiredService<IStreamSerializer<UnrealPackage>>();
var rLobjectSerializerFactory = rlServices.GetRequiredService<IObjectSerializerFactory>();

var unpacker = new PackageUnpacker(rlFileSummarySerializer, new DecryptionProvider("keys.txt"));
var nativeFactory = new NativeClassFactory();

var cacheOptions = new PackageCacheOptions(rlPackageSerializer, nativeFactory)
{
     SearchPaths = { "path/to/folder/with/packages" },
     GraphLinkPackages = true,
     PackageUnpacker = unpacker,
     NativeClassFactory = nativeFactory,
     ObjectSerializerFactory = rLobjectSerializerFactory,
     PackageBlacklist = { "EngineMaterials", "EngineResources" }
};
var packageCache = new PackageCache(cacheOptions);
var loader = new PackageLoader(rlPackageSerializer, packageCache, unpacker, nativeFactory, rLobjectSerializerFactory);

var package = loader.LoadPackage("path/to/park_p.udk", "park_p");

 IServiceProvider GetRLSerializerCollection()
 {
     var serviceCollection = new ServiceCollection();
     serviceCollection.AddSerializers(typeof(UnrealPackage), new SerializerOptions(RocketLeagueBase.FileVersion));
     serviceCollection.AddSingleton<IObjectSerializerFactory, ObjectSerializerFactory>();
     var services = serviceCollection.BuildServiceProvider();
     return services;
 }
```

## Dependencies

The Core library depends on various .NET libraries for handling binary serialization, compression, and other operations required for working with UPK files.

## Related Projects

- **Decryptor**: Tool for decrypting protected Rocket League packages
- **RLToUdkConverter**: Converts Rocket League packages to UDK-compatible format
- **DummyPackageBuilder**: Creates test UPK files for development purposes

## License

See the LICENSE file in the root project directory for license information.

## Contributing

Contributions are welcome. Please follow the project code style and submit pull requests for new features or bug fixes.
