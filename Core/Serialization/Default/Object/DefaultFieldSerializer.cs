﻿using RlUpk.Core.Classes.Core;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object;

/// <summary>
///     Default implementation of UField serializer
/// </summary>
public class DefaultFieldSerializer : BaseObjectSerializer<UField>
{
    private readonly IObjectSerializer<UObject> _objectSerializer;

    /// <summary>
    ///     DefaultFieldSerializer constructor taking in the required field serializers
    /// </summary>
    /// <param name="objectSerializer"></param>
    public DefaultFieldSerializer(IObjectSerializer<UObject> objectSerializer)
    {
        _objectSerializer = objectSerializer;
    }


    /// <inheritdoc />
    public override void DeserializeObject(UField field, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(field, objectStream);

        field.Next = objectStream.ReadObject();
    }

    /// <inheritdoc />
    public override void SerializeObject(UField obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}