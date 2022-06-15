﻿using System.Diagnostics;
using Core.Classes.Core;
using Core.Classes.Core.Structs;
using Core.Classes.Engine;
using Core.Classes.Engine.Structs;
using Core.Serialization.Abstraction;
using Core.Serialization.Extensions;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultSkeletalMeshSerializer : BaseObjectSerializer<USkeletalMesh>
{
    private readonly IStreamSerializer<FBoxSphereBounds> _boxSphereBoundsSerializer;
    private readonly IStreamSerializer<FStaticLodModel> _lodSerializer;
    private readonly IStreamSerializer<FMeshBone> _meshBoneSerializer;
    private readonly IStreamSerializer<FName> _nameSerializer;
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerializer;
    private readonly IObjectSerializer<UObject> _objectSerializer;
    private readonly IStreamSerializer<FRotator> _rotatorSerializer;
    private readonly IStreamSerializer<FVector> _vectorSerializer;
    private USkeletalMesh? _currentMesh;


    public DefaultSkeletalMeshSerializer(IStreamSerializer<FBoxSphereBounds> boxSphereBoundsSerializer,
        IStreamSerializer<ObjectIndex> objectIndexSerializer, IObjectSerializer<UObject> objectSerializer, IStreamSerializer<FRotator> rotatorSerializer,
        IStreamSerializer<FVector> vectorSerializer, IStreamSerializer<FMeshBone> meshBoneSerializer, IStreamSerializer<FStaticLodModel> lodSerializer,
        IStreamSerializer<FName> nameSerializer)
    {
        _boxSphereBoundsSerializer = boxSphereBoundsSerializer;
        _objectIndexSerializer = objectIndexSerializer;
        _objectSerializer = objectSerializer;
        _rotatorSerializer = rotatorSerializer;
        _vectorSerializer = vectorSerializer;
        _meshBoneSerializer = meshBoneSerializer;
        _lodSerializer = lodSerializer;
        _nameSerializer = nameSerializer;
    }

    public override void DeserializeObject(USkeletalMesh obj, IUnrealPackageStream objectStream)
    {
        _currentMesh = obj;
        _objectSerializer.DeserializeObject(obj, objectStream);
        if (obj.ScriptProperties.FindIndex(x => x.Name == "bHasVertexColors") >= 0)
        {
            //Add color stream thing
            Debugger.Break();
        }

        obj.BoxSphereBounds = _boxSphereBoundsSerializer.Deserialize(objectStream.BaseStream);
        obj.Materials = _objectIndexSerializer.ReadTArrayToList(objectStream.BaseStream).Select(x => obj.OwnerPackage.GetObject(x) as UMaterialInterface)
            .ToList();
        obj.Origin = _vectorSerializer.Deserialize(objectStream.BaseStream);
        obj.RotOrigin = _rotatorSerializer.Deserialize(objectStream.BaseStream);
        obj.RefSkeleton = _meshBoneSerializer.ReadTArrayToList(objectStream.BaseStream);
        obj.SkeletalDepth = objectStream.ReadInt32();
        obj.LODModels = _lodSerializer.ReadTArrayToList(objectStream.BaseStream);
        obj.NameMap = objectStream.BaseStream.ReadDictionary(ReadName, stream => stream.ReadInt32());
        var PerPolyBoneKDOPsCount = objectStream.ReadInt32();
        var BoneBreakNamesCount = objectStream.ReadInt32();
        var BoneBreakOptionsCount = objectStream.ReadInt32();
        obj.ClothingAssets =
            objectStream.BaseStream.ReadTarray(stream => obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream.BaseStream)));
        DropRamainingNativeData(obj, objectStream.BaseStream);
        _currentMesh = null;
    }

    public override void SerializeObject(USkeletalMesh obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }

    private string ReadName(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(_currentMesh);
        return _currentMesh.OwnerPackage.GetName(_nameSerializer.Deserialize(stream));
    }
}