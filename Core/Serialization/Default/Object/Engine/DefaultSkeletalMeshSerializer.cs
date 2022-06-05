using Core.Classes.Core;
using Core.Classes.Core.Structs;
using Core.Classes.Engine;
using Core.Classes.Engine.Structs;
using Core.Serialization.Abstraction;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultSkeletalMeshSerializer : BaseObjectSerializer<USkeletalMesh>
{
    private readonly IStreamSerializerFor<FBoxSphereBounds> _boxSphereBoundsSerializer;
    private readonly IStreamSerializerFor<FMeshBone> _meshBoneSerializer;
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerializer;
    private readonly IObjectSerializer<UObject> _objectSerializer;
    private readonly IStreamSerializerFor<FRotator> _rotatorSerializer;
    private readonly IStreamSerializerFor<FVector> _vectorSerializer;


    public DefaultSkeletalMeshSerializer(IStreamSerializerFor<FBoxSphereBounds> boxSphereBoundsSerializer,
        IStreamSerializerFor<ObjectIndex> objectIndexSerializer, IObjectSerializer<UObject> objectSerializer, IStreamSerializerFor<FRotator> rotatorSerializer,
        IStreamSerializerFor<FVector> vectorSerializer, IStreamSerializerFor<FMeshBone> meshBoneSerializer)
    {
        _boxSphereBoundsSerializer = boxSphereBoundsSerializer;
        _objectIndexSerializer = objectIndexSerializer;
        _objectSerializer = objectSerializer;
        _rotatorSerializer = rotatorSerializer;
        _vectorSerializer = vectorSerializer;
        _meshBoneSerializer = meshBoneSerializer;
    }

    public override void DeserializeObject(USkeletalMesh obj, Stream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);
        obj.BoxSphereBounds = _boxSphereBoundsSerializer.Deserialize(objectStream);
        obj.Materials = _objectIndexSerializer.ReadTArrayToList(objectStream).Select(x => obj.OwnerPackage.GetObject(x) as UMaterial).ToList();
        obj.Origin = _vectorSerializer.Deserialize(objectStream);
        obj.RotOrigin = _rotatorSerializer.Deserialize(objectStream);
        obj.RefSkeleton = _meshBoneSerializer.ReadTArrayToList(objectStream);
    }

    public override void SerializeObject(USkeletalMesh obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}