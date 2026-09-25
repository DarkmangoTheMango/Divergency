using Divergency.Content.Items.Weapons.Melee;
using Divergency.Content.Particles;
using Divergency.Content.Projectiles.Magic;
using ParticleLibrary;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using static System.Net.Mime.MediaTypeNames;

namespace Divergency.Content.Items.Weapons.Ranged;

public class MuscorePro : ModProjectile
{
    Player Player => Main.player[Projectile.owner];

    Vector2 VisualOffset;

    public override string Texture => "Divergency/Content/Items/Weapons/Ranged/MuscorePro";

    public override bool ShouldUpdatePosition() => false;

    public override bool? CanCutTiles() => false;

    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 12;
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
    }

    #region AI

    public override void AI()
    {
        if (!Player.active || Player.itemTime <= 2)
            Projectile.Kill();

        Projectile.timeLeft = 2;

        if (++Projectile.frameCounter >= 4)
        {
            Projectile.frameCounter = 0;

            if (++Projectile.frame >= Main.projFrames[Type])
                Projectile.frame = Main.projFrames[Type] - 1;
        }

        Player.heldProj = Projectile.whoAmI;
        Projectile.spriteDirection = Projectile.direction = Player.direction;
        Projectile.velocity = Player.itemRotation.ToRotationVector2();
        Projectile.Center = Player.RotatedRelativePoint(Player.MountedCenter, false, true);
        Projectile.rotation = Player.itemRotation;

        VisualOffset = Main.rand.NextVector2CircularEdge(1, 1) * Projectile.localAI[0];
        Projectile.localAI[0] *= 0.9f;

        Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.2f, 0.1f) * Projectile.localAI[0]);
    }

    #endregion AI

    public override void OnKill(int timeLeft)
    {

    }

    #region Drawing

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        
        int frameHeight = texture.Height / Main.projFrames[Type];
        int startY = frameHeight * Projectile.frame;

        Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Projectile.velocity * 22 * Player.direction, sourceRectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, sourceRectangle.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

        return false;
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