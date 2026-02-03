using Divergency.Common.Helpers;
using Divergency.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using ParticleLibrary;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.Melee;

public class CoreCrystalizePro : ModProjectile
{
    private Player Player => Main.player[Projectile.owner];
    private float MaxTimeLeft;
    private float Progress;
    private Vector2 InitVelocity;
    private int TrailLength = 6;
    private float[] oldSwingAngles;
    private float[] oldScales;
    private bool trailInitialized;
    private bool initialized;
    private bool soundPlayed1;
    private bool soundPlayed2;
    private int hitFreezeTime;

    public override string Texture => "Divergency/Content/Items/Weapons/Melee/CoreCrystalize";

    public override bool ShouldUpdatePosition() => false;

    public override bool? CanHitNPC(NPC target) => hitFreezeTime <= 0;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        ProjectileID.Sets.AllowsContactDamageFromJellyfish[Type] = true;
        ProjectileID.Sets.NoMeleeSpeedVelocityScaling[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.Size = new Vector2(176);
        Projectile.penetrate = 3;
        Projectile.friendly = true;
        Projectile.stopsDealingDamageAfterPenetrateHits = true;
        Projectile.DamageType = DamageClass.Melee;

        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.ownerHitCheck = true;
        Projectile.usesOwnerMeleeHitCD = true;

        Projectile.MaxUpdates = 3;
        Projectile.aiStyle = -1;
        Projectile.hide = true;
    }

    #region Behavior

    public override void OnSpawn(IEntitySource source)
    {
        oldSwingAngles = new float[TrailLength];
        oldScales = new float[TrailLength];

        for (int i = 0; i < TrailLength; i++)
        {
            oldSwingAngles[i] = 0f;
            oldScales[i] = Projectile.scale;
        }

        trailInitialized = true;

        MaxTimeLeft = Player.HeldItem.useTime * Projectile.MaxUpdates;
        Projectile.timeLeft = (int)MaxTimeLeft;

        Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.One);
        InitVelocity = Projectile.velocity;

