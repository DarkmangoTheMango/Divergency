using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Divergency.Common.Systems3D
{

    public class SwordAnimation
    {
        public static float linear(int time, int maxTime)
        {
            return (float)time / (float)maxTime;
        }

        Vector3 TargetRotation;
        Func<int, int, float> SlashAnimation;
        int Duration;
        Vector2 Offset;
        int Delay;

        public SwordAnimation(Vector3 TargetRotation, int Duration, Func<int, int, float> SlashAnimation = null, Vector2 Offset = default, int Delay = 0)
        {
            this.TargetRotation = TargetRotation;
            this.SlashAnimation = ((SlashAnimation == null) ? linear : SlashAnimation);
            this.Duration = Duration;
            this.Offset = Offset;
            this.Delay = Delay;
        }
    }

    public interface I3D
    {
        public Vector3 StartRotation { get; }
        public SwordAnimation[] SwordAnimations { get; }
        public Vector2 ConstOffset { get; }
        public string SwordTexture { get; }
    }
}
