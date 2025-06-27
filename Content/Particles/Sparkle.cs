using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ParticleLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Divergency.Content.Particles
{
	public class Sparkle : Particle
	{
        public override string Texture => "Divergency/Assets/Textures/Sparkle";

		public override void SetDefaults()
		{
			width = 1;
			height = 1;
			timeLeft = 30;
        }

		public override void AI()
        {
			if (timeLeft == 30)
			{
				rotation = Main.rand.NextFloat(MathHelper.Pi);
			}

            rotation += velocity.Length() * 0.1f;

			velocity *= 0.9f;

			if (timeLeft <= 15)
			{
				scale *= 0.9f;
			}

			Lighting.AddLight(position, Color.Lerp(new Color(255, 8, 0, 0), new Color(231, 255, 43, 0), (float)timeLeft / 30).ToVector3() * 0.2f);
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPos, Color lightColor)
		{
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            float alpha = timeLeft > 40f ? (20f - (timeLeft - 40f)) / 20f : timeLeft <= 20f ? timeLeft / 20f : 1f;

            spriteBatch.Draw(texture, Center - Main.screenPosition, texture.Bounds, Color.Lerp(new Color(255, 8, 0, 0), new Color(231, 255, 43, 0), (float)timeLeft / 30) * alpha, rotation + MathHelper.PiOver2, texture.Size() * 0.5f, Scale, SpriteEffects.None, 0f);

            texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Bloom").Value;

            spriteBatch.Draw(texture, Center - Main.screenPosition, texture.Bounds, Color.Lerp(new Color(255, 8, 0, 0), new Color(231, 255, 43, 0), (float)timeLeft / 30) * (alpha * 0.5f), rotation + MathHelper.PiOver2, texture.Size() * 0.5f, Scale * 0.02f, SpriteEffects.None, 0f);

            return false;
		}
    }

    public class BloomstickFlash : Particle
    {
        float scaleXMod;
        float scaleYMod;

        public override string Texture => "Divergency/Assets/Textures/Beam";

        public override void SetDefaults()
        {
            width = 1;
            height = 1;
            timeLeft = 30;
        }

        public override void AI()
        {
            if (timeLeft == 30)
            {
                scaleXMod = 1;
                if (Main.rand.NextBool(2))
                {
                    scaleXMod = -1;
                }
                rotation = Main.rand.NextFloat(MathHelper.Pi);
                velocity = Vector2.Zero;
            }

            rotation += 0.1f * scaleXMod;
            Scale *= 0.9f;

            Lighting.AddLight(position, Color.Lerp(new Color(255, 8, 0, 0), new Color(231, 255, 43, 0), (float)timeLeft / 30).ToVector3() * 0.2f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPos, Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            spriteBatch.Draw(texture, Center - Main.screenPosition, texture.Bounds, Color.Lerp(new Color(255, 8, 0, 0), new Color(231, 255, 43, 0), (float)timeLeft / 30), rotation, texture.Size() * 0.5f, Scale *
                new Vector2(0.5f, 2f), SpriteEffects.None, 0f);

            spriteBatch.Draw(texture, Center - Main.screenPosition, texture.Bounds, Color.Lerp(new Color(255, 8, 0, 0), new Color(231, 255, 43, 0), (float)timeLeft / 30), rotation + MathHelper.PiOver2, texture.Size() * 0.5f, Scale * new Vector2(0.5f, 2f), SpriteEffects.None, 0f);

            return false;
        }
    }

    public class BloomstickSmoke : Particle
    {
        float scaleXMod;
        float scaleYMod;

        public override string Texture => "Divergency/Assets/Textures/Smoke_0";

        string textureButReal = "Divergency/Assets/Textures/Smoke_";
        int textureNum = 0;

        public override void SetDefaults()
        {
            width = 1;
            height = 1;
            timeLeft = 50;
            Scale = 0.6f;
        }

        public override void AI()
        {
            layer = Layer.BeforeProjectiles;

            if (timeLeft == 50)
            {
                textureNum = Main.rand.Next(0, 3);
                rotation = Main.rand.NextFloat(MathHelper.Pi);
                scaleXMod = Main.rand.NextFloat(-0.01f, 0.01f);
            }

            velocity *= 0.95f;
            rotation += scaleXMod;
            Scale *= 1.01f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPos, Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Smoke_" + textureNum).Value;

            spriteBatch.Draw(texture, Center - Main.screenPosition, texture.Bounds, Color.Lerp(new Color(0, 0, 0, 0), new Color(50, 50, 50, 100), (float)timeLeft / 50), rotation, texture.Size() * 0.5f, Scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
