using Divergency.Common.Helpers;
using Divergency.Content.Buffs;
using Divergency.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Projectiles.Hostile
{
    public class WraithFire : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new(32, 34);
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Animate(6);

            if (Main.rand.NextBool(5))
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(1, 1) * 10, DustID.Terra, Vector2.UnitY * -Main.rand.NextFloat(1, 5), 0, default, 1).noGravity = true;
        }

        void Animate(int speed)
        {
            if (++Projectile.frameCounter >= speed)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Type]) { Projectile.frame = 0; }
            }
        }
        
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }

        public Trail trail;

        public override bool PreDraw(ref Color lightColor)
        {
            DrawStar();

            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Bloom").Value;

            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color color = new Color(43, 255, 0, 0);

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale * 0.1f * ((MathF.Sin(Main.GameUpdateCount * 0.1f) * 0.05f) + 0.9f), SpriteEffects.None, 0);

            texture = ModContent.Request<Texture2D>(Texture).Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;

            sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            origin = sourceRectangle.Size() / 2f;
            position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            color = new Color(255, 255, 255, 150);

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        void DrawStar()
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/ParticleTextures/SmallStar").Value;

            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color color = new Color(43, 255, 0, 0);

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale * 0.4f * ((MathF.Sin(Main.GameUpdateCount * 0.1f) * 0.05f) + 0.9f), SpriteEffects.None, 0);

            texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/ParticleTextures/SmallStar").Value;

            sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            origin = sourceRectangle.Size() / 2f;
            position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            color = new Color(255, 255, 255, 0);

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale * 0.2f * ((MathF.Sin(Main.GameUpdateCount * 0.1f) * 0.05f) + 0.9f), SpriteEffects.None, 0);
        }
    }
}
