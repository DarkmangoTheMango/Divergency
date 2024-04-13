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
        public static ModKeybind Begin { get; private set; }
        public static ModKeybind End { get; private set; }

        public override void Load()
        {
            Reload = KeybindLoader.RegisterKeybind(Mod, "Reload the current weapon", "R");
            Begin = KeybindLoader.RegisterKeybind(Mod, "Begin infinite on last used pedestal", "B");
            End = KeybindLoader.RegisterKeybind(Mod, "end current encounter", "E");
        }

        public override void Unload()
        {
            Reload = null;
            Begin = null;
            End = null;
        }
    }
}
