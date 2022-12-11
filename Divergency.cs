using Divergency.Common.Helpers;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace Divergency
{
    public partial class Divergency : Mod
    {
        public class TemporaryFix : PreJITFilter
        {
            public override bool ShouldJIT(MemberInfo member) => false;
        }

        public static Divergency Instance { get; set; }

        public Divergency()
        {
            Instance = this;
            PreJITFilter = new TemporaryFix();
        }

        public override void Unload()
        {
            if (!Main.dedServ)
            {
                Instance = null;
            }
        }
    }
}