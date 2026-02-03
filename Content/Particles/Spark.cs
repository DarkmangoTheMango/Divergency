using ParticleLibrary;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace Divergency.Content.Particles;

public class CoreSparkle : Particle
{
    public override string Texture => "Divergency/Assets/Textures/Light";

    public override void SetDefaults()
    {
        width = 1;
        height = 1;
        timeLeft = 100;
    }

    public override void AI()
    {
        if (Scale <= 0.1f)
            Kill();

        velocity *= 0.9f;
        Scale *= 0.9f;

        Lighting.AddLight(Center, new Color(96, 214, 72).ToVector3() * Scale);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPos, Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

        Color color = new Color(96, 214, 72, 0) * Scale;
        Vector2 scaleV = new Vector2(0.5f, 1f) * scale;

        Main.EntitySpriteDraw(texture, Center - Main.screenPosition, texture.Bounds, color, 0, texture.Size() * 0.5f, scaleV, SpriteEffects.None, 0f);
        Main.EntitySpriteDraw(texture, Center - Main.screenPosition, texture.Bounds, color, MathHelper.PiOver2, texture.Size() * 0.5f, scaleV, SpriteEffects.None, 0f);

        color = new Color(191, 255, 119, 0) * Scale;
        scaleV = new Vector2(0.2f, 0.5f) * scale;

        Main.EntitySpriteDraw(texture, Center - Main.screenPosition, texture.Bounds, color, 0, texture.Size() * 0.5f, scaleV, SpriteEffects.None, 0f);
        Main.EntitySpriteDraw(texture, Center - Main.screenPosition, texture.Bounds, color, MathHelper.PiOver2, texture.Size() * 0.5f, scaleV, SpriteEffects.None, 0f);


        return false;
    }
}

public class Spark : Particle
{
    public override string Texture => "Divergency/Assets/Textures/Beam";
    public override void SetDefaults()
    {
        width = 1;
        height = 1;
        timeLeft = 100;
    }
    
    public override void AI()
    {
        rotation = velocity.ToRotation();

        if (Scale <= 0.1f)
            Kill();

        velocity *= 0.9f;
        Scale *= 0.9f;

        Lighting.AddLight(Center, new Color(96, 214, 72).ToVector3() * Scale);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPos, Color lightColor)
    {
		Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

        Color color = new Color(96, 214, 72, 0) * Scale;
        Vector2 scaleV = new Vector2(0.5f, 1f) * scale;

        spriteBatch.Draw(texture, Center - Main.screenPosition, texture.Bounds, color, rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scaleV, SpriteEffects.None, 0f);

        color = new Color(191, 255, 119, 0) * Scale;
        scaleV = new Vector2(0.2f, 0.5f) * scale;

        spriteBatch.Draw(texture, Center - Main.screenPosition, texture.Bounds, color, rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scaleV, SpriteEffects.None, 0f);

        return false;
	}
}

public class Spark2 : Particle
{
    public override string Texture => "Divergency/Assets/Textures/Beam";

    public override void SetDefaults()
    {
        width = 1;
        height = 1;
        timeLeft = 60;
    }

    bool init;

    bool doesntCollides;

    public override void AI()
    {
        if (init)
        {
            if (Collision.SolidTiles(position, 1, 1))
            {
                doesntCollides = true;
            }
        }

        rotation = velocity.ToRotation();

        velocity.X *= 0.95f;
        velocity.Y += 0.5f;
        Scale *= 0.95f;

        Lighting.AddLight(position, Color.Lerp(new Color(105, 184, 225, 0), new Color(164, 225, 105, 0), timeLeft / 60f).ToVector3() * Scale);

        Vector2 oldVelocity = velocity;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPos, Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

        spriteBatch.Draw(texture, Center - Main.screenPosition, texture.Bounds, Color.Lerp(new Color(105, 184, 225, 0), new Color(164, 225, 105, 0), timeLeft / 60f), rotation + MathHelper.PiOver2, texture.Size() * 0.5f, Scale * new Vector2(0.5f, 1), SpriteEffects.None, 0f);

        return false;
    }
}
