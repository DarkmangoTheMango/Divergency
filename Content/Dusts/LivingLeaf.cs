using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Divergency.Content.Dusts
{
    public class LivingLeaf : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, Main.rand.Next(2) * 13, 10, 14);
        }

        public override bool Update(Dust dust)
        {
            dust.rotation = dust.velocity.ToRotation() + MathHelper.PiOver2;

            return true;
        }
    }
}
