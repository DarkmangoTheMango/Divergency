using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Divergency
{
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind Reload { get; private set; }

        public override void Load()
        {
            Reload = KeybindLoader.RegisterKeybind(Mod, "Reload the current weapon", "R");
        }

        public override void Unload()
        {
            Reload = null;
        }
    }
}
