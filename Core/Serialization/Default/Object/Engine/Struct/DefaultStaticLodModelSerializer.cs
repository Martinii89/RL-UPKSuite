using System.Diagnostics;

using RlUpk.Core.Classes.Core.Structs;
using RlUpk.Core.Classes.Engine.Structs;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Serialization.Extensions;

namespace RlUpk.Core.Serialization.Default.Object.Engine.Struct;

public class DefaultStaticLodModelSerializer : BaseObjectSerializer<FStaticLodModel>
{
    private readonly IStreamSerializer<FByteBulkData> _BulkDataSerializer;
    private readonly IStreamSerializer<FColor> _colorSerializer;
    private readonly IObjectSerializer<FSkeletalMeshVertexBuffer> _SkeletalMeshVertexBufferSerializer;
    private readonly IStreamSerializer<FSkelIndexBuffer> _SkelIndexBufferSerializer;
    private readonly IStreamSerializer<FSkelMeshChunk> _SkelMeshChunkSerializer;
    private readonly IStreamSerializer<FSkelMeshSection> _SkelMeshSectionSerializer;


    public DefaultStaticLodModelSerializer(IStreamSerializer<FSkelMeshSection> skelMeshSectionSerializer,
        IStreamSerializer<FSkelIndexBuffer> skelIndexBufferSerializer, IStreamSerializer<FSkelMeshChunk> skelMeshChunkSerializer,
        IStreamSerializer<FByteBulkData> bulkDataSerializer, IObjectSerializer<FSkeletalMeshVertexBuffer> skeletalMeshVertexBufferSerializer,
        IStreamSerializer<FColor> colorSerializer)
    {
        _SkelMeshSectionSerializer = skelMeshSectionSerializer;
        _SkelIndexBufferSerializer = skelIndexBufferSerializer;
        _SkelMeshChunkSerializer = skelMeshChunkSerializer;
        _BulkDataSerializer = bulkDataSerializer;
        _SkeletalMeshVertexBufferSerializer = skeletalMeshVertexBufferSerializer;
        _colorSerializer = colorSerializer;
    }


    /// <inheritdoc />
    public override void DeserializeObject(FStaticLodModel obj, IUnrealPackageStream objectStream)
    {
        obj.Sections = _SkelMeshSectionSerializer.ReadTArrayToList(objectStream.BaseStream);
        obj.IndexBuffer = _SkelIndexBufferSerializer.Deserialize(objectStream.BaseStream);
        if (obj.IndexBuffer.Size is not 4 and not 2 || obj.IndexBuffer.Indices.ElementSize is not 4 and not 2)
        {
            Debugger.Break();
        }

        obj.UsedBones = objectStream.ReadTArray(objectStream1 => objectStream1.ReadInt16());
        obj.Chunks = _SkelMeshChunkSerializer.ReadTArrayToList(objectStream.BaseStream);
        obj.Size = objectStream.ReadInt32();
        obj.NumVerts = objectStream.ReadInt32();
        obj.RequiredBones = objectStream.ReadTArray(objectStream1 => objectStream1.ReadByte());
        obj.FBulkData = _BulkDataSerializer.Deserialize(objectStream.BaseStream);
        obj.NumUvSets = objectStream.ReadInt32();
        _SkeletalMeshVertexBufferSerializer.DeserializeObject(obj.GpuSkin, objectStream);

        if (obj.OwnerHasVertexColors)
        {
            obj.VertexColor = _colorSerializer.ReadTArrayWithElementSize(objectStream.BaseStream);
        }


        obj.ExtraVertexInfluencesCount = objectStream.ReadInt32();
        if (obj.ExtraVertexInfluencesCount > 0)
        {
            Debugger.Break();
        }

        obj.AdjacencyIndexBuffer = _SkelIndexBufferSerializer.Deserialize(objectStream.BaseStream);
    }

