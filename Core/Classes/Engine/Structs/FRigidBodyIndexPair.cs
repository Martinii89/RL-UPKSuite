namespace RlUpk.Core.Classes.Engine.Structs;

public struct FRigidBodyIndexPair : IEquatable<FRigidBodyIndexPair>
{
    public int Index1 { get; set; }
    public int Index2 { get; set; }

    public bool Equals(FRigidBodyIndexPair other)
    {
        return Index1 == other.Index1 && Index2 == other.Index2;
    }

    public override bool Equals(object? obj)
    {
        return obj is FRigidBodyIndexPair other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Index1, Index2);
    }
}