﻿using Core.Classes.Core;
using Core.Types;

namespace Core.Classes.Engine;

[NativeOnlyClass("Engine", "DominantDirectionalLightComponent", typeof(UDirectionalLightComponent))]
public class UDominantDirectionalLightComponent : UDirectionalLightComponent
{
    public UDominantDirectionalLightComponent(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null)
        : base(name, @class, outer, ownerPackage, objectArchetype)
    {
    }

    public List<ushort> DominantLightShadowMap { get; set; } = new();
}