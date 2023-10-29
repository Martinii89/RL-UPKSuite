﻿using Core.Classes.Core;
using Core.Types;

namespace Core.Classes.Engine;

[NativeOnlyClass("Engine", "Pylon", typeof(ANavigationPoint))]
public class APylon : ANavigationPoint
{
    public APylon(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public UObject? NavMeshPtr { get; set; } //UNavigationMeshBase
    public UObject? ObstacleMesh { get; set; } //UNavigationMeshBase
    public UObject? DynamicObstacleMesh { get; set; } //UNavigationMeshBase
}