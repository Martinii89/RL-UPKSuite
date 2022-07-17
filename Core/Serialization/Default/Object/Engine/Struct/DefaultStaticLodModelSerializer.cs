using System.Diagnostics;
using Core.Classes.Core.Structs;
using Core.Classes.Engine.Structs;
using Core.Serialization.Abstraction;
using Core.Serialization.Extensions;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultStaticLodModelSerializer : BaseObjectSerializer<FStaticLodModel>
{
    private readonly IStreamSerializer<FByteBulkData> _BulkDataSerializer;
    private readonly IObjectSerializer<FSkeletalMeshVertexBuffer> _SkeletalMeshVertexBufferSerializer;
    private readonly IStreamSerializer<FSkelIndexBuffer> _SkelIndexBufferSerializer;
    private readonly IStreamSerializer<FSkelMeshChunk> _SkelMeshChunkSerializer;
    private readonly IStreamSerializer<FSkelMeshSection> _SkelMeshSectionSerializer;


    public DefaultStaticLodModelSerializer(IStreamSerializer<FSkelMeshSection> skelMeshSectionSerializer,
        IStreamSerializer<FSkelIndexBuffer> skelIndexBufferSerializer, IStreamSerializer<FSkelMeshChunk> skelMeshChunkSerializer,
        IStreamSerializer<FByteBulkData> bulkDataSerializer, IObjectSerializer<FSkeletalMeshVertexBuffer> skeletalMeshVertexBufferSerializer)
    {
        _SkelMeshSectionSerializer = skelMeshSectionSerializer;
        _SkelIndexBufferSerializer = skelIndexBufferSerializer;
        _SkelMeshChunkSerializer = skelMeshChunkSerializer;
        _BulkDataSerializer = bulkDataSerializer;
        _SkeletalMeshVertexBufferSerializer = skeletalMeshVertexBufferSerializer;
    }


    /// <inheritdoc />
    public override void DeserializeObject(FStaticLodModel obj, IUnrealPackageStream objectStream)
    {
        obj.Sections = _SkelMeshSectionSerializer.ReadTArrayToList(objectStream.BaseStream);
        obj.IndexBuffer = _SkelIndexBufferSerializer.Deserialize(objectStream.BaseStream);
        if (obj.IndexBuffer.Size != 4 || obj.IndexBuffer.Indices.ElementSize != 4)
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
        // I don't think these properties are actually part of the FSkeletalMeshVertexBuffer. but separate objects in FStaticLodModel

        var extraVertexInfluencesCount = objectStream.ReadInt32();
        if (extraVertexInfluencesCount > 0)
        {
            Debugger.Break();
        }

        obj.AdjacencyIndexBuffer = _SkelIndexBufferSerializer.Deserialize(objectStream.BaseStream);
    }

    /// <inheritdoc />
    public override void SerializeObject(FStaticLodModel obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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
        switch (buffer.Size)
        {
            case 4:
                buffer.Indices = stream.ReadTarrayWithElementSize(stream1 => stream1.ReadUInt32());
                break;
            case 2:
                buffer.Indices = stream.ReadTarrayWithElementSize(stream1 => (uint) stream1.ReadUInt16());
                break;
            default:
                throw new InvalidDataException();
        }

        return buffer;
    }

    public void Serialize(Stream stream, FSkelIndexBuffer value)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }
}

public class DefaultGpuVertSerializer : BaseObjectSerializer<GpuVert>
{
    private readonly IStreamSerializer<FVector> _vectorSerializer;

    public DefaultGpuVertSerializer(IStreamSerializer<FVector> vectorSerializer)
    {
        _vectorSerializer = vectorSerializer;
    }

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

    public override void SerializeObject(GpuVert obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}