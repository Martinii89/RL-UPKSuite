﻿using Core.Classes.Engine;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultLightMapTexture2DSerializer : BaseObjectSerializer<ULightMapTexture2D>
{
    private readonly IObjectSerializer<UTexture2D> _textureSerializer;


    public DefaultLightMapTexture2DSerializer(IObjectSerializer<UTexture2D> textureSerializer)
    {
        _textureSerializer = textureSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(ULightMapTexture2D obj, IUnrealPackageStream objectStream)
    {
        _textureSerializer.DeserializeObject(obj, objectStream);
        obj.LightMapFlags = (ULightMapTexture2D.ELightMapFlags) objectStream.ReadUInt32();
    }

    /// <inheritdoc />
    public override void SerializeObject(ULightMapTexture2D obj, IUnrealPackageStream objectStream)
    {
        _textureSerializer.SerializeObject(obj, objectStream);
        objectStream.WriteUInt32((uint) obj.LightMapFlags);
    }
}