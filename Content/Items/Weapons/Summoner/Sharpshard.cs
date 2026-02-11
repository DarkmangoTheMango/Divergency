using Divergency.Common.Helpers;
using Divergency.Content.Particles;
using ParticleLibrary;
using System;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Localization;

namespace Divergency.Content.Items.Weapons.Summoner;

public class Sharpshard : ModItem
{
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SharpshardTag.TagDamage);

    public override bool MeleePrefix() => true;

    public override void SetDefaults()
    {
        Item.DamageType = DamageClass.SummonMeleeSpeed;
        Item.damage = 20;
        Item.knockBack = 2;
        Item.rare = ItemRarityID.Green;

        Item.shoot = ModContent.ProjectileType<SharpshardPro>();
        Item.shootSpeed = 4;

        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.UseSound = SoundID.Item152;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }
}

public class SharpshardPro : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.IsAWhip[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.width = 18;
        Projectile.height = 18;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ownerHitCheck = true;
        Projectile.extraUpdates = 1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.DamageType = DamageClass.SummonMeleeSpeed;
        Projectile.WhipSettings.Segments = 26;
        Projectile.WhipSettings.RangeMultiplier = 1.3f;
    }

    private float Timer
    {
        get => Projectile.ai[0];
        set => Projectile.ai[0] = value;
    }

    private float ChargeTime
    {
        get => Projectile.ai[1];
        set => Projectile.ai[1] = value;
    }

    public override void AI()
    {
        Player owner = Main.player[Projectile.owner];
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        Projectile.Center = Main.GetPlayerArmPosition(Projectile) + Projectile.velocity * Timer;

        Projectile.spriteDirection = Projectile.velocity.X >= 0f ? 1 : -1;

        Timer++;

        float swingTime = owner.itemAnimationMax * Projectile.MaxUpdates;
        if (Timer >= swingTime || owner.itemAnimation <= 0)
        {
            Projectile.Kill();
            return;
        }

        owner.heldProj = Projectile.whoAmI;

        if (Timer == swingTime / 2)
        {
            List<Vector2> points = Projectile.WhipPointsForCollision;
            Projectile.FillWhipControlPoints(Projectile, points);
            SoundEngine.PlaySound(SoundID.Item153, points[points.Count - 1]);
        }

        float swingProgress = Timer / swingTime;

        if (Utils.GetLerpValue(0.1f, 0.7f, swingProgress, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, swingProgress, clamped: true) > 0.5f && !Main.rand.NextBool(3))
        {
            List<Vector2> points = Projectile.WhipPointsForCollision;
            points.Clear();
            Projectile.FillWhipControlPoints(Projectile, points);
            int pointIndex = Main.rand.Next(points.Count - 10, points.Count);
            Rectangle spawnArea = Utils.CenteredRectangle(points[pointIndex], new Vector2(30f, 30f));

            Vector2 spinningPoint = points[pointIndex] - points[pointIndex - 1];
            ParticleManager.NewParticle<CoreSparkle>(points[pointIndex], spinningPoint.RotatedBy(owner.direction * ((float)Math.PI / 2f)) * 0.5f, default, 1);
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        for (int k = 0; k < 10; k++)
            ParticleManager.NewParticle<Spark>(target.Center, Main.rand.NextVector2CircularEdge(2, 0.5f) * Main.rand.NextFloat(3, 10), default, 2f);

        target.AddBuff(ModContent.BuffType<SharpshardTag>(), 240);
        Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
        Projectile.damage = (int)(Projectile.damage * 0.7f);
    }

    private static void DrawLine(List<Vector2> list)
    {
        Texture2D texture = TextureAssets.FishingLine.Value;
        Rectangle frame = texture.Frame();
        Vector2 origin = new(frame.Width / 2, 2);

        Vector2 pos = list[0];
        for (int i = 0; i < list.Count - 2; i++)
        {
            Vector2 element = list[i];
            Vector2 diff = list[i + 1] - element;

            float rotation = diff.ToRotation() - MathHelper.PiOver2;
            Color color = new Color(96, 214, 72);
            Vector2 scale = new Vector2(1, (diff.Length() + 2) / frame.Height);

            Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

            pos += diff;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        List<Vector2> points = [];

        Projectile.FillWhipControlPoints(Projectile, points);

        DrawLine(points);

        SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        Texture2D texture = TextureAssets.Projectile[Type].Value;

        const int frameWidth = 22;
        const int frameHeight = 26;

        Vector2 origin = new(frameWidth / 2f, 12f);
        Vector2 pos = points[0];

        for (int i = 0; i < points.Count - 1; i++)
        {
            int frameIndex;

            int lastSegmentIndex = points.Count - 2;

            if (i == 0)
                frameIndex = 0;
            else if (i == lastSegmentIndex)
                frameIndex = 4;
            else
            {
                int bodySegmentIndex = i - 1;
                int totalBodySegments = lastSegmentIndex - 1;

                float progress = totalBodySegments > 0 ? bodySegmentIndex / (float)totalBodySegments : 0f;

                if (progress < 1f / 3f)
                    frameIndex = 1;
                else if (progress < 2f / 3f)
                    frameIndex = 2;
                else
                    frameIndex = 3;
            }

            Vector2 current = points[i];
            Vector2 diff = points[i + 1] - current;

            Rectangle frame = new(0, frameHeight * frameIndex, frameWidth, frameHeight);

            Color color = Lighting.GetColor(current.ToTileCoordinates());

            if (i > 0)
                color = Color.White;

            float rotation = diff.ToRotation() - MathHelper.PiOver2;
            float scale = 1f;

            if (frameIndex == 4)
            {
                Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out _, out _);
                float t = Timer / timeToFlyOut;

                scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
            }

            Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

            pos += diff;
        }

        return false;
    }
}

public class SharpshardTag : ModBuff
{
    public static readonly int TagDamage = 10;

    public override void SetStaticDefaults()
    {
        BuffID.Sets.IsATagBuff[Type] = true;
    }
}

public class SharpshardTagNPC : GlobalNPC
{
    public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
    {
        if (projectile.npcProj || projectile.trap || !projectile.IsMinionOrSentryRelated)
            return;

        var projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];

        if (npc.HasBuff<SharpshardTag>())
            modifiers.FlatBonusDamage += SharpshardTag.TagDamage * projTagMultiplier;
    }
}