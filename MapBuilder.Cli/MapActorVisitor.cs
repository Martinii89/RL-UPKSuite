using System.Diagnostics.CodeAnalysis;

using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Core.Structs;
using RlUpk.Core.Classes.Engine;

namespace RlUpk.MapBuilder.Cli;



internal class MapActorVisitor
{
    
    internal Dictionary<string, int> UnhandledTypes { get; } = new ();
    
    public IEnumerable<MeshComponentData> Visit(AActor actor)
    {
        var clz = actor.Class;
        if (clz is null)
        {
            yield break;
        }
        if (actor.Class != null && actor.Class.IsA("StaticMeshCollectionActor"))
        {
            foreach (MeshComponentData meshComponentData in VisitMeshCollector(actor))
            {
                yield return meshComponentData;
            }
            yield break;
        }

        if (!UnhandledTypes.TryAdd(clz.Name, 1))
        {
            UnhandledTypes[clz.Name]++;
        }
    }
    
    private IEnumerable<MeshComponentData> VisitMeshCollector(AActor obj)
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