using ParticleLibrary;
using System;

namespace Divergency.Content.Particles;

public class GreenSpark : Particle
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
        Scale *= 0.9f;
        velocity *= 0.9f;
        rotation = velocity.ToRotation() + MathHelper.PiOver2;

        layer = Layer.BeforeTiles;
    }

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPos, Color lightColor)
    {
        Vector2 scale2 = new Vector2(0.5f, 1.6f) * Scale;

        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(default, BlendState.Additive, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

        spriteBatch.Draw(texture, Center - Main.screenPosition, null, Color.Lerp(Color.Transparent, color, MathF.Pow(timeLeft / 30f, 3f)), rotation, texture.Size() * 0.5f, scale2, 0, 0f);
        spriteBatch.Draw(texture, Center - Main.screenPosition, null, Color.Lerp(Color.Transparent, color, MathF.Pow(timeLeft / 30f, 3f)), rotation, texture.Size() * 0.5f, scale2 * new Vector2(0.45f, 1f), 0, 0f);

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(default, default, default, default, default, default, default);

        return false;
	}
}
