using FluentAssertions;

using RlUpk.Core.Classes.Core.Structs;
using RlUpk.MapBuilder.Cli;

namespace RlUpk.MapBuilder.Tests;

public class RotationCalculatorTest
{
    private static readonly int[] flipRot = [-1, 1, -1];

    /// <summary>
    /// Output is yzx (pitch, yaw, roll)
    /// </summary>
    /// <param name="inVar"></param>
    /// <returns></returns>
    public static List<int> GetUdkRot(IList<double> inVar)
    {
        var tempList = new List<int>();

        for (int i = 0; i < inVar.Count && i < 3; i++)
        {
            double degrees = inVar[i] * (180.0 / Math.PI);
            int multRot = (int)Math.Round(degrees * flipRot[i] * (65536.0 / 360.0));
            tempList.Add(multRot);
        }

        // Reorder: [Y, Z, X]
        List<int> outVar = [tempList[1], tempList[2], tempList[0]];
        return outVar;
    }

    public static List<double> GetBlenderRotations(IList<int> inVar)
    {
        // Reorder: [X, Y, Z] from [Y, Z, X]
        int[] reordered = [inVar[2] , inVar[0] , inVar[1]];
        var outVar = new List<double>();
        for (int i = 0; i < reordered.Length; i++)
        {
            double degrees = reordered[i] / (65536.0 / 360.0);
            double radians = (degrees / flipRot[i]) * (Math.PI / 180.0);
            outVar.Add(radians);
        }

        return outVar;
    }

    [Fact]
    public void KnownInputGivesKnownOutput()
    {
        // Output is yzx (pitch, yaw, roll)
        var res = GetUdkRot([0.7, 1.5, 0.5]);
        res.Should().BeEquivalentTo([15646, -5215, -7301]);
    }

    [Fact]
    public void CanReverseResultToGetInput()
    {
        var res = GetUdkRot([0.7, 1.5, 0.5]);
        var reversed = GetBlenderRotations(res);

        reversed[0].Should().BeApproximately(0.7, 0.01);
        reversed[1].Should().BeApproximately(1.5, 0.01);
        reversed[2].Should().BeApproximately(0.5, 0.01);
    }

    [Fact]
    public void EulerAnglesToFRotator_ReturnsExpectedValue()
    {
        EulerAngles eulerAngles = new(x: .7f, y: 1.5f, z: .5f);
        var rotator = eulerAngles.ToFRotator();

        Assert.Equal(15646, rotator.Pitch);
        Assert.Equal(-5215, rotator.Yaw);
        Assert.Equal(-7301, rotator.Roll);
    }

    [Fact]
    public void FRotatorToEulerAngles_ReturnsExpectedValue()
    {
        FRotator rotator = new() { Pitch = 15646, Yaw = -5215, Roll = -7301, };
        var eulerAngles = rotator.ToEulerAngles();

        eulerAngles.X.Should().BeApproximately(.7f, 0.01f);
        eulerAngles.Y.Should().BeApproximately(1.5f, 0.01f);
        eulerAngles.Z.Should().BeApproximately(.5f, 0.01f);
    }

    [Fact]
    public void FRotatorToUe3Quaternion_ReturnsExpectedValue()
    {
        var rotator = new FRotator() { Pitch = 15940, Yaw = -5420, Roll = -27472 };
        var quat = rotator.ToUe3QuaternionFromUe3RotationMatrix();
        
        quat.X.Should().BeApproximately(.631f, 0.01f);
        quat.Y.Should().BeApproximately(-.347f, 0.01f);
        quat.Z.Should().BeApproximately(.601f, 0.01f);
        quat.W.Should().BeApproximately(.347f, 0.01f);
    }
}