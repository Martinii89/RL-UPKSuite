using Core.Classes.Core;
using Core.Types;

namespace Core.Classes.Engine;

[NativeOnlyClass("Engine", "SoundClass", typeof(UObject))]
public class USoundClass : UObject
{
    public USoundClass(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public Dictionary<UObject, object> EditorData { get; set; }
}