    /// <inheritdoc />
    public override void SerializeObject(FStaticLodModel obj, IUnrealPackageStream objectStream)
    {
        objectStream.WriteTArray(obj.Sections, _SkelMeshSectionSerializer);
        _SkelIndexBufferSerializer.Serialize(objectStream.BaseStream, obj.IndexBuffer);
        objectStream.WriteTArray(obj.UsedBones, (stream, s) => stream.WriteInt16(s));
        objectStream.WriteTArray(obj.Chunks, _SkelMeshChunkSerializer);
        objectStream.WriteInt32(obj.Size);
        objectStream.WriteInt32(obj.NumVerts);
        objectStream.WriteTArray(obj.RequiredBones, (stream, s) => stream.WriteByte(s));
        _BulkDataSerializer.Serialize(objectStream.BaseStream, obj.FBulkData);
        objectStream.WriteInt32(obj.NumUvSets);
        _SkeletalMeshVertexBufferSerializer.SerializeObject(obj.GpuSkin, objectStream);

        if (obj.OwnerHasVertexColors)

        {
            _colorSerializer.BulkWriteTArray(objectStream.BaseStream, obj.VertexColor);
        }

        objectStream.WriteInt32(obj.ExtraVertexInfluencesCount);
        _SkelIndexBufferSerializer.Serialize(objectStream.BaseStream, obj.AdjacencyIndexBuffer);
    }
}

public class DefaultSkelMeshSectionSerializer : IStreamSerializer<FSkelMeshSection>
{
    /// <inheritdoc />
    public FSkelMeshSection Deserialize(Stream stream)
    {
        return new FSkelMeshSection
        {
            MaterialIndex = stream.ReadUInt16(),
            ChunkIndex = stream.ReadUInt16(),
            FirstIndex = stream.ReadInt32(),
            NumTriangles = stream.ReadInt32(),
            TriangleSorting = (byte) stream.ReadByte()
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FSkelMeshSection value)
    {
        stream.WriteUInt16(value.MaterialIndex);
        stream.WriteUInt16(value.ChunkIndex);
        stream.WriteInt32(value.FirstIndex);
        stream.WriteInt32(value.NumTriangles);
        stream.WriteByte(value.TriangleSorting);
    }
}

public class DefaultSkelIndexBufferSerializer : IStreamSerializer<FSkelIndexBuffer>
{
    /// <inheritdoc />
    public FSkelIndexBuffer Deserialize(Stream stream)
    {
        var buffer = new FSkelIndexBuffer();
        buffer.Unk = stream.ReadInt32();
        buffer.Size = (byte) stream.ReadByte();
        buffer.Indices = buffer.Size switch
        {
            4 => stream.ReadTarrayWithElementSize(stream1 => stream1.ReadUInt32()),
            2 => stream.ReadTarrayWithElementSize(stream1 => (uint) stream1.ReadUInt16()),
            _ => throw new InvalidDataException()
        };

        return buffer;
    }

    public void Serialize(Stream stream, FSkelIndexBuffer value)
    {
        stream.WriteInt32(value.Unk);
        stream.WriteByte(value.Size);
        switch (value.Size)
        {
            case 4:
                stream.BulkWriteTArray(value.Indices, (stream1, u) => stream1.WriteUInt32(u));
                break;
            case 2:
                stream.BulkWriteTArray(value.Indices, (stream1, u) => stream1.WriteUInt16((ushort) u));
                break;
            default:
                throw new InvalidDataException();
        }
    }
}

public class DefaultSkelMeshChunkSerializer : IStreamSerializer<FSkelMeshChunk>
{
    private readonly IStreamSerializer<FRigidVertex> _rigidVertexSerializer;
    private readonly IStreamSerializer<FSoftVertex> _softVertexSerializer;

    public DefaultSkelMeshChunkSerializer(IStreamSerializer<FRigidVertex> rigidVertexSerializer, IStreamSerializer<FSoftVertex> softVertexSerializer)
    {
        _rigidVertexSerializer = rigidVertexSerializer;
        _softVertexSerializer = softVertexSerializer;
    }

