using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Divergency.Content.Particles
{
	public class FlareLineParticle : Particle
	{
        public override string Texture => "Divergency/Assets/Textures/ParticleTextures/SoftCircle";

        public override void SetDefaults()
		{
			width = 1;
			height = 1;
			timeLeft = 60;
		}

		public override void AI()
        {

            rotation = velocity.ToRotation();

			velocity *= 0.96f;

		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPos, Color lightColor)
		{
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

			float alpha = timeLeft > 40f ? (20f - (timeLeft - 40f)) / 20f : timeLeft <= 20f ? timeLeft / 20f : 1f;

			spriteBatch.Draw(texture, Center - Main.screenPosition, texture.Bounds, color * alpha, rotation, texture.Size() * 0.5f, Scale * new Vector2(0.1f, 0.005f), SpriteEffects.None, 0f);

			return false;
		}
	}
    public class WindParticle : Particle
    {
        public override string Texture => "Divergency/Assets/Textures/ParticleTextures/SoftCircle";

        public override void SetDefaults()
        {
            width = 1;
            height = 1;
            timeLeft = 260;
            opacity = 125;
            layer = Layer.BeforeNPCsBehindTiles;
        }

        public override void AI()
        {
            rotation = velocity.ToRotation();

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPos, Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            float alpha = timeLeft <= 20 ? 1f - 1f / 20f * (20 - timeLeft) : 1f;
            if (alpha < 0f) alpha = 0f;
            spriteBatch.Draw(texture, Center - Main.screenPosition, texture.Bounds, color * alpha, rotation, texture.Size() * 0.5f, Scale * new Vector2(0.1f, 0.005f), SpriteEffects.None, 0f);

            return false;
        }
    }
}
