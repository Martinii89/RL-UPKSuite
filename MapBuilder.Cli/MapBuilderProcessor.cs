using System.Diagnostics;
using System.Numerics;
using System.Text.Json;

using RlUpk.Core;
using RlUpk.Core.Classes.Core.Structs;
using RlUpk.Core.Classes.Engine;
using RlUpk.Core.RocketLeague;

using SharpGLTF.Scenes;

namespace RlUpk.MapBuilder.Cli;

internal class MapBuilderProcessor(PackageLoader packageLoader, PackageUnpacker unpacker)
{
    public async Task ExportAssets(string file, string decryptedFolder, string assetFolder, string outputFolder)
    {
        decryptedFolder = Path.GetFullPath(decryptedFolder);
        assetFolder = Path.GetFullPath(assetFolder);
        outputFolder = Path.GetFullPath(outputFolder);
        Directory.CreateDirectory(Path.GetDirectoryName(assetFolder)!);
        Directory.CreateDirectory(Path.GetDirectoryName(decryptedFolder)!);
        Directory.CreateDirectory(Path.GetDirectoryName(outputFolder)!);
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

    public void ProcessFile(string packageFile, string assetsFolder, string outputFolder)
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

        var dataModelPair = meshComponentDatas
            .Where(x => !x.Hidden)
            // .Where(x =>  x.StaticMeshName.Contains("Plantpotter") ||  x.StaticMeshName.Contains("Bush"))
            // .Where(x =>  x.StaticMeshName.Contains("Plantpotter"))
            // .Where(x => x.StaticMeshName.Contains("Bush"))
            // .Take(1)
            .Select(x => (x, modelFinder.LoadSceneBuilder(x.StaticMeshName)))
            .ToList();

        for (int i = 0; i < dataModelPair.Count; i++)
        {
            var (data, model) = dataModelPair[i];
            if (model is null) continue;
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
        string outputPath = Path.Combine(Path.GetDirectoryName(outputFolder) ?? ".", $"{Path.GetFileNameWithoutExtension(packageFile)}_combined.gltf");
        merged.ToGltf2().Save(outputPath);

        Console.WriteLine($"Combined model saved to {outputPath}");

        string outputPath2 = Path.Combine(Path.GetDirectoryName(outputFolder) ?? ".", $"{Path.GetFileNameWithoutExtension(packageFile)}_combined.json");
        using var jsonOut = File.CreateText(outputPath2);
        jsonOut.Write(JsonSerializer.Serialize(dataModelPair.Select(x => x.x), new JsonSerializerOptions { WriteIndented = true }));
    }

    public Matrix4x4 CreateTransformMatrix(FVector translation, FRotator rotation, FVector scale)
    {
        var rotationQuat = rotation.ConvertToRightHandedQuaternion();
        var rotationMatrix = Matrix4x4.CreateFromQuaternion(rotationQuat);
        Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(scale.X, scale.Z, scale.Y);
        Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(translation.X, translation.Z, translation.Y);
        Matrix4x4 transformMatrix = scaleMatrix * rotationMatrix * translationMatrix;
        // var test = Matrix4x4.Decompose(transformMatrix, out Vector3 testScale, out Quaternion testRotation, out Vector3 testTranslation);
        return transformMatrix;
    }

    private const float CONST_PI_F = 3.1415926f;
    private const float CONST_DegToUnrRot = 182.0444f;
    private const float rotToRad = 0.00549316540360483f;
    private const float _90DegInRad = CONST_PI_F / 2;
    
}