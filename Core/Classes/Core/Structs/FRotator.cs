using System.Diagnostics;

namespace Core.Classes.Core.Structs;

[DebuggerDisplay("(Pitch:{Pitch}, Yaw:{Yaw}, Roll:{Roll})")]
public class FRotator
{
    public int Pitch { get; set; }
    public int Yaw { get; set; }
    public int Roll { get; set; }
}