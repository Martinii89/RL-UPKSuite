using System.Reflection;

namespace Core.Serialization;

// https://stackoverflow.com/a/29379834/3079571
internal static class TypeLoaderExtensions
{
    internal static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
    {
        if (assembly == null) throw new ArgumentNullException(nameof(assembly));
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            return e.Types.Where(t => t != null)!;
        }
    }
}

/// <summary>
///     Collection of Serializers Implementing <see cref="IStreamSerializerFor{T}" />.
/// </summary>
public class SerializerCollection : ISerializerCollection
{
    private static readonly Type SerializerInterfaceType = typeof(IStreamSerializerFor<>);

    private readonly Dictionary<Type, object> _serializers = new();

    /// <summary>
    ///     A ready only copy of the serializer dictionary. Main use case is for unit test asserts
    /// </summary>
    public IReadOnlyDictionary<Type, object> Serializers => _serializers;

    /// <summary>
    ///     Retrieve a <see cref="IStreamSerializerFor{T}" /> if one is registered for the given type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>Returns a serializer. May return null if no serializer if found in the collection</returns>
    public IStreamSerializerFor<T>? GetSerializerFor<T>()
    {
        _serializers.TryGetValue(typeof(T), out var obj);
        return obj as IStreamSerializerFor<T>;
    }

    /// <summary>
    ///     Register a seralizer for a type
    /// </summary>
    /// <typeparam name="T">The serializable type</typeparam>
    /// <param name="serializer">The serializer</param>
    public void AddSerializer<T>(IStreamSerializerFor<T> serializer)
    {
        if (_serializers.ContainsKey(typeof(T)))
            throw new InvalidOperationException($"Serializer for {typeof(T).Name} already registered");

        _serializers.Add(typeof(T), serializer);
    }

    private static List<Type> GetImplementedSerializersOfType(Type type)
    {
        return type.GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == SerializerInterfaceType).ToList();
    }

    private void AddSerializer(Type type, DuplicateImplementationResolution duplicateImplementationResolution)
    {
        var possibleInterfaces = GetImplementedSerializersOfType(type);
        if (type.BaseType != null)
        {
            var interfacesInBase = GetImplementedSerializersOfType(type.BaseType);
            interfacesInBase.ForEach(i => possibleInterfaces.Remove(i));
        }

        foreach (var implementedInterface in possibleInterfaces)
        {
            if (!implementedInterface.IsGenericType ||
                implementedInterface.GetGenericTypeDefinition() != SerializerInterfaceType)
                continue;

            var serializableType = implementedInterface.GetGenericArguments()[0];
            var serializerInstance = Activator.CreateInstance(type);
            if (serializerInstance == null) throw new NullReferenceException(nameof(serializerInstance));

            if (_serializers.ContainsKey(serializableType))
            {
                switch (duplicateImplementationResolution)
                {
                    case DuplicateImplementationResolution.Replace:
                        _serializers[serializableType] = serializerInstance;
                        break;
                    case DuplicateImplementationResolution.Throw:
                        throw new InvalidOperationException($"Serializer for {serializableType} already registered");
                    case DuplicateImplementationResolution.Skip:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(duplicateImplementationResolution),
                            duplicateImplementationResolution, null);
                }

                return;
            }

            _serializers.Add(serializableType, serializerInstance);
        }
    }

    /// <summary>
    ///     Scans the assembly for <see cref="IStreamSerializerFor{T}" /> implementations with no
    ///     <see cref="FileVersionAttribute" /> and registers them to the collections.
    ///     Types already registered will silently be skipped. Useful for complementing a set of tagged serializers with
    ///     compatible default serializers
    /// </summary>
    /// <param name="assembly"></param>
    public void AddMissingDefaultSerializersFromAssembly(Assembly assembly)
    {
        foreach (var type in assembly.GetLoadableTypes())
        {
            if (type.IsAbstract || type.IsInterface) continue;

            var versionTag = type.GetCustomAttribute<FileVersionAttribute>();
            if (versionTag != null) continue;

            AddSerializer(type, DuplicateImplementationResolution.Skip);
        }
    }

    /// <summary>
    ///     Scans the assembly for <see cref="IStreamSerializerFor{T}" /> implementations with no
    ///     <see cref="FileVersionAttribute" /> and registers them to the collections.
    ///     If a found type is already registered, it will throw
    /// </summary>
    /// <param name="assembly"></param>
    /// <exception cref="InvalidOperationException">Thrown for duplicate serializer implementations</exception>
    public void AddDefaultSerializersFromAssembly(Assembly assembly)
    {
        foreach (var type in assembly.GetLoadableTypes())
        {
            if (type.IsAbstract || type.IsInterface) continue;

            var versionTag = type.GetCustomAttribute<FileVersionAttribute>();
            if (versionTag != null) continue;

            AddSerializer(type, DuplicateImplementationResolution.Throw);
        }
    }

    /// <summary>
    ///     Scans the assembly for <see cref="IStreamSerializerFor{T}" /> implementations that has a
    ///     <see cref="FileVersionAttribute" /> and registers them to the collections.
    ///     If a found type is already registered, it will throw
    /// </summary>
    /// <param name="assembly">Assembly to scan</param>
    /// <param name="version"><see cref="FileVersionAttribute.FileVersion" /> tag to filter on</param>
    /// <exception cref="InvalidOperationException">Thrown for duplicate serializer implementations</exception>
    public void AddSerializersFromAssembly(Assembly assembly, string version)
    {
        if (string.IsNullOrEmpty(version))
        {
            AddDefaultSerializersFromAssembly(assembly);
            return;
        }

        foreach (var type in assembly.GetLoadableTypes())
        {
            if (type.IsAbstract || type.IsInterface) continue;

            var tagVersion = type.GetCustomAttribute<FileVersionAttribute>()?.FileVersion;
            if (tagVersion != version) continue;

            AddSerializer(type, DuplicateImplementationResolution.Throw);
        }
    }

    /// <summary>
    ///     A enum dictating how the collection will handle duplicate serializers in
    ///     <see cref="SerializerCollection.AddSerializer" />
    /// </summary>
    private enum DuplicateImplementationResolution
    {
        Replace,
        Throw,
        Skip
    }
}