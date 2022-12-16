using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Divergency.Content.Dusts
{
    public class Smoke : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.scale *= Main.rand.NextFloat(0.8f, 2f);
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 19, 19, 19);
            dust.rotation = Main.rand.NextFloat(6.28f);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color gray = new(25, 25, 25);
            Color black = Color.Black;
            Color color;

            if (dust.alpha < 120) { color = Color.Lerp(dust.color, gray, dust.alpha / 120f); }
            else if (dust.alpha < 180) { color = Color.Lerp(gray, black, (dust.alpha - 120) / 60f); }
            else { color = black; }

            return color * ((255 - dust.alpha) / 255f);
        }

        public override bool Update(Dust dust)
        {
            dust.velocity *= 0.98f;
            dust.velocity.X *= 0.95f;
            dust.color *= 0.98f;

            if (dust.alpha > 100)
            {
                dust.scale *= 0.975f;
                dust.alpha += 2;
            }
            else
            {
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.1f);
                dust.scale *= 0.985f;
                dust.alpha += 4;
            }

            dust.position += dust.velocity;
            dust.rotation += 0.01f;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }

    public class SmokeIncendiary : ModDust
    {
        public override string Texture => "Divergency/Content/Dusts/Smoke";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.scale *= Main.rand.NextFloat(0.8f, 2f);
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 19, 19, 19);
            dust.rotation = Main.rand.NextFloat(6.28f);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color gray = new(97, 6, 0);
            Color black = Color.Black;
            Color color;

            if (dust.alpha < 120) { color = Color.Lerp(new Color(255, 187, 0), gray, dust.alpha / 120f); }
            else if (dust.alpha < 180) { color = Color.Lerp(gray, black, (dust.alpha - 120) / 60f); }
            else { color = black; }

            return color * ((255 - dust.alpha) / 255f);
        }

        public override bool Update(Dust dust)
        {
            dust.velocity *= 0.98f;
            dust.velocity.X *= 0.95f;
            dust.color *= 0.98f;
            dust.scale += 0.05f;

            if (dust.alpha > 100)
            {
                dust.alpha += 2;
            }
            else
            {
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.1f);
                dust.alpha += 4;
            }

            dust.position += dust.velocity;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }
}
