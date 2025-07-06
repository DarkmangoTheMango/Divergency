using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stratum.Content.Particles
{
    public class StarParticle2 : Particle
    {
        public override string Texture => "Divergency/Assets/Textures/ParticleTextures/SmallStar";

        public override void SetDefaults()
        {
            width = 1;
            height = 1;
            timeLeft = 100;
        }

        public override void AI()
        {
            velocity *= 0.9f;
            scale *= 0.9f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPos, Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 drawPosition = Center - Main.screenPosition;

            float pulse = 1f + 0.1f * (float)Math.Sin(Main.GameUpdateCount * 0.5f);

            float t = (float)(Math.Sin(Main.GameUpdateCount * 0.5f) * 0.5f + 0.5f);
            Color interpolatedColor = Color.Lerp(new(0, 255, 0, 0), new(255, 255, 255, 0), t);

            Main.EntitySpriteDraw(texture, drawPosition, texture.Frame(), interpolatedColor, rotation, texture.Frame().Size() * 0.5f, scale * pulse, SpriteEffects.None, 0f);

            Color color = new(255, 255, 255, 0);
            Main.EntitySpriteDraw(texture, drawPosition, texture.Frame(), color, rotation, texture.Frame().Size() * 0.5f, (scale * pulse) * 0.9f, SpriteEffects.None, 0f);

            return false;
        }
    }
}