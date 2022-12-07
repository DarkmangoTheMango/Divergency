using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Divergency.Common.Players
{
    public class ScreenShakePlayer : ModPlayer
    {
        public float ScreenShakeIntensity;

        public override void ModifyScreenPosition()
        {
            if (ScreenShakeIntensity > 0.1f)
            {
                Main.screenPosition += new Vector2(Main.rand.NextFloat(ScreenShakeIntensity), Main.rand.NextFloat(ScreenShakeIntensity));
                ScreenShakeIntensity *= 0.9f;
            }
        }
    }
}
