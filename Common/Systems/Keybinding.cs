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
        public static ModKeybind BlockingBlocks { get; private set; }
        public static ModKeybind End { get; private set; }

        public override void Load()
        {
            Reload = KeybindLoader.RegisterKeybind(Mod, "Reload the current weapon", "R");
            BlockingBlocks = KeybindLoader.RegisterKeybind(Mod, "Edit blocking blocks of waves", "B");
            End = KeybindLoader.RegisterKeybind(Mod, "End current combat room", "E");
        }

        public override void Unload()
        {
            Reload = null;
            BlockingBlocks = null;
            End = null;
        }
    }
}
