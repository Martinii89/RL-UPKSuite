using Core.Classes.Core;
using Core.Classes.Engine;
using Core.Serialization.Abstraction;
using Core.Serialization.Extensions;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultRB_BodySetupSerializer : BaseObjectSerializer<URB_BodySetup>
{
    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultRB_BodySetupSerializer(IObjectSerializer<UObject> objectSerializer)
    {
        _objectSerializer = objectSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(URB_BodySetup obj, Stream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);

        //var preCachedPhysDataCount = objectStream.ReadInt32();

        obj.PreCachedPhysData = objectStream.ReadTarray(stream =>
        {
            var res = new FKCachedConvexData
            {
                CachedConvexElements = stream.ReadTarray(stream1 =>
                {
                    var data = new FKCachedConvexDataElement
                    {
                        ConvexElementData = stream1.ReadTarray(stream2 => (byte) stream2.ReadByte())
                    };
                    return data;
                })
            };
            return res;
        });

        //if (preCachedPhysDataCount != 0)
        //{
        //    Debugger.Break();
        //}
    }

    /// <inheritdoc />
    public override void SerializeObject(URB_BodySetup obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}