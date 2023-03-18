using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Divergency.Content.Dusts
{
    public class LivingShard : ModDust
    {
        public override Color? GetAlpha(Dust dust, Color lightColor) => new Color(255, 255, 255);
    }
}
