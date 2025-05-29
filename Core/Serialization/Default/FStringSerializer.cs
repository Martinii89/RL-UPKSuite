using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Serialization.Extensions;
using RlUpk.Core.Types;

namespace RlUpk.Core.Serialization.Default;

/// <inheritdoc />
public class FStringSerializer : IStreamSerializer<FString>
{
    /// <inheritdoc />
    public FString Deserialize(Stream stream)
    {
        return new FString
        {
            InnerString = stream.ReadFString()
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FString value)
    {
        stream.WriteFString(value.InnerString);
    }
}