using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Divergency.Common.Systems3D
{
    internal interface I3D
    {
        public Vector3 StartRotation { get; }
        public Vector3 EndRotation { get; }
        public Func<int, int, float> SlashAnimation { get; }
        public int Duration { get; }
        public Vector2 Offset{ get; }
        public string SwordTexture { get; }
    }
}
