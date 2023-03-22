using Humanizer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Divergency.Common.Systems3D
{
    public class InterpolationAnimation3D
    {
        public static float linear(int time, int maxTime)
        {
            return (float)time / (float)maxTime;
        }

        Func<int, int, float> X;
        Func<int, int, float> Y;
        Func<int, int, float> Z;

        public InterpolationAnimation3D()
        {
            X = Y = Z = linear;
        }
        public InterpolationAnimation3D(Func<int, int, float> func)
        {
            X = Y = Z = func;
        }

        public InterpolationAnimation3D(Func<int, int, float> funcX, Func<int, int, float> funcY, Func<int, int, float> funcZ)
        {
            X = funcX;
            Y = funcY;
            Z = funcZ;
        }

        float lerp(float a, float b, float f)
        {
            return a * (1.0f - f) + (b * f);
        }

        public Vector3 Lerp(Vector3 v1, Vector3 v2, int time, int maxTime)
        {
            if (time > maxTime)
                time = maxTime;

            return new Vector3(
                lerp(v1.X, v2.X, X(time, maxTime)),
                lerp(v1.Y, v2.Y, Y(time, maxTime)),
                lerp(v1.Z, v2.Z, Z(time, maxTime))
            );
        }
    }
    public class InterpolationAnimation2D
    {
        public static float linear(int time, int maxTime)
        {
            return (float)time / (float)maxTime;
        }

        Func<int, int, float> X;
        Func<int, int, float> Y;
        
        public InterpolationAnimation2D()
        {
            X = Y = linear;
        }
        public InterpolationAnimation2D(Func<int, int, float> func)
        {
            X = Y = func;
        }

        public InterpolationAnimation2D(Func<int, int, float> funcX, Func<int, int, float> funcY, Func<int, int, float> funcZ)
        {
            X = funcX;
            Y = funcY;
        }

        float lerp(float a, float b, float f)
        {
            return a * (1.0f - f) + (b * f);
        }

        public Vector2 Lerp(Vector2 v1, Vector2 v2, int time, int maxTime)
        {
            return new Vector2(
                lerp(v1.X, v2.X, X(time, maxTime)),
                lerp(v1.Y, v2.Y, Y(time, maxTime))
            );
        }
    }

    public class AnimKeyframes
    {
        public List<int> lookupArray = new List<int>();
        public SwordAnimation[] realArray;

        public AnimKeyframes(SwordAnimation[] incommingArray)
        {
            realArray = incommingArray;

            int curIDX = 0;

            for (int SAIDX = 0; SAIDX < realArray.Length; SAIDX++)
            {
                SwordAnimation SA = realArray[SAIDX];
                SA.LastKeyframe = curIDX;

                for (int i = 0; i < (SA.Duration); i++)
                {
                    lookupArray.Add(SAIDX);
                    curIDX++;
                }

                SA.ThisKeyframe = curIDX - 1; // MIGHT BE WRONG

                for (int i = 0; i < (SA.Delay); i++)
                {
                    lookupArray.Add(SAIDX);
                    curIDX++;
                }
            }
        }
    }

    public class SwordAnimation
    {
        public Vector3 TargetRotation;
        public InterpolationAnimation3D TargetRotationAnim;

        public Vector2 Offset;
        public InterpolationAnimation2D OffsetAnim;

        public Vector2 Scale;
        public InterpolationAnimation2D ScaleAnim;

        public Vector2 LocalOffset;
        public InterpolationAnimation2D LocalOffsetAnim;

        public int Duration;
        public int Delay;

        public int LastKeyframe;
        public int ThisKeyframe;

        public SwordAnimation(
            Vector3 TargetRotation, int Duration,
            int Delay = 0, InterpolationAnimation3D TargetRotationAnim = null,
            Vector2 Offset = default, InterpolationAnimation2D OffsetAnim = null,
            Vector2 Scale = default,InterpolationAnimation2D ScaleAnim = null,
            Vector2 LocalOffset = default, InterpolationAnimation2D LocalOffsetAnim = null)
        {
            this.TargetRotation = TargetRotation;
            this.TargetRotationAnim = ((TargetRotationAnim == null) ? new InterpolationAnimation3D() : TargetRotationAnim);

            this.Offset = Offset;
            this.OffsetAnim = ((OffsetAnim == null) ? new InterpolationAnimation2D() : OffsetAnim);

            this.Scale = Scale == default ? new Vector2(1, 1) : Scale;
            this.ScaleAnim = ((ScaleAnim == null) ? new InterpolationAnimation2D() : ScaleAnim);

            this.LocalOffset = LocalOffset;
            this.LocalOffsetAnim = ((LocalOffsetAnim == null) ? new InterpolationAnimation2D() : LocalOffsetAnim);

            this.Duration = Duration;
            this.Delay = Delay;
        }
    }

    public interface I3D
    {
        public Vector3 StartRotation { get; }
        public Vector2 StartOffset { get; }
        public Vector2 StartLocalOffset { get; }
        public Vector2 StartScale { get; }
        public AnimKeyframes SwordFrames { get; }
        public Vector2 ConstOffset { get; }
        public string SwordTexture { get; }
    }
}
