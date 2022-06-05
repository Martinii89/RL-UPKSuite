using System.Diagnostics;
using Core.Classes.Core.Structs;
using Core.Classes.Engine.Structs;
using Core.Serialization.Abstraction;
using Core.Serialization.Extensions;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultStaticLodModelSerializer : IStreamSerializerFor<FStaticLodModel>
{
    private readonly IStreamSerializerFor<FByteBulkData> _BulkDataSerializer;
    private readonly IStreamSerializerFor<FSkeletalMeshVertexBuffer> _SkeletalMeshVertexBufferSerializer;
    private readonly IStreamSerializerFor<FSkelIndexBuffer> _SkelIndexBufferSerializer;
    private readonly IStreamSerializerFor<FSkelMeshChunk> _SkelMeshChunkSerializer;
    private readonly IStreamSerializerFor<FSkelMeshSection> _SkelMeshSectionSerializer;


    public DefaultStaticLodModelSerializer(IStreamSerializerFor<FSkelMeshSection> skelMeshSectionSerializer,
        IStreamSerializerFor<FSkelIndexBuffer> skelIndexBufferSerializer, IStreamSerializerFor<FSkelMeshChunk> skelMeshChunkSerializer,
        IStreamSerializerFor<FByteBulkData> bulkDataSerializer, IStreamSerializerFor<FSkeletalMeshVertexBuffer> skeletalMeshVertexBufferSerializer)
    {
        _SkelMeshSectionSerializer = skelMeshSectionSerializer;
        _SkelIndexBufferSerializer = skelIndexBufferSerializer;
        _SkelMeshChunkSerializer = skelMeshChunkSerializer;
        _BulkDataSerializer = bulkDataSerializer;
        _SkeletalMeshVertexBufferSerializer = skeletalMeshVertexBufferSerializer;
    }

    /// <inheritdoc />
    public FStaticLodModel Deserialize(Stream stream)
    {
        var lodModel = new FStaticLodModel();
        lodModel.Sections = _SkelMeshSectionSerializer.ReadTArrayToList(stream);
        lodModel.IndexBuffer = _SkelIndexBufferSerializer.Deserialize(stream);
        if (lodModel.IndexBuffer.Size != 4 || lodModel.IndexBuffer.Indices.ElementSize != 4)
        {
            Debugger.Break();
        }

        lodModel.UsedBones = stream.ReadTarray(stream1 => stream1.ReadInt16());
        lodModel.Chunks = _SkelMeshChunkSerializer.ReadTArrayToList(stream);
        lodModel.Size = stream.ReadInt32();
        lodModel.NumVerts = stream.ReadInt32();
        lodModel.RequiredBones = stream.ReadTarray(stream1 => (byte) stream1.ReadByte());
        lodModel.FBulkData = _BulkDataSerializer.Deserialize(stream);
        lodModel.NumUvSets = stream.ReadInt32();
        lodModel.GpuSkin = _SkeletalMeshVertexBufferSerializer.Deserialize(stream);
        return lodModel;
    }


    /// <inheritdoc />
    public void Serialize(Stream stream, FStaticLodModel value)
    {
        throw new NotImplementedException();
    }
}

public class DefaultSkelMeshSectionSerializer : IStreamSerializerFor<FSkelMeshSection>
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

public class DefaultSkelIndexBufferSerializer : IStreamSerializerFor<FSkelIndexBuffer>
{
    /// <inheritdoc />
    public FSkelIndexBuffer Deserialize(Stream stream)
    {
        return new FSkelIndexBuffer
        {
            Unk = stream.ReadInt32(),
            Size = (byte) stream.ReadByte(),
            Indices = stream.ReadTarrayWithElementSize(stream1 => stream1.ReadUInt32())
        };
    }

    public void Serialize(Stream stream, FSkelIndexBuffer value)
    {
        throw new NotImplementedException();
    }
}

public class DefaultSkelMeshChunkSerializer : IStreamSerializerFor<FSkelMeshChunk>
{
    private readonly IStreamSerializerFor<FRigidVertex> _rigidVertexSerializer;
    private readonly IStreamSerializerFor<FSoftVertex> _softVertexSerializer;

    public DefaultSkelMeshChunkSerializer(IStreamSerializerFor<FRigidVertex> rigidVertexSerializer, IStreamSerializerFor<FSoftVertex> softVertexSerializer)
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

public class DefaultRigidVertexSerializer : IStreamSerializerFor<FRigidVertex>
{
    private readonly IStreamSerializerFor<FColor> _colorSerializer;
    private readonly IStreamSerializerFor<FVector2D> _vector2DSerializer;

