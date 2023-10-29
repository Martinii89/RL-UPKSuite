using System.Diagnostics;
using Core.Classes.Core;

namespace Core.Serialization.Abstraction;

/// <summary>
///     A IObjectSerializer reads and writes the serial data of an object
/// </summary>
public interface IObjectSerializer
{
    /// <summary>
    ///     Returns the type this serializer is compatible with
    /// </summary>
    /// <returns></returns>
    Type GetSerializerFor();

    /// <summary>
    ///     Read the object properties from the stream
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="objectStream"></param>
    void DeserializeObject(object obj, IUnrealPackageStream objectStream);

    /// <summary>
    ///     Write the object properties to the stream
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="objectStream"></param>
    void SerializeObject(object obj, IUnrealPackageStream objectStream);
}

/// <summary>
///     Generic version of a IObjectSerializer.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IObjectSerializer<in T> : IObjectSerializer
{
    /// <summary>
    ///     Read the object properties from the stream
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="objectStream"></param>
    void DeserializeObject(T obj, IUnrealPackageStream objectStream);

    /// <summary>
    ///     Write the object properties to the stream
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="objectStream"></param>
    void SerializeObject(T obj, IUnrealPackageStream objectStream);
}

/// <summary>
///     Base object serializer that forwards the typeless methods to the generic methods.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseObjectSerializer<T> : IObjectSerializer<T>
{
    /// <inheritdoc />
    public Type GetSerializerFor()
    {
        return typeof(T);
    }

    /// <inheritdoc />
    public void DeserializeObject(object obj, IUnrealPackageStream objectStream)
    {
        if (obj is T tObj)
        {
            DeserializeObject(tObj, objectStream);
        }
    }

    /// <inheritdoc />
    public void SerializeObject(object obj, IUnrealPackageStream objectStream)
    {
        if (obj is T tObj)
        {
            SerializeObject(tObj, objectStream);
        }
    }

    /// <inheritdoc />
    public abstract void DeserializeObject(T obj, IUnrealPackageStream objectStream);

    /// <inheritdoc />
    public abstract void SerializeObject(T obj, IUnrealPackageStream objectStream);

    protected void DropRamainingNativeData(UObject obj, Stream objectStream)
    {
        var remaining = obj.ExportTableItem.SerialOffset + obj.ExportTableItem.SerialSize - objectStream.Position;
        if (remaining > 0)
        {
            Debugger.Break();
        }

        objectStream.Move(remaining);
    }
}