using Divergency.Content.Particles;
using Microsoft.Build.ObjectModelRemoting;
using ParticleLibrary;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;

namespace Divergency.Content.Projectiles.Ranged;

public class CoreFlame : ModProjectile
{
    private int rotationDirection = 1;

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        doesntFollow = true;
        return false;
    }

    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 7;
    }

    public override void SetDefaults()
    {
        Projectile.Size = new(98);
        Projectile.scale = 0.1f;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.friendly = true;
        Projectile.scale = 1f;
        Projectile.penetrate = 3;
        Projectile.ignoreWater = false;
        Projectile.aiStyle = -1;
        AIType = -1;
        Projectile.stopsDealingDamageAfterPenetrateHits = true;
    }

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
    }

    public override void OnKill(int timeLeft)
    {

    }

    #region AI

    Player player => Main.player[Projectile.owner];

    bool doesntFollow = false;

    public override void AI()
    {
        Lighting.AddLight(Projectile.Center, new Color(96, 214, 72).ToVector3() * (Projectile.alpha / 255));

        if (Main.rand.NextBool((int)Projectile.ai[0] + 1) && Projectile.alpha < 128)
            ParticleManager.NewParticle<CoreSparkle>(Projectile.Center + Main.rand.NextVector2Circular(1, 1) * (Projectile.width * 0.5f * (Projectile.scale * 0.5f)), Projectile.velocity * 0.1f, Color.White, 1);

        Projectile.ai[0]++;

        UpdateFrame(2);

        Projectile.scale += 0.1f;

        if (Projectile.alpha >= 255)
            Projectile.Kill();

        Projectile.alpha += 12;

        if (Projectile.alpha > 128)
            Projectile.damage = 0;

        Projectile.Resize((int)(98 * Projectile.scale), (int)(98 * Projectile.scale));

        if (!doesntFollow)
            Projectile.Center += player.velocity;
    }

    void UpdateFrame(int frameSpeed)
    {
        if (++Projectile.frameCounter > frameSpeed)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;

            if (Projectile.frame > Main.projFrames[Projectile.type] - 1)
                Projectile.Kill();
        }
    }

    #endregion AI

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
    {
        width = 16;
        height = 16;

        return true;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Main.player[Projectile.owner].AddBuff(BuffID.DryadsWard, 60);
    }

    #region Drawing

    public override bool PreDraw(ref Color lightColor)
    {
        DrawBloom();

        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

        int frameHeight = texture.Height / Main.projFrames[Projectile.type];
        Rectangle sourceRectangle = new(0, frameHeight * Projectile.frame, texture.Width, frameHeight);

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Projectile.GetAlpha(new(255, 255, 255, 128)), Projectile.rotation, sourceRectangle.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

        return false;
    }

    void DrawBloom()
    {
        Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Bloom2").Value;

        Rectangle sourceRectangle = new(0, 0, texture.Width, texture.Height);

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Projectile.GetAlpha(new(96, 214, 72)), Projectile.rotation, sourceRectangle.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None, 0);

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
    }

    #endregion Drawing

    #region Networking

    public override void SendExtraAI(BinaryWriter writer)
    {

    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {

    }

    #endregion Networking
}
