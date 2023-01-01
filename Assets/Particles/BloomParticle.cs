using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Divergency.Assets.Particles
{
	public class BloomParticle: Particle
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
    public class BloomParticleNPC: Particle
    {
        public override string Texture => "Divergency/Assets/Textures/ParticleTextures/SoftCircle";

        public override void SetDefaults()
        {
            width = 1;
            height = 1;
            opacity = 125;
            layer = Layer.BeforeNPCsBehindTiles;
        }

        public override void AI()
        {
            rotation = velocity.ToRotation();
            NPC npc = Main.npc[(int)ai[0]];
           // if (npc.type == ModContent.NPCType<WraithCore>() && npc.active)
           if (npc.active)
           {
                position = npc.Center;
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

            float alpha = timeLeft <= 20 ? 1f - 1f / 20f * (20 - timeLeft) : 1f;
            if (alpha < 0f) alpha = 0f;
            spriteBatch.Draw(texture, Center - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), color, velocity.ToRotation(), new Vector2(texture.Width / 2f, texture.Height / 2f), 1f * Scale, SpriteEffects.None, 0f);

            return false;
        }
    }
    public class BloomParticleProjectile : Particle
    {
        public override string Texture => "Divergency/Assets/Textures/ParticleTextures/SoftCircle";

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

            float alpha = timeLeft <= 20 ? 1f - 1f / 20f * (20 - timeLeft) : 1f;
            if (alpha < 0f) alpha = 0f;
            spriteBatch.Draw(texture, Center - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), color, velocity.ToRotation(), new Vector2(texture.Width / 2f, texture.Height / 2f), 1f * Scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
