using Divergency.Common.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Particles
{
    public class MysteryShineParticle : Particle
    {
        public override string Texture => "Divergency/Assets/Textures/ParticleTextures/MysteryShine";

        public override void SetDefaults()
        {
            width = 1;
            height = 1;
            opacity = 125;
            layer = Layer.BeforeProjectiles;
        }

        public override void AI()
        {
            rotation = velocity.ToRotation();
            Projectile proj = Main.projectile[(int)ai[0]];
            // if (proj.type == ModContent.ProjType<cocknballprojectile>() && proj.active)
            if (proj.active)
            {
                position = proj.Center;
                timeLeft = 10;

            }
            else
            {
                active = false;
            }

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPos, Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            rotation += 0.03f;
            float alpha = timeLeft <= 20 ? 1f - 1f / 20f * (20 - timeLeft) : 1f;
            if (alpha < 0f) alpha = 0f;
            spriteBatch.Draw(texture, Center - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), color, rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), 1f * Scale, SpriteEffects.None, 0f);

            return false;
        }
    }
    public class CoreFireParticle : Particle
    {
        private int frameCount;
        private int frameTick;
        public override string Texture => "Terraria/Images/Item_" + ItemID.BambooDoor;
        public override void SetDefaults()
        {
            width = 34;
            height = 34;
            Scale = 1f;
            timeLeft = 40;
        }


        public override void AI()
        {
            rotation = velocity.ToRotation();

            velocity *= 0.98f;
            Scale *= 1.05f;
            if (Scale <= 0f)
                active = false;
            opacity = 125f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Divergency/Assets/Textures/FireBreath").Value;
            Texture2D tex3 = ModContent.Request<Texture2D>("Divergency/Assets/Textures/ParticleTextures/Ellipsis").Value;

            float alpha = timeLeft <= 20 ? 1f - 1f / 20f * (20 - timeLeft) : 1f;
            if (alpha < 0f) alpha = 0f;
            Color color = Color.Multiply(new(0.50f, 2.05f, 0.5f, 0), alpha / 2);
            Color color2 = Color.Multiply(new(0.50f, 2.05f, 0.5f, 0), alpha / 5);

            spriteBatch.Draw(tex, position - Main.screenPosition, tex.AnimationFrame(ref frameCount, ref frameTick, 7, 7, true), color2, 0f, new Vector2(tex.Width / 2f, tex.Height / 2f / 7f), Scale / 3.2f, SpriteEffects.None, 0f);
            spriteBatch.Draw(tex3, position - Main.screenPosition, new Rectangle(0, 0, tex3.Width, tex3.Height), color, rotation, new Vector2(tex3.Width / 2f, tex3.Height / 2f), 0.17f * Scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}