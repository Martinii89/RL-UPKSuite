using System.Diagnostics;
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
    private readonly IStreamSerializerFor<FBoxSphereBounds> _boxSphereBoundsSerializer;
    private readonly IStreamSerializerFor<FStaticLodModel> _lodSerializer;
    private readonly IStreamSerializerFor<FMeshBone> _meshBoneSerializer;
    private readonly IStreamSerializerFor<FName> _nameSerializer;
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerializer;
    private readonly IObjectSerializer<UObject> _objectSerializer;
    private readonly IStreamSerializerFor<FRotator> _rotatorSerializer;
    private readonly IStreamSerializerFor<FVector> _vectorSerializer;
    private USkeletalMesh? _currentMesh;


    public DefaultSkeletalMeshSerializer(IStreamSerializerFor<FBoxSphereBounds> boxSphereBoundsSerializer,
        IStreamSerializerFor<ObjectIndex> objectIndexSerializer, IObjectSerializer<UObject> objectSerializer, IStreamSerializerFor<FRotator> rotatorSerializer,
        IStreamSerializerFor<FVector> vectorSerializer, IStreamSerializerFor<FMeshBone> meshBoneSerializer, IStreamSerializerFor<FStaticLodModel> lodSerializer,
        IStreamSerializerFor<FName> nameSerializer)
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

    public override void DeserializeObject(USkeletalMesh obj, Stream objectStream)
    {
        _currentMesh = obj;
        _objectSerializer.DeserializeObject(obj, objectStream);
        if (obj.ScriptProperties.FindIndex(x => x.Name == "bHasVertexColors") >= 0)
        {
            //Add color stream thing
            Debugger.Break();
        }

        obj.BoxSphereBounds = _boxSphereBoundsSerializer.Deserialize(objectStream);
        obj.Materials = _objectIndexSerializer.ReadTArrayToList(objectStream).Select(x => obj.OwnerPackage.GetObject(x) as UMaterialInterface).ToList();
        obj.Origin = _vectorSerializer.Deserialize(objectStream);
        obj.RotOrigin = _rotatorSerializer.Deserialize(objectStream);
        obj.RefSkeleton = _meshBoneSerializer.ReadTArrayToList(objectStream);
        obj.SkeletalDepth = objectStream.ReadInt32();
        obj.LODModels = _lodSerializer.ReadTArrayToList(objectStream);
        obj.NameMap = objectStream.ReadDictionary(ReadName, stream => stream.ReadInt32());
        var PerPolyBoneKDOPsCount = objectStream.ReadInt32();
        var BoneBreakNamesCount = objectStream.ReadInt32();
        var BoneBreakOptionsCount = objectStream.ReadInt32();
        obj.ClothingAssets = objectStream.ReadTarray(stream => obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream)));
        DropRamainingNativeData(obj, objectStream);
        _currentMesh = null;
    }

    public override void SerializeObject(USkeletalMesh obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }

    private string ReadName(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(_currentMesh);
        return _currentMesh.OwnerPackage.GetName(_nameSerializer.Deserialize(stream));
    }
}