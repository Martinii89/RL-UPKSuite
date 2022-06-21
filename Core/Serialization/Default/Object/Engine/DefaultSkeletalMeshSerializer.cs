using System.Diagnostics;
using Core.Classes.Core;
using Core.Classes.Core.Structs;
using Core.Classes.Engine;
using Core.Classes.Engine.Structs;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultSkeletalMeshSerializer : BaseObjectSerializer<USkeletalMesh>
{
    private readonly IStreamSerializer<FBoxSphereBounds> _boxSphereBoundsSerializer;
    private readonly IObjectSerializer<FStaticLodModel> _lodSerializer;
    private readonly IStreamSerializer<FMeshBone> _meshBoneSerializer;
    private readonly IObjectSerializer<UObject> _objectSerializer;
    private readonly IStreamSerializer<FRotator> _rotatorSerializer;
    private readonly IStreamSerializer<FVector> _vectorSerializer;
    private USkeletalMesh? _currentMesh;


    public DefaultSkeletalMeshSerializer(IStreamSerializer<FBoxSphereBounds> boxSphereBoundsSerializer, IObjectSerializer<UObject> objectSerializer,
        IStreamSerializer<FRotator> rotatorSerializer,
        IStreamSerializer<FVector> vectorSerializer, IStreamSerializer<FMeshBone> meshBoneSerializer, IObjectSerializer<FStaticLodModel> lodSerializer)
    {
        _boxSphereBoundsSerializer = boxSphereBoundsSerializer;
        _objectSerializer = objectSerializer;
        _rotatorSerializer = rotatorSerializer;
        _vectorSerializer = vectorSerializer;
        _meshBoneSerializer = meshBoneSerializer;
        _lodSerializer = lodSerializer;
    }

    /// <inheritdoc />
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
        obj.Materials = objectStream.ReadTArray(stream => stream.ReadObject() as UMaterialInterface);
        obj.Origin = _vectorSerializer.Deserialize(objectStream.BaseStream);
        obj.RotOrigin = _rotatorSerializer.Deserialize(objectStream.BaseStream);
        obj.RefSkeleton = _meshBoneSerializer.ReadTArrayToList(objectStream.BaseStream);
        obj.SkeletalDepth = objectStream.ReadInt32();
        obj.LODModels = _lodSerializer.ReadTArrayToList(objectStream);
        obj.NameMap = objectStream.ReadDictionary(stream => stream.ReadFNameStr(), stream => stream.ReadInt32());
        var PerPolyBoneKDOPsCount = objectStream.ReadInt32();
        var BoneBreakNamesCount = objectStream.ReadInt32();
        var BoneBreakOptionsCount = objectStream.ReadInt32();
        obj.ClothingAssets = objectStream.ReadTArray(stream => stream.ReadObject());
        DropRamainingNativeData(obj, objectStream.BaseStream);
        _currentMesh = null;
    }

    /// <inheritdoc />
    public override void SerializeObject(USkeletalMesh obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}