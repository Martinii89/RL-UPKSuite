using System.Diagnostics;
using Core.Classes.Engine.Structs;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultkDOPNode3NewSerializer : IStreamSerializer<FkDOPNode>
{
    /// <inheritdoc />
    public FkDOPNode Deserialize(Stream stream)
    {
        var res = new FkDOPNode();
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

    /// <inheritdoc />
    public void Serialize(Stream stream, FkDOPNode value)
    {
        stream.Write(value.Mins);
        stream.Write(value.Maxs);
    }
}