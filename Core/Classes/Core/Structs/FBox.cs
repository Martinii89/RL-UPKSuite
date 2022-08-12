namespace Core.Classes.Core.Structs;

public class FBox
{
    public FVector Min { get; set; } = new();
    public FVector Max { get; set; }
    public byte IsValid { get; set; }
}