        AI();
    }

    public override void AI()
    {
        HandleHitFreeze();
        UpdateProgress();
        HandleSounds();
        float swingAngle = CalculateSwingAngle();

        UpdateTrail(swingAngle);
        UpdateProjectilePosition(swingAngle);
    }

    private void HandleHitFreeze()
    {
        if (hitFreezeTime > 0)
        {
            hitFreezeTime--;
            Projectile.timeLeft++;
        }
    }

    private void UpdateProgress()
    {
        if (hitFreezeTime > 0)
            return;

        Progress = 1 - Projectile.timeLeft / MaxTimeLeft;

        if (Progress >= 0.3f && Progress <= 0.6f && Main.rand.NextBool(2))
            ParticleManager.NewParticle<CoreSparkle>(Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (Projectile.width * 0.5f) * Main.rand.NextFloat(Projectile.scale), (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * 5 * (Projectile.ai[0] * Player.direction), default, 1);
    }

    private void HandleSounds()
    {
        if (!soundPlayed1 && Progress >= 0.4f)
        {
            soundPlayed1 = true;
            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/Swing", 5) { Pitch = -0.5f, PitchVariance = 0.1f, MaxInstances = 5, PauseBehavior = PauseBehavior.PauseWithGame }, Projectile.Center);

            Projectile.soundDelay = 10;
        }

        if (soundPlayed1 && !soundPlayed2 && Projectile.soundDelay == 0)
        {
            soundPlayed2 = true;
            SoundEngine.PlaySound(SoundID.Item71 with { Pitch = -0.5f, PitchVariance = 0.1f, MaxInstances = 5, PauseBehavior = PauseBehavior.PauseWithGame }, Projectile.Center);
        }
    }

    private float CalculateSwingAngle()
    {
        float t = Progress < 0.4f ? EaseFunction.EaseSineIn.Ease(Progress / 0.4f) * 0.3f : 0.3f + EaseFunction.EaseCubicOut.Ease((Progress - 0.4f) / 0.6f) * 0.7f;

        return MathF.Sin((t * 2f - 1f) * MathHelper.PiOver2 * Projectile.ai[0] * Player.direction);
    }

    private void UpdateTrail(float angle)
    {
        if (!initialized)
        {
            for (int i = 0; i < TrailLength; i++)
            {
                oldSwingAngles[i] = angle;
                oldScales[i] = Projectile.scale;
            }

            initialized = true;
        }

        if (!trailInitialized || hitFreezeTime > 0)
            return;

        for (int i = TrailLength - 1; i > 0; i--)
        {
            oldSwingAngles[i] = oldSwingAngles[i - 1];
            oldScales[i] = oldScales[i - 1];
        }

        oldSwingAngles[0] = angle;
        oldScales[0] = Projectile.scale;
    }

    private void UpdateProjectilePosition(float angle)
    {
        Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, InitVelocity.RotatedBy(angle * 1.7f).ToRotation() - MathHelper.PiOver2);

        float swingMagnitude = 1f - MathF.Abs(angle);
        Projectile.scale = (Player.GetAdjustedItemScale(Player.HeldItem) + swingMagnitude * 0.8f) * Player.GetAdjustedItemScale(Player.HeldItem);
        Projectile.velocity = InitVelocity.RotatedBy(angle * 3.9f);

        Projectile.Center = Player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, InitVelocity.RotatedBy(angle * 1.7f).ToRotation() - MathHelper.PiOver2) + Projectile.velocity * Projectile.scale * 63 + new Vector2(0, Player.gfxOffY);

        Projectile.rotation = Projectile.velocity.ToRotation();
        Player.heldProj = Projectile.whoAmI;


        if (Projectile.timeLeft >= 4)
            Player.SetDummyItemTime(2);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Player.Heal(5);

        for (int k = 0; k < 20; k++)
            ParticleManager.NewParticle<Spark>(target.Center, Main.rand.NextVector2CircularEdge(2, 0.5f) * Main.rand.NextFloat(5, 20), default, 2f);

        SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Custom/Blood", 0, 3) { PitchVariance = 0.1f, Volume = 0.5f });
        SoundEngine.PlaySound(SoundID.Item84 with { MaxInstances = 1 }, target.Center);

        Main.instance.CameraModifiers.Add(new ScreenShakeModifier(10, 0.95f, target.Center, 1000f));

        Projectile.damage = (int)(Projectile.damage * 0.8f);
        hitFreezeTime = 30;
    }

    #endregion Behavior

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        if (Progress <= 0.3f) return false;

        Vector2 direction = Projectile.rotation.ToRotationVector2();
        Vector2 center = Projectile.Center;
        Vector2 top = center + direction * Projectile.height * Projectile.scale / 2f;
        Vector2 bottom = center - direction * Projectile.height * Projectile.scale / 2f;

        float collisionPoint = 0f;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), bottom, top, 60 * Projectile.scale, ref collisionPoint);
    }

    #region Drawing

    public override bool PreDraw(ref Color lightColor)
    {
        DrawTrail();

        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        Texture2D glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;

        Vector2 offset = Main.rand.NextVector2Circular(1, 1) * (hitFreezeTime * 0.5f);

        Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, texture.Bounds, lightColor, Projectile.rotation + MathHelper.PiOver4 + (Projectile.ai[0] * Player.direction > 0 ? 0 : MathHelper.PiOver2), texture.Size() * 0.5f, Projectile.scale, Projectile.ai[0] * Player.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

        Main.spriteBatch.Draw(glowTexture, Projectile.Center - Main.screenPosition, glowTexture.Bounds, Color.White, Projectile.rotation + MathHelper.PiOver4 + (Projectile.ai[0] * Player.direction > 0 ? 0 : MathHelper.PiOver2), glowTexture.Size() * 0.5f, Projectile.scale, Projectile.ai[0] * Player.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

        DrawStar();

        return false;
    }

    private void DrawTrail()
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;

        for (int k = TrailLength - 1; k >= 0; k--)
        {
            float angle = oldSwingAngles[k];
            float scale = oldScales[k];

            Vector2 vel = InitVelocity.RotatedBy(angle * 3.9f);
            Vector2 pos = Player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, InitVelocity.RotatedBy(angle * 1.7f).ToRotation() - MathHelper.PiOver2) + vel * scale * 63 + new Vector2(0, Player.gfxOffY);

            float t = 1f - MathF.Abs(angle);

            Color color = Color.White with { A = 50 } * (1 - (k / (float)TrailLength));

            Main.EntitySpriteDraw(texture, pos - Main.screenPosition, null, color * (t * 0.5f), vel.ToRotation() + MathHelper.PiOver4 + (Projectile.ai[0] * Player.direction > 0 ? 0 : MathHelper.PiOver2), texture.Size() * 0.5f, scale * 1.2f, Projectile.ai[0] * Player.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        }
    }

    private void DrawStar()
    {
        Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Beam").Value;

        Vector2 offset = Projectile.rotation.ToRotationVector2() * Projectile.scale * 82f;
        Vector2 drawPosition = Projectile.Center + offset - Main.screenPosition + new Vector2(0, Player.gfxOffY);

        float t = Progress < 0.4f ? EaseFunction.EaseSineIn.Ease(Progress / 0.4f) * 0.3f : 0.3f + EaseFunction.EaseCubicOut.Ease((Progress - 0.4f) / 0.6f) * 0.7f;

        float alpha = 1f - MathF.Abs(t - 0.5f) * 2f;

        Color color1 = new Color(96, 214, 72, 0) * alpha;
        Color color2 = new Color(191, 255, 119, 0) * alpha;

        void DrawBeam(Vector2 scale, float rotation, Color c) => Main.spriteBatch.Draw(texture, drawPosition, texture.Bounds, c, rotation, texture.Size() * 0.5f, scale * ((alpha * 0.5f) + 0.5f) * 2, SpriteEffects.None, 0);

        DrawBeam(new Vector2(0.5f, 3f), 0, color1);
        DrawBeam(new Vector2(0.5f, 2f), MathHelper.PiOver2, color1);
        DrawBeam(new Vector2(0.2f, 1.5f), 0, color2);
        DrawBeam(new Vector2(0.2f, 1f), MathHelper.PiOver2, color2);
    }

    #endregion Drawing
}