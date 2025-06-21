using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text.Json;

using RlUpk.Core;
using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Core.Structs;
using RlUpk.Core.Classes.Engine;
using RlUpk.Core.RocketLeague;
using RlUpk.Core.Types.PackageTables;

using SharpGLTF.Scenes;

namespace RlUpk.MapBuilder.Cli;

internal class MapBuilderProcessor(PackageLoader packageLoader, PackageUnpacker unpacker)
{

    public async Task ExportAssets(string file, string decryptedFolder, string assetFolder)
    {
        decryptedFolder = Path.GetFullPath(decryptedFolder);
        assetFolder = Path.GetFullPath(assetFolder);
        Directory.CreateDirectory(Path.GetDirectoryName(assetFolder)!);
        Directory.CreateDirectory(Path.GetDirectoryName(decryptedFolder)!);
        string decryptedPath = await DecryptPackage(file, decryptedFolder);
        await UmodelWrapper.ExportMeshes(decryptedPath, assetFolder);
    }

    private async Task<string> DecryptPackage(string file, string decryptedFolder, bool overwrite = false)
    {
        var fileName = Path.GetFileNameWithoutExtension(file);
        string decryptedPath = decryptedFolder + "/" + fileName + "_decrypted.upk";
        if (File.Exists(decryptedPath) && !overwrite)
        {
            return decryptedPath;
        }
        
        await using var packageStream = File.OpenRead(file);
        await using var outputStream = File.OpenWrite(decryptedPath);
        
        unpacker.Unpack(packageStream, outputStream);
        return decryptedPath;
    }

