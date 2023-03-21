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

                for (int i = 0; i < (SA.Duration + SA.Delay); i++)
                {
                    lookupArray.Add(SAIDX);
                    curIDX++;
                }

                SA.ThisKeyframe = curIDX-1; // MIGHT BE WRONG
            }
        }
    }

    public class SwordAnimation
    {

        public Vector3 TargetRotation;
        public InterpolationAnimation3D Interpolation3D;
        public InterpolationAnimation2D Interpolation2D;
        public int Duration;
        
        public int LastKeyframe;
        public int ThisKeyframe;
        
        public Vector2 Offset;
        public int Delay;

        public SwordAnimation(Vector3 TargetRotation, int Duration, InterpolationAnimation3D SlashAnimation3D = null, InterpolationAnimation2D SlashAnimation2D = null, Vector2 Offset = default, int Delay = 0)
        {
            this.TargetRotation = TargetRotation;
            this.Interpolation3D = ((SlashAnimation3D == null) ? new InterpolationAnimation3D() : SlashAnimation3D);
            this.Interpolation2D = ((SlashAnimation2D == null) ? new InterpolationAnimation2D() : SlashAnimation2D);
            this.Duration = Duration;
            this.Offset = Offset;
            this.Delay = Delay;
        }
    }

    public interface I3D
    {
        public Vector3 StartRotation { get; }
        public Vector2 StartOffset { get; }
        public AnimKeyframes SwordFrames { get; }
        public Vector2 ConstOffset { get; }
        public string SwordTexture { get; }
    }
}
