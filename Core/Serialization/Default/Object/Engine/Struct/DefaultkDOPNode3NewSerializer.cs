using System.Diagnostics;
using Core.Classes.Engine.Structs;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultkDOPNode3NewSerializer : IStreamSerializer<FkDOPNode3New>
{
    /// <inheritdoc />
    public FkDOPNode3New Deserialize(Stream stream)
    {
        var res = new FkDOPNode3New();
        var read = stream.Read(res.Mins, 0, 3);
        if (read != 3)
        {
            Debugger.Break();
        }

        read = stream.Read(res.Maxs, 0, 3);
        if (read != 3)
        {
            Debugger.Break();
        }

        return res;
    }

    public void Serialize(Stream stream, FkDOPNode3New value)
    {
        throw new NotImplementedException();
    }
}