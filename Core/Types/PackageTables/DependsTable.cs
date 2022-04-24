using Core.Serialization;

namespace Core.Types.PackageTables;

public class DependsTable: IBinaryDeserializableClass
{
    public TArray<TArray<int>> Depends { get; set; } = new();

    public void InitializeSize(int exportCount)
    {
        for (int i = 0; i < exportCount; i++)
        {
            Depends.Add(new TArray<int>());
        }
    }

    public void Deserialize(BinaryReader reader)
    {
        foreach (var depend in Depends)
        {
            depend.Deserialize(reader);
        }
    }
}