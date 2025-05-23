using Core.Serialization.Abstraction;

namespace Core.Serialization;

/// <summary>
///     Basic implementation of a IObjectSerializerFactory
/// </summary>
public class ObjectSerializerFactory : IObjectSerializerFactory
{
    private readonly Dictionary<Type, IObjectSerializer> _serializers = new();

    /// <summary>
    ///     Constructs a factory from the serializers
    /// </summary>
    /// <param name="serializers"></param>
    public ObjectSerializerFactory(IEnumerable<IObjectSerializer> serializers)
    {
        foreach (var objectSerializer in serializers)
        {
            _serializers.Add(objectSerializer.GetSerializerFor(), objectSerializer);
        }
    }

    /// <inheritdoc />
    public IObjectSerializer? GetSerializer(Type type)
    {
        return _serializers.GetValueOrDefault(type);
    }
}