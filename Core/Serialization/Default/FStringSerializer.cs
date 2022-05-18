using Core.Serialization.Extensions;
using Core.Types;

namespace Core.Serialization.Default;

/// <inheritdoc />
public class FStringSerializer : IStreamSerializerFor<FString>
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
        throw new NotImplementedException();
    }
}