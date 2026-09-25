using Divergency.Content.Items.Weapons.Melee;
using Divergency.Content.Particles;
using Divergency.Content.Projectiles.Magic;
using ParticleLibrary;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Divergency.Content.Items.Weapons.Ranged;

public class MuscorePro : ModProjectile
{
    Player Player => Main.player[Projectile.owner];

    Vector2 VisualOffset;

    public override string Texture => "Divergency/Content/Items/Weapons/Ranged/Muscore";

    public override bool ShouldUpdatePosition() => false;

    public override bool? CanCutTiles() => false;

    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Projectile.Size = new(16);

        Projectile.DamageType = DamageClass.Ranged;
        Projectile.penetrate = -1;

        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;

        Projectile.aiStyle = -1;
    }

    public override void OnSpawn(IEntitySource source)
    {
        for (int k = 0; k < 10; k++)
        {
            ParticleManager.NewParticle<GreenSpark>(Projectile.Center + Projectile.velocity * 50, Projectile.velocity.RotatedByRandom(0.3f) * (Main.rand.NextFloat(20) + 1), default, 2, 1);
        }

        SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/MuscoreShoot") { PitchVariance = 0.1f }, Projectile.Center);
        Projectile.localAI[0] = 1;

        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 30, ModContent.ProjectileType<PhotosynthesisBolt>(), Projectile.damage, 1, Projectile.owner);
    }

    #region AI

    public override void AI()
    {
        if (!Player.active || Player.itemTime <= 1)
            Projectile.Kill();

        Projectile.timeLeft = 2;

        Animate();

        Player.heldProj = Projectile.whoAmI;
        Projectile.spriteDirection = Projectile.direction = Player.direction;
        Projectile.velocity = Player.itemRotation.ToRotationVector2();
        Projectile.Center = Player.RotatedRelativePoint(Player.MountedCenter, false, true);
        Projectile.rotation = Player.itemRotation;

        VisualOffset = Main.rand.NextVector2CircularEdge(1, 1) * Projectile.localAI[0];
        Projectile.localAI[0] *= 0.9f;

        Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.2f, 0.1f) * Projectile.localAI[0]);
    }

    void Animate()
    {

    }

    #endregion AI

    public override void OnKill(int timeLeft)
    {

    }

    #region Drawing

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

        int frameHeight = texture.Height / Main.projFrames[Projectile.type];
        Rectangle sourceRectangle = new(0, frameHeight * Projectile.frame, texture.Width, frameHeight);

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Projectile.velocity * 22 * Player.direction, sourceRectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, sourceRectangle.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

        DrawGlow();

        return false;
    }

    void DrawGlow()
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;

        int frameHeight = texture.Height / Main.projFrames[Projectile.type];
        Rectangle sourceRectangle = new(0, frameHeight * Projectile.frame, texture.Width, frameHeight);

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Projectile.velocity * 22 * Player.direction + VisualOffset, sourceRectangle, new Color(255, 255, 255, 0) * Projectile.localAI[0], Projectile.rotation, sourceRectangle.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
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