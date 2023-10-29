using Core.Classes.Engine;
using Core.Serialization.Abstraction;
using Core.Types;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultStaticParameterSetSerializer : BaseObjectSerializer<FStaticParameterSet>
{
    private readonly IStreamSerializer<FGuid> _guidSerializer;
    private readonly IStreamSerializer<FNormalParameter> _normalParameterSerializer;
    private readonly IStreamSerializer<FStaticComponentMaskParameter> _staticComponentMaskParameterSerializer;
    private readonly IStreamSerializer<FStaticSwitchParameter> _staticSwitchParameterSerializer;
    private readonly IStreamSerializer<FStaticTerrainLayerWeightParameter> _staticTerrainLayerWeightParameterSerializer;

    public DefaultStaticParameterSetSerializer(IStreamSerializer<FGuid> guidSerializer,
        IStreamSerializer<FStaticSwitchParameter> staticSwitchParameterSerializer,
        IStreamSerializer<FStaticComponentMaskParameter> staticComponentMaskParameterSerializer, IStreamSerializer<FNormalParameter> normalParameterSerializer,
        IStreamSerializer<FStaticTerrainLayerWeightParameter> staticTerrainLayerWeightParameterSerializer)
    {
        _guidSerializer = guidSerializer;
        _staticSwitchParameterSerializer = staticSwitchParameterSerializer;
        _staticComponentMaskParameterSerializer = staticComponentMaskParameterSerializer;
        _normalParameterSerializer = normalParameterSerializer;
        _staticTerrainLayerWeightParameterSerializer = staticTerrainLayerWeightParameterSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(FStaticParameterSet obj, IUnrealPackageStream objectStream)
    {
        obj.BaseMaterialID = _guidSerializer.Deserialize(objectStream.BaseStream);
        _staticSwitchParameterSerializer.ReadTArrayToList(objectStream.BaseStream, obj.StaticSwitchParameters);
        _staticComponentMaskParameterSerializer.ReadTArrayToList(objectStream.BaseStream, obj.StaticComponentMaskParameters);
        _normalParameterSerializer.ReadTArrayToList(objectStream.BaseStream, obj.NormalParameters);
        _staticTerrainLayerWeightParameterSerializer.ReadTArrayToList(objectStream.BaseStream, obj.TerrainLayerWeightParameters);
    }

    /// <inheritdoc />
    public override void SerializeObject(FStaticParameterSet obj, IUnrealPackageStream objectStream)
    {
        _guidSerializer.Serialize(objectStream.BaseStream, obj.BaseMaterialID);
        objectStream.WriteTArray(obj.StaticSwitchParameters, _staticSwitchParameterSerializer);
        objectStream.WriteTArray(obj.StaticComponentMaskParameters, _staticComponentMaskParameterSerializer);
        objectStream.WriteTArray(obj.NormalParameters, _normalParameterSerializer);
        objectStream.WriteTArray(obj.TerrainLayerWeightParameters, _staticTerrainLayerWeightParameterSerializer);
    }
}

public class DefaultNormalParameterSerializer : IStreamSerializer<FNormalParameter>
{
    private readonly IStreamSerializer<FGuid> _guidSerializer;
    private readonly IStreamSerializer<FName> _nameSerializer;

    public DefaultNormalParameterSerializer(IStreamSerializer<FGuid> guidSerializer, IStreamSerializer<FName> nameSerializer)
    {
        _guidSerializer = guidSerializer;
        _nameSerializer = nameSerializer;
    }

    /// <inheritdoc />
    public FNormalParameter Deserialize(Stream stream)
    {
        return new FNormalParameter
        {
            ParameterName = _nameSerializer.Deserialize(stream),
            CompressionSettings = (byte) stream.ReadByte(),
            bOverride = stream.ReadInt32() == 1,
            ExpressionGUID = _guidSerializer.Deserialize(stream)
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FNormalParameter value)
    {
        _nameSerializer.Serialize(stream, value.ParameterName);
        stream.WriteByte(value.CompressionSettings);
        stream.WriteInt32(value.bOverride ? 1 : 0);
        _guidSerializer.Serialize(stream, value.ExpressionGUID);
    }
}

public class DefaultStaticComponentMaskParameterSerializer : IStreamSerializer<FStaticComponentMaskParameter>
{
    private readonly IStreamSerializer<FGuid> _guidSerializer;
    private readonly IStreamSerializer<FName> _nameSerializer;

    public DefaultStaticComponentMaskParameterSerializer(IStreamSerializer<FName> nameSerializer, IStreamSerializer<FGuid> guidSerializer)
    {
        _nameSerializer = nameSerializer;
        _guidSerializer = guidSerializer;
    }

    /// <inheritdoc />
    public FStaticComponentMaskParameter Deserialize(Stream stream)
    {
        return new FStaticComponentMaskParameter
        {
            ParameterName = _nameSerializer.Deserialize(stream),
            R = stream.ReadInt32() == 1,
            G = stream.ReadInt32() == 1,
            B = stream.ReadInt32() == 1,
            A = stream.ReadInt32() == 1,
            bOverride = stream.ReadInt32() == 1,
            ExpressionGUID = _guidSerializer.Deserialize(stream)
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FStaticComponentMaskParameter value)
    {
        _nameSerializer.Serialize(stream, value.ParameterName);
        stream.WriteInt32(value.R ? 1 : 0);
        stream.WriteInt32(value.G ? 1 : 0);
        stream.WriteInt32(value.B ? 1 : 0);
        stream.WriteInt32(value.A ? 1 : 0);
        stream.WriteInt32(value.bOverride ? 1 : 0);
        _guidSerializer.Serialize(stream, value.ExpressionGUID);
    }
}

public class DefaultStaticSwitchParameterSerializer : IStreamSerializer<FStaticSwitchParameter>
{
    private readonly IStreamSerializer<FGuid> _guidSerializer;
    private readonly IStreamSerializer<FName> _nameSerializer;

    public DefaultStaticSwitchParameterSerializer(IStreamSerializer<FName> nameSerializer, IStreamSerializer<FGuid> guidSerializer)
    {
        _nameSerializer = nameSerializer;
        _guidSerializer = guidSerializer;
    }

    /// <inheritdoc />
    public FStaticSwitchParameter Deserialize(Stream stream)
    {
        return new FStaticSwitchParameter
        {
            ParameterName = _nameSerializer.Deserialize(stream),
            Value = stream.ReadInt32() == 1,
            bOverride = stream.ReadInt32() == 1,
            ExpressionGUID = _guidSerializer.Deserialize(stream)
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FStaticSwitchParameter value)
    {
        _nameSerializer.Serialize(stream, value.ParameterName);
        stream.WriteInt32(value.Value ? 1 : 0);
        stream.WriteInt32(value.bOverride ? 1 : 0);
        _guidSerializer.Serialize(stream, value.ExpressionGUID);
    }
}

public class DefaultStaticTerrainLayerWeightParameterSerializer : IStreamSerializer<FStaticTerrainLayerWeightParameter>
{
    private readonly IStreamSerializer<FGuid> _guidSerializer;
    private readonly IStreamSerializer<FName> _nameSerializer;

    /// <inheritdoc />
    public FStaticTerrainLayerWeightParameter Deserialize(Stream stream)
    {
        return new FStaticTerrainLayerWeightParameter
        {
            ParameterName = _nameSerializer.Deserialize(stream),
            WeightmapIndex = stream.ReadInt32(),
            bOverride = stream.ReadInt32() == 1,
            ExpressionGUID = _guidSerializer.Deserialize(stream)
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FStaticTerrainLayerWeightParameter value)
    {
        _nameSerializer.Serialize(stream, value.ParameterName);
        stream.WriteInt32(value.WeightmapIndex);
        stream.WriteInt32(value.bOverride ? 1 : 0);
        _guidSerializer.Serialize(stream, value.ExpressionGUID);
    }
}