using Divergency.Content.Particles;
using ParticleLibrary;
using System;
using System.Security.Cryptography.X509Certificates;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.ID;

namespace Divergency.Content.Projectiles.Magic;

public class PhotosynthesisBolt : ModProjectile
{
    public override string Texture => "Divergency/Assets/Textures/Empty";

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
    }

    public override void SetDefaults()
    {
        Projectile.penetrate = 3;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.stopsDealingDamageAfterPenetrateHits = true;

        Projectile.Size = new(16);
        Projectile.scale = 1;

        Projectile.tileCollide = true;
        Projectile.ignoreWater = false;

        Projectile.aiStyle = -1;
        Projectile.timeLeft = 400;
        Projectile.MaxUpdates = 3;
    }

    public override void OnSpawn(IEntitySource source)
    {
        for (int k = 0; k < ProjectileID.Sets.TrailCacheLength[Projectile.type]; k++)
        {
            Projectile.oldPos[k] = Projectile.position - (Projectile.velocity * k * 0.001f);
        }
    }

    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        for (int k = 0; k < 5; k++)
            ParticleManager.NewParticle<GreenSpark>(Projectile.Center, Projectile.velocity.RotatedByRandom(0.5f) * (Main.rand.NextFloat(0.5f) + 0.1f), default, 2, 1);

        if (Projectile.penetrate <= 1)
            Projectile.velocity = Vector2.Zero;
    }

    public override void OnKill(int timeLeft)
    {

    }

    #region Drawing

    // i don't feel like writing a summary for this whole thing; basically, it's a really lazy way to lerp between 3 colors (i should probably make a helper function for this at some point...)
    static Color LerpThreeColors(Color colorA, Color colorB, Color colorC, float progress)
    {
        if (progress < 0.5f)
        {
            float t = progress / 0.5f;
            return Color.Lerp(colorA, colorB, t);
        }
        else
        {
            float t = (progress - 0.5f) / 0.5f;
            return Color.Lerp(colorB, colorC, t);
        }
    }

    float timer;

    public override bool PreDraw(ref Color lightColor)
    {
        timer += 0.5f;

        if (Projectile.oldPos[1] == Vector2.Zero)
            return false;

        // terraria's built-in trail renderer is shit, so we use a custom texture instead of a width function
        Texture2D Texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/CrimeAgainstHumanity").Value;

        VertexStrip strip = new();

        strip.PrepareStripWithProceduralPadding(
            positions: Projectile.oldPos,
            rotations: Projectile.oldRot,
            colorFunction: progress =>
                LerpThreeColors(
                    new Color(255, 255, 255, 0),
                    new Color(128, 255, 0, 0),
                    new Color(0, 128, 128, 0),
                    progress + (MathF.Sin(timer) * 0.2f)
                    ),
            widthFunction: progress => 20 + (MathF.Sin(timer) * 3f),
            offsetForAllPositions: Projectile.Size / 2f - Main.screenPosition,
            includeBacksides: false,
            tryStoppingOddBug: true
        );

        Main.graphics.GraphicsDevice.Textures[0] = Texture;
        strip.DrawTrail();

        return false;
    }

    #endregion Drawing
}