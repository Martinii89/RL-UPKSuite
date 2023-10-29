﻿using System.Runtime.CompilerServices;
using Core.Classes.Core;
using Core.Classes.Core.Properties;
using Core.Classes.Core.Structs;
using Core.Classes.Engine.Structs;
using Core.Types;

namespace Core.Classes.Engine;

[AttributeUsage(AttributeTargets.Property)]
public class NativeProperty : Attribute
{
    public NativeProperty(PropertyType propertyType, [CallerMemberName] string? propertyName = null)
    {
        PropertyType = propertyType;
        PropertyName = propertyName ?? string.Empty;
    }

    public PropertyType PropertyType { get; init; }
    public string PropertyName { get; init; }

    public UProperty? CreateProperty(UStruct clz)
    {
        var propName = clz.OwnerPackage.GetOrAddName(PropertyName);
        var corePackage = clz.OwnerPackage.PackageName == "Core" ? clz.OwnerPackage : clz.OwnerPackage.PackageCache?.ResolveExportPackage("Core");
        if (corePackage is null)
        {
            return null;
        }

        var propertyClass = FindPropertyClass(PropertyType, corePackage);
        var prop = propertyClass?.NewInstance(propName, clz, clz.OwnerPackage, null) as UProperty;
        return prop;
    }

    private UClass? FindPropertyClass(PropertyType type, UnrealPackage corePackage)
    {
        var typeName = Enum.GetName(type);
        return typeName == null ? null : corePackage.FindClass(typeName);
    }
}

[NativeOnlyClass("Engine", "StaticMesh", typeof(UObject))]
public class UStaticMesh : UObject
{
    public UStaticMesh(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public uint HighResSourceMeshCRC { get; set; }
    public int Unk2 { get; set; }
    public List<int> UnkIntArray { get; set; } = new();
    public int Unk3 { get; set; }
    public int Unk4 { get; set; }
    public int Unk5 { get; set; }

    public FBoxSphereBounds FBoxSphereBounds { get; set; } = new();

    [NativeProperty(PropertyType.ObjectProperty)]
    public URB_BodySetup? BodySetup { get; set; }

    public FkDOPBounds FkDopBounds { get; set; } = new();
    public TArray<FkDOPNode> NewNodes { get; set; } = new();
    public TArray<FkDOPTriangles> Triangles { get; set; } = new();
    public int InternalVersion { get; set; }
    public int UnkFlag { get; set; }
    public int F178ElementsCount { get; set; } // TArray<FStaticMeshUnk5> f178;
    public int F74 { get; set; }
    public int Unk { get; set; }
    public List<FStaticMeshLODModel> LODModels { get; set; } = new();
    public byte[] UnknownBytes { get; set; }

    // script properties
    [NativeProperty(PropertyType.BoolProperty)]
    public bool UseSimpleLineCollision { get; set; }

    [NativeProperty(PropertyType.BoolProperty)]
    public bool UseSimpleBoxCollision { get; set; }

    [NativeProperty(PropertyType.BoolProperty)]
    public bool UseSimpleRigidBodyCollision { get; set; }

    [NativeProperty(PropertyType.BoolProperty)]
    public bool bStripComplexCollisionForConsole { get; set; }

    [NativeProperty(PropertyType.BoolProperty)]
    public bool UseFullPrecisionUVs { get; set; }

    [NativeProperty(PropertyType.BoolProperty)]
    public bool bUsedForInstancing { get; set; }

    [NativeProperty(PropertyType.BoolProperty)]
    public bool bUseMaximumStreamingTexelRatio { get; set; }

    [NativeProperty(PropertyType.BoolProperty)]
    public bool bPartitionForEdgeGeometry { get; set; }

    [NativeProperty(PropertyType.BoolProperty)]
    public bool bCanBecomeDynamic { get; set; }

    [NativeProperty(PropertyType.IntProperty)]
    public int LightMapResolution { get; set; }

    [NativeProperty(PropertyType.IntProperty)]
    public int LightMapCoordinateIndex { get; set; }

    public int LodInfoCount { get; set; }

    public FRotator ThumbnailAngle { get; set; }

    public float ThumbnailDistance { get; set; }

    public string HighResSourceMeshName { get; set; }

    public FGuid LightingGuid { get; set; }
}

//public class FStaticMeshLODInfo
//{
//    public List<FStaticMeshLODElement> Elements { get; set; }
//}

//public class FStaticMeshLODElement
//{
//    public UMaterialInterface Material { get; set; }
//    public bool bEnableShadowCasting { get; set; }
//    public bool bEnableCollision { get; set; }
//}