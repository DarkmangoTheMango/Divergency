using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ParticleLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Divergency.Content.Particles
{
	public class Spark : Particle
	{
        public override string Texture => "Divergency/Assets/Textures/Beam";

		public override void SetDefaults()
		{
			width = 1;
			height = 1;
			timeLeft = 30;
        }

		public override void AI()
        {
            rotation = velocity.ToRotation();

			velocity *= 0.9f;
            velocity.Y += 0.1f;
            scale *= 0.9f;

			Lighting.AddLight(position, Color.Lerp(new Color(255, 8, 0, 0), new Color(231, 255, 43, 0), (float)timeLeft / 30).ToVector3() * 0.2f);
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPos, Color lightColor)
		{
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            float alpha = timeLeft > 40f ? (20f - (timeLeft - 40f)) / 20f : timeLeft <= 20f ? timeLeft / 20f : 1f;

            spriteBatch.Draw(texture, Center - Main.screenPosition, texture.Bounds, Color.Lerp(new Color(255, 8, 0, 0), new Color(231, 255, 43, 0), (float)timeLeft / 30) * alpha, rotation + MathHelper.PiOver2, texture.Size() * 0.5f, Scale * new Vector2(0.2f, 1), SpriteEffects.None, 0f);

			return false;
		}
	}
}
