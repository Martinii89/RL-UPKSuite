using System.Numerics;

using RlUpk.Core.Classes.Core.Structs;

namespace RlUpk.MapBuilder.Cli;

public static class RotationExtensions
{
    private const float UnrealToDegrees = 360.0f / 65536.0f;
    private const float UnrealToRads = UnrealToDegrees * MathF.PI / 180.0f;

    public static FRotator ToFRotator(this EulerAngles rotation)
    {
        return new FRotator
        {
            Pitch = (int)Math.Round((180.0 / Math.PI) * (65536.0 / 360.0) * rotation.Y),
            Yaw = (int)Math.Round((180.0 / Math.PI) * (65536.0 / 360.0) * -rotation.Z),
            Roll = (int)Math.Round((180.0 / Math.PI) * (65536.0 / 360.0) * -rotation.X),
        };
    }

    public static EulerAngles ToEulerAngles(this FRotator rotation)
    {
        // Inverse of GetUdkRot: convert FRotator to EulerAngles (in radians)
        float y = (float)(rotation.Pitch * (360.0f / 65536.0f) * (Math.PI / 180.0f));
        float z = (float)(-rotation.Yaw * (360.0f / 65536.0f) * (Math.PI / 180.0f));
        float x = (float)(-rotation.Roll * (360.0f / 65536.0f) * (Math.PI / 180.0f));
        return new EulerAngles(x, y, z);
    }


    public static Quaternion ConvertToRightHandedQuaternion(this FRotator rotator)
    {
        var q = rotator.ToUe3QuaternionFromUe3RotationMatrix();
        // flip z and y. negate w
        return new Quaternion(q.X, q.Z, q.Y, -q.W);
    }

    // Borrowed math from ue3  UnMath.h: FRotationMatrix(const FRotator& Rot)
    private static Matrix4x4 ToUe3RotationMatrix(this FRotator rotation)
    {
        float pitch = rotation.Pitch * UnrealToRads;
        float yaw = rotation.Yaw * UnrealToRads;
        float roll = rotation.Roll * UnrealToRads;

        float SR = MathF.Sin(roll);
        float SP = MathF.Sin(pitch);
        float SY = MathF.Sin(yaw);
        float CR = MathF.Cos(roll);
        float CP = MathF.Cos(pitch);
        float CY = MathF.Cos(yaw);

        var m = new Matrix4x4
        {
            [0, 0] = CP * CY,
            [0, 1] = CP * SY,
            [0, 2] = SP,
            [0, 3] = 0f,
            [1, 0] = SR * SP * CY - CR * SY,
            [1, 1] = SR * SP * SY + CR * CY,
            [1, 2] = -SR * CP,
            [1, 3] = 0f,
            [2, 0] = -(CR * SP * CY + SR * SY),
            [2, 1] = CY * SR - CR * SP * SY,
            [2, 2] = CR * CP,
            [2, 3] = 0f,
            [3, 0] = 0,
            [3, 1] = 0,
            [3, 2] = 0,
            [3, 3] = 1f
        };

        return m;
    }

    private static readonly int[] nxt = [1, 2, 0];

    // Borrowed math from ue3 UnMath.h : FQuat::FQuat( const FMatrix& M )
    public static Quaternion ToUe3QuaternionFromUe3RotationMatrix(this FRotator rotator)
    {
        var m = rotator.ToUe3RotationMatrix();
        /* TODO: Implement this
         * 	// If Matrix is NULL, return Identity quaternion.
            if( M.GetAxis(0).IsNearlyZero() && M.GetAxis(1).IsNearlyZero() && M.GetAxis(2).IsNearlyZero() )
            {
                *this = FQuat::Identity;
                return;
            }
         */
        var q = new Quaternion();
        float tr = m[0, 0] + m[1, 1] + m[2, 2];
        if (tr > 0)
        {
            var invS = 1 / Math.Sqrt(tr + 1);
            q.W = (float)(0.5 * (1.0 / invS));
            var s = 0.5 * invS;
            q.X = (float)((m[1, 2] - m[2, 1]) * s);
            q.Y = (float)((m[2, 0] - m[0, 2]) * s);
            q.Z = (float)((m[0, 1] - m[1, 0]) * s);
        }
        else
        {
            int i = 0;
            float s = 0;

            if (m[1, 1] > m[0, 0])
                i = 1;

            if (m[2, 2] > m[i, i])
                i = 2;


            int j = nxt[i];
            int k = nxt[j];

            s = m[i, i] - m[j, j] - m[k, k] + 1.0f;

            float invS = 1.0f / MathF.Sqrt(s);

            Span<float> qt = stackalloc float[4];
            qt[i] = 0.5f * (1f / invS);

            s = 0.5f * invS;

            qt[3] = (m[j, k] - m[k, j]) * s;
            qt[j] = (m[i, j] + m[j, i]) * s;
            qt[k] = (m[i, k] + m[k, i]) * s;

            q.X = qt[0];
            q.Y = qt[1];
            q.Z = qt[2];
            q.W = qt[3];
        }

        return q;
    }
}