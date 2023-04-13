
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Divergency.Content.Dusts
{
    public class NoteDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 15, 15, 15);
            // If our texture had 3 different dust on top of each other (a 30x90 pixel image), we might do this:
            // dust.frame = new Rectangle(0, Main.rand.Next(3) * 30, 30, 30);
        }

     
    }



 }