    public void ProcessFile(string packageFile, string assetsFolder)
    {
        var inputFileName = Path.GetFileNameWithoutExtension(packageFile);
        using var convertedStream = new MemoryStream();
        var package = packageLoader.LoadPackage(packageFile, inputFileName, false);
        if (package is null)
        {
            Console.WriteLine($"Failed to load package {inputFileName}");
            return;
        }

        var actors = FindObjectsOf("StaticMeshCollectionActor", package.ExportTable);
        var meshComponentDatas = actors.SelectMany(VisitMeshCollector)
            .Where(x => !x.Hidden)
            // .Take(4)
            .ToList();

        var modelFinder = new ModelFinder(Path.GetFullPath(assetsFolder));

        
        var merged = new SceneBuilder();
        
        var models = meshComponentDatas.Select(x => modelFinder.LoadSceneBuilder(x.StaticMeshName)).ToList();

        for (int i = 0; i < models.Count; i++)
        {
            var model = models[i];
            if (model is null) continue;
            var data = meshComponentDatas[i];
            var loc = data.Translation * 0.01f;
            try
            {
                Matrix4x4 sceneTransform = CreateTransformMatrix(loc, data.Rotation, data.Scale3d);
                merged.AddScene(model, sceneTransform);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // Save the combined model to a file
        string outputPath = Path.Combine(Path.GetDirectoryName(packageFile) ?? ".", $"{Path.GetFileNameWithoutExtension(packageFile)}_combined.gltf");
        merged.ToGltf2().Save(outputPath);

        Console.WriteLine($"Combined model saved to {outputPath}");

        string outputPath2 = Path.Combine(Path.GetDirectoryName(packageFile) ?? ".", $"{Path.GetFileNameWithoutExtension(packageFile)}_combined.json");
        using var jsonOut = File.CreateText(outputPath2);
        jsonOut.Write(JsonSerializer.Serialize(meshComponentDatas, new JsonSerializerOptions { WriteIndented = true }));
        // Console.WriteLine(JsonSerializer.Serialize(meshComponentDatas, new JsonSerializerOptions { WriteIndented = true }));
    }
    
    public Matrix4x4 CreateTransformMatrix(FVector translation, FRotator rotation, FVector scale)
    {
        float pitchRad = rotation.Pitch * (float)(Math.PI / 32768.0f);
        float yawRad = rotation.Yaw * (float)(Math.PI / 32768.0f);
        float rollRad = rotation.Roll * (float)(Math.PI / 32768.0f);

        Matrix4x4 rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(yawRad, pitchRad, rollRad);
        Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(scale.X, scale.Z, scale.Y);
        Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(translation.X, translation.Z, translation.Y);
        return scaleMatrix * rotationMatrix * translationMatrix;
    }

    private static IEnumerable<UObject> FindObjectsOf(string name, ExportTable exportTable)
    {
        return exportTable.Where(x => x.Object?.Class?.IsA(name) ?? false).Select(x => x.Object!);
    }

    private IEnumerable<MeshComponentData> VisitMeshCollector(UObject obj)
    {
        obj.Deserialize();
        var meshComponents = obj.ScriptProperties
            .Where(x => x.Name == "StaticMeshComponents")
            .SelectMany(x => x.Value as List<object> ?? [])
            .Cast<UStaticMeshComponent>();

        foreach (var meshComponent in meshComponents)
        {
            meshComponent.Deserialize();
            var hidden = GetScriptBool(meshComponent, "HiddenGame", false);
            var staticMesh = GetScriptProperty<UStaticMesh?>(meshComponent, "StaticMesh", null);
            if (staticMesh is null)
            {
                continue;
            }
            staticMesh.Deserialize();
            
            
            // var materials = GetScriptProperty<List<object>>(meshComponent, "Materials", []).Cast<UMaterialInterface>().ToList();
            // materials.ForEach(x => x?.Deserialize());
            var staticMeshFullName = staticMesh.ToString();
            var scale3d = GetScriptVector(meshComponent, "Scale3D", FVector.One);
            var translation = GetScriptVector(meshComponent, "Translation", FVector.Zero);
            var rotation = GetScriptRotator(meshComponent, "Rotation");
            yield return new MeshComponentData(staticMeshFullName, hidden, scale3d, translation, rotation);
        }
    }

    private bool TryFindScriptProperty<T>(UObject obj, string name, [MaybeNullWhen(false)] out T result)
    {
        var prop = obj.ScriptProperties.FirstOrDefault(x => x.Name == name);
        if (prop?.Value is T propValue)
        {
            result = propValue;
            return true;
        }

        result = default;
        return false;
    }

    private T GetScriptProperty<T>(UObject obj, string name, T defaultValue) => TryFindScriptProperty<T>(obj, name, out var result) ? result : defaultValue;

    private FVector GetScriptVector(UObject obj, string name, FVector defaultValue)
    {
        FVector vec = new();
        if (!TryFindScriptProperty<Dictionary<string, object>>(obj, name, out var result))
        {
            return new FVector()
            {
                X = defaultValue.X,
                Y = defaultValue.Y,
                Z = defaultValue.Z,
            };
        }

        vec.X = result.GetValueOrDefault("X") as float? ?? defaultValue.X;
        vec.Y = result.GetValueOrDefault("Y") as float? ?? defaultValue.Y;
        vec.Z = result.GetValueOrDefault("Z") as float? ?? defaultValue.Z;
        return vec;
    }

    private FRotator GetScriptRotator(UObject obj, string name)
    {
        FRotator vec = new();
        if (!TryFindScriptProperty<Dictionary<string, object>>(obj, name, out var result))
        {
            return vec;
        }

        vec.Pitch = result.GetValueOrDefault("Pitch") as int? ?? 0;
        vec.Roll = result.GetValueOrDefault("Roll") as int? ?? 0;
        vec.Yaw = result.GetValueOrDefault("Yaw") as int? ?? 0;
        return vec;
    }
    
    private bool GetScriptBool(UObject obj, string name, bool defaultValue)
    {
        FRotator vec = new();
        if (!TryFindScriptProperty<byte>(obj, name, out var result))
        {
            return defaultValue;
        }

        return result > 0;
    }
}

internal record MeshComponentData(string StaticMeshName, bool Hidden, FVector Scale3d, FVector Translation, FRotator Rotation);