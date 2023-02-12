using Divergency.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Common.Helpers
{
    public static class DivergencyDraw
    {


        public static void SpawnRing(Vector2 center, Color color, float flatScale = 0.13f, float multiScale = 0.9f, float glowScale = 2)
        {
            //Dust dust = Dust.NewDustPerfect(center, ModContent.DustType<GlowDust>(), Vector2.Zero, Scale: glowScale);
            //dust.noGravity = true;
            Color dustColor = new(color.R, color.G, color.B) { A = 0 };
            //dust.color = dustColor;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int p = Projectile.NewProjectile(null, center, Vector2.Zero, ModContent.ProjectileType<Ring_Visual>(), 0, 0,
                    Main.myPlayer, flatScale, multiScale);
                (Main.projectile[p].ModProjectile as Ring_Visual).color = color;
            }
        }
        public static void SpawnCirclePulse(Vector2 center, Color color, float scale = 1, Entity target = null)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int p = Projectile.NewProjectile(null, center, Vector2.Zero, ModContent.ProjectileType<CirclePulse_Visual>(), 0, 0,
                    Main.myPlayer, scale);
                (Main.projectile[p].ModProjectile as CirclePulse_Visual).color = color;
                (Main.projectile[p].ModProjectile as CirclePulse_Visual).entityTarget = target;
            }
        }
        public static void SpawnExplosion(Vector2 center, Color color, int dustID, float shakeAmount = 7, int dustAmount = 30, float dustScale = 2, float scale = 4f, bool noDust = false, Texture2D tex = null, float rot = 0)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int p = Projectile.NewProjectile(null, center, Vector2.Zero, ModContent.ProjectileType<Explosion_Visual>(), 0, 0,
                    Main.myPlayer, shakeAmount, dustAmount);
                Main.projectile[p].rotation = rot;
                if (Main.projectile[p].ModProjectile is Explosion_Visual explode)
                {
                    explode.color = color;
                    explode.dustID = dustID;
                    explode.dustScale = dustScale;
                    explode.scale = scale;
                    explode.noDust = noDust;
                    explode.texture = tex;
                }
            }
        }
    }
    public class Ring_Visual : ModProjectile
    {
        public override string Texture => "Divergency/Assets/Textures/Ring";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ring");
        }
        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 20;
            Projectile.scale = 0.1f;
        }
        public Color color;
        public override void AI()
        {
            Projectile.scale += Projectile.ai[0]; // 0.13f
            Projectile.scale *= Projectile.ai[1]; // 0.9f
            if (Projectile.timeLeft < 10)
                Projectile.alpha = (int)MathHelper.Lerp(255f, 0f, Projectile.timeLeft / 10f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(color), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
    public class CirclePulse_Visual : ModProjectile
    {
        public override string Texture => "Divergency/Assets/Textures/Shockwave";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pulse");
        }
        public override void SetDefaults()
        {
            Projectile.width = 600;
            Projectile.height = 600;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }
        public Entity entityTarget;
        public override void AI()
        {
            if (entityTarget != null)
            {
                if (entityTarget.active)
                    Projectile.Center = entityTarget.Center;
            }

            Projectile.timeLeft = 10;
            Projectile.velocity *= 0;
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] < 60)
            {
                if (Projectile.localAI[0] < 30)
                    Projectile.alpha -= 5;
                else
                    Projectile.alpha += 5;
                Projectile.scale += 0.003f;
            }
            else
            {
                Projectile.alpha = 255;
                Projectile.scale = 1;
                Projectile.Kill();
            }
        }
        public Color color;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), Projectile.GetAlpha(color), Projectile.rotation, origin, Projectile.scale * Projectile.ai[0], SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
    public class Explosion_Visual : ModProjectile
    {
        public override string Texture => "Divergency/Assets/Textures/Empty";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosion");
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10;
        }

        private float GlowTimer;
        private bool Glow;
        public Color color;
        public int dustID;
        public float dustScale;
        public float scale;
        public bool noDust;
        public Texture2D texture;
        public override void AI()
        {
            if (Glow)
            {
                GlowTimer += 3;
                if (GlowTimer > 60)
                {
                    Glow = false;
                    GlowTimer = 0;
                }
            }
            if (Projectile.localAI[0]++ == 0)
            {
                Glow = true;
                Projectile.alpha = 255;
                Main.LocalPlayer.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity = 3;
                if (!noDust)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        //int dust = Dust.NewDust(Projectile.Center + Projectile.velocity, 1, 1, ModContent.DustType<GlowDust>(), Scale: 2);
                        //Main.dust[dust].velocity *= 6;
                       // Main.dust[dust].noGravity = true;
                        //Color dustColor = new(color.R, color.G, color.B) { A = 0 };
                        //Main.dust[dust].color = dustColor;
                    }
                    for (int i = 0; i < Projectile.ai[1]; i++)
                    {
                        int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustID, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: dustScale);
                        Main.dust[dust].velocity *= 10;
                        Main.dust[dust].noGravity = true;
                    }
                    for (int i = 0; i < Projectile.ai[1]; i++)
                    {
                        int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: dustScale);
                        Main.dust[dust].velocity *= 15;
                        Main.dust[dust].noGravity = true;
                    }
                    if (Main.netMode != NetmodeID.Server)
                    {
                        for (int g = 0; g < 6; g++)
                        {
                            int goreIndex = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64));
                            Main.gore[goreIndex].scale = 1.5f;
                            Main.gore[goreIndex].velocity *= 2f;
                        }
                    }
                }
            }
            if (Projectile.localAI[0] >= 20)
                Projectile.Kill();
        }
        public override void PostDraw(Color lightColor)
        {
            if (texture == null)
                texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/WhiteGlow").Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D teleportGlow = texture;
            Rectangle rect2 = new(0, 0, teleportGlow.Width, teleportGlow.Height);
            Vector2 origin2 = new(teleportGlow.Width / 2, teleportGlow.Height / 2);
            Vector2 position2 = Projectile.Center - Main.screenPosition;
            Color colour2 = Color.Lerp(color, color, 1f / GlowTimer * 10f) * (1f / GlowTimer * 10f);
            if (Glow)
            {
                Main.spriteBatch.Draw(teleportGlow, position2, new Rectangle?(rect2), colour2, Projectile.rotation, origin2, scale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(teleportGlow, position2, new Rectangle?(rect2), colour2 * 0.4f, Projectile.rotation, origin2, scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}