﻿using Core.Classes.Core.Structs;

namespace Core.Classes.Engine.Structs;

public class FSoftVertex
{
    public byte[] BoneIndex { get; set; } = new byte[4];
    public byte[] BoneWeight { get; set; } = new byte[4];
    public FColor Color { get; set; } = new();
    public uint[] Normal { get; set; } = new uint[3];
    public FVector Pos { get; set; } = new();
    public FVector2D[] UV { get; set; } = new FVector2D[4];
}