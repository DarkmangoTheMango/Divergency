using Divergency.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Projectiles.Ranged.Doors
{
    public class WoodenDoor : ModProjectile
    {
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 16;

            return true;
        }

        public override void SetStaticDefaults()
        {
            //.setdefault("Door");
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.Size = new Vector2(48);
            Projectile.scale = 1f;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;

            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 30f)
            {
                Projectile.rotation += Projectile.velocity.Length() * 0.02f;

                Projectile.velocity.Y += 0.5f;
                Projectile.velocity.X *= 0.98f;
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            if (Projectile.velocity.Y >= 16f)
            {
                Projectile.velocity.Y = 16f;
            }
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];

            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Impacts/WoodenDoorBreak") with { PitchVariance = 0.4f });

            CameraSystem.ScreenShake(3);

            if (!Main.dedServ)
            {
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, -Projectile.velocity * 0.1f, Mod.Find<ModGore>("WoodenDoor1").Type);
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, -Projectile.velocity * 0.1f, Mod.Find<ModGore>("WoodenDoor2").Type);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color color = Projectile.GetAlpha(lightColor);

            SpriteEffects spriteEffects = SpriteEffects.None;

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            return false;
        }
    }
}