    /// <inheritdoc />
    public FSkelMeshChunk Deserialize(Stream stream)
    {
        return new FSkelMeshChunk
        {
            FirstVertex = stream.ReadInt32(),
            RigidVerts = _rigidVertexSerializer.ReadTArrayToList(stream),
            SoftVerts = _softVertexSerializer.ReadTArrayToList(stream),
            Bones = stream.ReadTarray(stream1 => stream1.ReadInt16()),
            NumRigidVerts = stream.ReadInt32(),
            NumSoftVerts = stream.ReadInt32(),
            MaxInfluences = stream.ReadInt32()
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FSkelMeshChunk value)
    {
        stream.WriteInt32(value.FirstVertex);
        _rigidVertexSerializer.WriteTArray(stream, value.RigidVerts.ToArray());
        _softVertexSerializer.WriteTArray(stream, value.SoftVerts.ToArray());
        stream.WriteTArray(value.Bones, (stream1, s) => stream1.WriteInt16(s));
        stream.WriteInt32(value.NumRigidVerts);
        stream.WriteInt32(value.NumSoftVerts);
        stream.WriteInt32(value.MaxInfluences);
    }
}

public class DefaultRigidVertexSerializer : IStreamSerializer<FRigidVertex>
{
    private readonly IStreamSerializer<FColor> _colorSerializer;
    private readonly IStreamSerializer<FVector2D> _vector2DSerializer;
    private readonly IStreamSerializer<FVector> _vectorSerializer;

    public DefaultRigidVertexSerializer(IStreamSerializer<FVector> vectorSerializer, IStreamSerializer<FVector2D> vector2DSerializer,
        IStreamSerializer<FColor> colorSerializer)
    {
        _vectorSerializer = vectorSerializer;
        _vector2DSerializer = vector2DSerializer;
        _colorSerializer = colorSerializer;
    }

    /// <inheritdoc />
    public FRigidVertex Deserialize(Stream stream)
    {
        var vertex = new FRigidVertex();
        vertex.Pos = _vectorSerializer.Deserialize(stream);
        vertex.Normal = stream.ReadUInt32s(3);
        vertex.UV[0] = _vector2DSerializer.Deserialize(stream);
        vertex.UV[1] = _vector2DSerializer.Deserialize(stream);
        vertex.UV[2] = _vector2DSerializer.Deserialize(stream);
        vertex.UV[3] = _vector2DSerializer.Deserialize(stream);
        vertex.Color = _colorSerializer.Deserialize(stream);
        vertex.BoneIndex = (byte) stream.ReadByte();
        return vertex;
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FRigidVertex value)
    {
        _vectorSerializer.Serialize(stream, value.Pos);
        stream.WriteUInt32s(value.Normal);
        _vector2DSerializer.Serialize(stream, value.UV[0]);
        _vector2DSerializer.Serialize(stream, value.UV[1]);
        _vector2DSerializer.Serialize(stream, value.UV[2]);
        _vector2DSerializer.Serialize(stream, value.UV[3]);
        _colorSerializer.Serialize(stream, value.Color);
        stream.WriteByte(value.BoneIndex);
    }
}

public class DefaultSoftVertexSerializer : IStreamSerializer<FSoftVertex>
{
    private readonly IStreamSerializer<FColor> _colorSerializer;
    private readonly IStreamSerializer<FVector2D> _vector2DSerializer;
    private readonly IStreamSerializer<FVector> _vectorSerializer;

    public DefaultSoftVertexSerializer(IStreamSerializer<FVector> vectorSerializer, IStreamSerializer<FVector2D> vector2DSerializer,
        IStreamSerializer<FColor> colorSerializer)
    {
        _vectorSerializer = vectorSerializer;
        _vector2DSerializer = vector2DSerializer;
        _colorSerializer = colorSerializer;
    }

    /// <inheritdoc />
    public FSoftVertex Deserialize(Stream stream)
    {
        var vertex = new FSoftVertex();
        vertex.Pos = _vectorSerializer.Deserialize(stream);
        vertex.Normal = stream.ReadUInt32s(3);
        vertex.UV[0] = _vector2DSerializer.Deserialize(stream);
        vertex.UV[1] = _vector2DSerializer.Deserialize(stream);
        vertex.UV[2] = _vector2DSerializer.Deserialize(stream);
        vertex.UV[3] = _vector2DSerializer.Deserialize(stream);
        vertex.Color = _colorSerializer.Deserialize(stream);
        vertex.BoneIndex = stream.ReadBytes(4);
        vertex.BoneWeight = stream.ReadBytes(4);
        return vertex;
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FSoftVertex value)
    {
        _vectorSerializer.Serialize(stream, value.Pos);
        stream.WriteUInt32s(value.Normal);
        _vector2DSerializer.Serialize(stream, value.UV[0]);
        _vector2DSerializer.Serialize(stream, value.UV[1]);
        _vector2DSerializer.Serialize(stream, value.UV[2]);
        _vector2DSerializer.Serialize(stream, value.UV[3]);
        _colorSerializer.Serialize(stream, value.Color);
        stream.WriteBytes(value.BoneIndex);
        stream.WriteBytes(value.BoneWeight);
    }
}

public class DefaultSkeletalMeshVertexBufferSerializer : BaseObjectSerializer<FSkeletalMeshVertexBuffer>
{
    private readonly IObjectSerializer<GpuVert> _gpuVertSerializer;
    private readonly IStreamSerializer<FVector> _vectorSerializer;