    private readonly IStreamSerializerFor<FVector> _vectorSerializer;

    public DefaultRigidVertexSerializer(IStreamSerializerFor<FVector> vectorSerializer, IStreamSerializerFor<FVector2D> vector2DSerializer,
        IStreamSerializerFor<FColor> colorSerializer)
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

public class DefaultSoftVertexSerializer : IStreamSerializerFor<FSoftVertex>
{
    private readonly IStreamSerializerFor<FColor> _colorSerializer;
    private readonly IStreamSerializerFor<FVector2D> _vector2DSerializer;
    private readonly IStreamSerializerFor<FVector> _vectorSerializer;

    public DefaultSoftVertexSerializer(IStreamSerializerFor<FVector> vectorSerializer, IStreamSerializerFor<FVector2D> vector2DSerializer,
        IStreamSerializerFor<FColor> colorSerializer)
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

public class DefaultSkeletalMeshVertexBufferSerializer : IStreamSerializerFor<FSkeletalMeshVertexBuffer>
{
    private readonly IObjectSerializer<GpuVert> _gpuVertSerializer;
    private readonly IStreamSerializerFor<FSkelIndexBuffer> _skelIndexBufferSerializer;

    private readonly IStreamSerializerFor<FVector> _vectorSerializer;

    public DefaultSkeletalMeshVertexBufferSerializer(IStreamSerializerFor<FVector> vectorSerializer, IObjectSerializer<GpuVert> gpuVertSerializer,
        IStreamSerializerFor<FSkelIndexBuffer> skelIndexBufferSerializer)
    {
        _vectorSerializer = vectorSerializer;
        _gpuVertSerializer = gpuVertSerializer;
        _skelIndexBufferSerializer = skelIndexBufferSerializer;
    }

    /// <inheritdoc />
    public FSkeletalMeshVertexBuffer Deserialize(Stream stream)
    {
        var r = new FSkeletalMeshVertexBuffer();
        r.NumUVSets = stream.ReadInt32();
        r.bUseFullPrecisionUVs = stream.ReadInt32();
        r.bUsePackedPosition = stream.ReadInt32();
        if (r.bUseFullPrecisionUVs != 0 && r.bUsePackedPosition != 1)
        {
            //implement all the vertex buffer types
            Debugger.Break();
        }

        r.MeshExtension = _vectorSerializer.Deserialize(stream);
        r.MeshOrigin = _vectorSerializer.Deserialize(stream);
        r.VertexBuffer = stream.ReadTarrayWithElementSize(stream1 =>
        {
            var gpuVert = new GpuVert
            {
                UV = new UvHalf[r.NumUVSets]
            };
            _gpuVertSerializer.DeserializeObject(gpuVert, stream1);
            return gpuVert;
        });
        //_gpuVertSerializer.ReadTArrayWithElementSize(stream);
        var ExtraVertexInfluencesCount = stream.ReadInt32();
        if (ExtraVertexInfluencesCount > 0)
        {
            Debugger.Break();
        }

        r.AdjacencyIndexBuffer = _skelIndexBufferSerializer.Deserialize(stream);

        return r;
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FSkeletalMeshVertexBuffer value)
    {
        throw new NotImplementedException();
    }
}

public class DefaultGpuVertSerializer : BaseObjectSerializer<GpuVert>
{
    private readonly IStreamSerializerFor<FVector> _vectorSerializer;

    public DefaultGpuVertSerializer(IStreamSerializerFor<FVector> vectorSerializer)
    {
        _vectorSerializer = vectorSerializer;
    }

    public override void DeserializeObject(GpuVert obj, Stream objectStream)
    {
        obj.N0 = objectStream.ReadUInt32();
        obj.N1 = objectStream.ReadUInt32();
        obj.BoneIndex = objectStream.ReadBytes(4);
        obj.BoneWeight = objectStream.ReadBytes(4);
        obj.Pos = _vectorSerializer.Deserialize(objectStream);
        for (var index = 0; index < obj.UV.Length; index++)
        {
            obj.UV[index] = new UvHalf
            {
                A = objectStream.ReadUInt16(),
                B = objectStream.ReadUInt16()
            };
        }
    }

    public override void SerializeObject(GpuVert obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}