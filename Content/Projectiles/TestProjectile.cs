using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Divergency.Content.Projectiles;

public class TestProjectile : ModProjectile
{
    public override bool ShouldUpdatePosition() => false;

    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 7;
    }

    public override void SetDefaults()
    {
        Projectile.Size = new(98);
        Projectile.scale = 1.2f;
    }

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);

        SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/Explosion") { PitchVariance = 0.1f }, Projectile.Center);

        CameraSystem.ScreenShake(8, 0.9f, Projectile.Center);
    }

    public override void OnKill(int timeLeft)
    {

    }

    #region AI

    public override void AI()
    {
        UpdateFrame(2);

        Projectile.scale += 0.06f;
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

    #region Drawing

    public override bool PreDraw(ref Color lightColor)
    {
        DrawBloom();

        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

        int frameHeight = texture.Height / Main.projFrames[Projectile.type];
        Rectangle sourceRectangle = new(0, frameHeight * Projectile.frame, texture.Width, frameHeight);

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, new Color(255, 255, 255, 128), Projectile.rotation, sourceRectangle.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

        return false;
    }

    void DrawBloom()
    {
        Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Bloom").Value;

        Rectangle sourceRectangle = new(0, 0, texture.Width, texture.Height);

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, new Color(255, 128, 0, 0) * 0.5f, Projectile.rotation, sourceRectangle.Size() * 0.5f, Projectile.scale * 0.2f, SpriteEffects.None, 0);
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