    public DefaultSkeletalMeshVertexBufferSerializer(IStreamSerializer<FVector> vectorSerializer, IObjectSerializer<GpuVert> gpuVertSerializer)
    {
        _vectorSerializer = vectorSerializer;
        _gpuVertSerializer = gpuVertSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(FSkeletalMeshVertexBuffer obj, IUnrealPackageStream objectStream)
    {
        obj.NumUVSets = objectStream.ReadInt32();
        obj.bUseFullPrecisionUVs = objectStream.ReadInt32();
        obj.bUsePackedPosition = objectStream.ReadInt32();
        if (obj.bUseFullPrecisionUVs != 0 && obj.bUsePackedPosition != 1)
        {
            //implement all the vertex buffer types
            Debugger.Break();
        }

        obj.MeshExtension = _vectorSerializer.Deserialize(objectStream.BaseStream);
        obj.MeshOrigin = _vectorSerializer.Deserialize(objectStream.BaseStream);
        obj.VertexBuffer = objectStream.BulkReadTArray(objectStream1 =>
        {
            var gpuVert = new GpuVert
            {
                UV = new UvHalf[obj.NumUVSets]
            };
            _gpuVertSerializer.DeserializeObject(gpuVert, objectStream1);
            return gpuVert;
        });
    }

    /// <inheritdoc />
    public override void SerializeObject(FSkeletalMeshVertexBuffer obj, IUnrealPackageStream objectStream)
    {
        objectStream.WriteInt32(obj.NumUVSets);
        objectStream.WriteInt32(obj.bUseFullPrecisionUVs);
        objectStream.WriteInt32(obj.bUsePackedPosition);
        _vectorSerializer.Serialize(objectStream.BaseStream, obj.MeshExtension);
        _vectorSerializer.Serialize(objectStream.BaseStream, obj.MeshOrigin);
        objectStream.BulkWriteTArray(obj.VertexBuffer, _gpuVertSerializer);
    }
}

public class DefaultGpuVertSerializer : BaseObjectSerializer<GpuVert>
{
    private readonly IStreamSerializer<FVector> _vectorSerializer;

    public DefaultGpuVertSerializer(IStreamSerializer<FVector> vectorSerializer)
    {
        _vectorSerializer = vectorSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(GpuVert obj, IUnrealPackageStream objectStream)
    {
        obj.N0 = objectStream.ReadUInt32();
        obj.N1 = objectStream.ReadUInt32();
        obj.BoneIndex = objectStream.ReadBytes(4);
        obj.BoneWeight = objectStream.ReadBytes(4);
        obj.Pos = _vectorSerializer.Deserialize(objectStream.BaseStream);
        for (var index = 0; index < obj.UV.Length; index++)
        {
            obj.UV[index] = new UvHalf
            {
                A = objectStream.ReadUInt16(),
                B = objectStream.ReadUInt16()
            };
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(GpuVert obj, IUnrealPackageStream objectStream)
    {
        objectStream.WriteUInt32(obj.N0);
        objectStream.WriteUInt32(obj.N1);
        objectStream.WriteBytes(obj.BoneIndex);
        objectStream.WriteBytes(obj.BoneWeight);
        _vectorSerializer.Serialize(objectStream.BaseStream, obj.Pos);
        foreach (var uvHalf in obj.UV)
        {
            objectStream.WriteUInt16(uvHalf.A);
            objectStream.WriteUInt16(uvHalf.B);
        }
    }
}