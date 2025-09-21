using ParticleLibrary;
using System;

namespace Divergency.Content.Particles;

public class SupernovaFlash : Particle
{
    const int Lifetime = 100;
    const float ScaleDecay = 0.92f;
    const float RotationSpeed = 0.12f;
    const float PulseSpeed = 30f;
    const float PulseStrength = 0.25f;
    const float OuterScale = 4f;
    const float InnerScale = 3f;

    public override string Texture => "Divergency/Assets/Textures/ParticleTextures/SmallStar";

    public override void SetDefaults()
    {
        width = 1;
        height = 1;
        timeLeft = Lifetime;
    }

    public override void AI()
    {
        Lighting.AddLight(Center, new Color(0, 255, 0).ToVector3() * Scale);
        Scale *= ScaleDecay;
        rotation += RotationSpeed;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPos, Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

        Vector2 origin = texture.Size() * 0.5f;
        Vector2 screenPos = Center - Main.screenPosition;

        float pulse = 1f + PulseStrength * MathF.Sin(Main.GlobalTimeWrappedHourly * PulseSpeed);

        Color outerColor = new Color(128, 255, 0, 0) * opacity;
        Color innerColor = new Color(128, 255, 255, 0) * opacity;

        Main.EntitySpriteDraw(texture, screenPos, texture.Bounds, outerColor, rotation, origin, Scale * OuterScale * pulse, SpriteEffects.None, 0f);
        Main.EntitySpriteDraw(texture, screenPos, texture.Bounds, innerColor, rotation, origin, Scale * InnerScale * pulse, SpriteEffects.None, 0f);

        return false;
    }
}
