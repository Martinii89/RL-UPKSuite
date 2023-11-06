﻿using Core.Classes.Core;
using Core.Types;

namespace Core.Classes.Engine;

[NativeOnlyClass("Engine", "PrefabInstance", typeof(AActor))]
public class APrefabInstance : AActor
{
    public APrefabInstance(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public Dictionary<UObject, UObject?> ArchetypeToInstanceMap { get; set; }
    public Dictionary<UObject, int> PI_ObjectMap { get; set; }
}