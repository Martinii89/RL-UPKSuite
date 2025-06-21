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

        var visitor = new MapActorVisitor();
        var actors = package.ExportTable.Select(x => x.Object).OfType<AActor>();
        var meshComponentDatas = actors.SelectMany(x => visitor.Visit(x)).ToList();

        foreach (var unhandledType in visitor.UnhandledTypes.OrderBy(x => x.Value))
        {
            Console.WriteLine($"No visitor handler for: {unhandledType.Key}. count: {unhandledType.Value}");
        }
        

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
}