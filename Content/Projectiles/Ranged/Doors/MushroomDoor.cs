using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Divergency.Content.Buffs;
using Divergency.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Divergency.Content.Projectiles.Ranged.Doors
{
    public class MushroomDoor : ModProjectile
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

            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/Mushroom") with { PitchVariance = 0.4f });

            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MushroomDoorWave>(), 0, 0, Projectile.owner);

            player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity = 3f;
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

    public class MushroomDoorWave : ModProjectile
    {
        public Trail trail;

        public Trail trail2;

        float radius = 0;

        float timer = 0;

        float width = 100;

        float healTime;

        bool instanceHealSound = true;

        public override string Texture => "Divergency/Assets/Textures/Empty";

        public override void SetStaticDefaults()
        {
            //.setdefault("Door");
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;

            Projectile.Size = new Vector2(16);
            Projectile.scale = 1f;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            radius += (100 - radius) / 5f;

            if (radius >= 50)
            {
                width += (0 - width) / 5f;
            }

            if (player.Distance(Projectile.Center) <= 100)
            {
                if (instanceHealSound)
                {
                    SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/MushroomHeal") with { PitchVariance = 0.4f }, Projectile.Center);
                }

                instanceHealSound = false;

                if (healTime >= 30)
                {
                    player.Heal(10);

                    healTime = 0;
                }

                healTime++;
            }
            else
            {
                instanceHealSound = true;
            }

            if (Main.rand.NextBool(3))
            {
                Dust.NewDustPerfect(Projectile.Center.findGroundUnder() + new Vector2(Main.rand.NextFloat(-radius, radius), 0), ModContent.DustType<Glow>(), new Vector2(0, Main.rand.NextFloat(-1, -2)), 0, new Color(0, 98, 255, 100), Main.rand.NextFloat(0.5f, 0.7f));
            }

            if (Main.rand.NextBool(20))
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.findGroundUnder() + new Vector2(Main.rand.NextFloat(-radius, radius), 0), Vector2.Zero, ModContent.ProjectileType<MushroomDoorMushroom>(), 0, 0, Projectile.owner);
            }
            
            Projectile.ai[0]++;

            if (Projectile.ai[0] >= Main.rand.NextFloat(360, 600))
            {
                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/MushroomIdle") with { PitchVariance = 0.4f }, Projectile.Center);

                Projectile.ai[0] = 0;
            }

            Lighting.AddLight(Projectile.Center, new Color(0, 98, 255, 100).ToVector3() * 0.001f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Shadow").Value;

            if (trail == null)
            {
                trail = new Trail(texture, Trail.DefaultPass, (p) => new Vector2(width), (p) => Projectile.GetAlpha(new Color(0, 98, 255, 100)));
                trail.drawOffset = Projectile.Size / 2f;

                trail2 = new Trail(texture, Trail.DefaultPass, (p) => new Vector2(width / 1.9f), (p) => Projectile.GetAlpha(new Color(255, 255, 255, 100)));
                trail2.drawOffset = Projectile.Size / 2f;
            }

            int parts = 10;
            Vector2[] tests = new Vector2[parts];
            float[] rotations = new float[parts];

            for (int point = 0; point < parts; point++)
            {
                float rad = ((float)point / (parts - 1)) * MathHelper.TwoPi;

                tests[point] = Projectile.position + new Vector2(MathF.Cos(rad) * radius, MathF.Sin(rad) * radius);
                rotations[point] = rad;
            }

            timer -= 0.01f;

            trail.Draw(tests, rotations, timer);
            trail2.Draw(tests, rotations, timer);

            return false;
        }
    }

    public class MushroomDoorMushroom : ModProjectile
    {
        float widthMod = 0;

        float heightMod = 2;

        float timer;

        bool spawned = false;

        public override void SetStaticDefaults()
        {
            //.setdefault("Mushroom");

            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;

            Projectile.Size = new Vector2(16);
            Projectile.scale = 1f;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (!spawned)
            {
                Projectile.ai[1] = Main.rand.Next(0, 5);
                Projectile.frame = (int)Projectile.ai[1];
                Projectile.timeLeft = Main.rand.Next(100, 300);

                spawned = true;
            }

            if (Projectile.timeLeft <= 20)
            {
                widthMod += (0 - widthMod) / 5f;
                heightMod += (0 - heightMod) / 5f;
            }
            else
            {
                widthMod += (1 - widthMod) / 5f;
                heightMod += (1 - heightMod) / 5f;
            }

            timer += 0.02f;

            Projectile.rotation = ((float)Math.Cos(timer) / 4);

            Lighting.AddLight(Projectile.Center, new Color(0, 98, 255).ToVector3());
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);

            Vector2 origin = new Vector2(sourceRectangle.Size().X / 2f, sourceRectangle.Size().Y);
            Vector2 position = Projectile.Bottom - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            Color color = Projectile.GetAlpha(Color.White);

            SpriteEffects spriteEffects = SpriteEffects.None;

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, new Vector2(widthMod, heightMod), spriteEffects, 0);

            return false;
        }
    }
}