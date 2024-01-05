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
                Main.screenPosition += Main.rand.NextVector2Circular(ScreenShakeIntensity, ScreenShakeIntensity);
                ScreenShakeIntensity *= 0.9f;
            }
        }
    }
}
