using Divergency.Common.Players;
using Divergency.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.LivingCore
{
    public class LivingCoreSpear : ModItem
    {
        public override void SetStaticDefaults()
        {
            //.setdefault("Living Core Spear");
            ////.setdefault("Hold <left> to charge the spear");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.damage = 45;
            Item.knockBack = 4f;

            Item.shoot = ModContent.ProjectileType<LivingCoreSpearPro>();
            Item.shootSpeed = 1f;

            Item.width = Item.height = 96;
            Item.scale = 1f;

            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useTurn = false;

            Item.value = Item.sellPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Green;
        }
    }

    public class LivingCoreSpearPro : ModProjectile
    {
        float delay;

        float timer;

        float radius;

        public override bool ShouldUpdatePosition() => false;

        public override void SetStaticDefaults()
        {
            //.setdefault("Living Core Spear");
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.scale = 1.3f;
            Projectile.Size = new Vector2(72);

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool? CanDamage()
        {
            Player player = Main.player[Projectile.owner];

            return !player.channel;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            player.ChangeDir(Projectile.direction);

            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
            player.heldProj = Projectile.whoAmI;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            radius = Projectile.ai[0] / 30f;

            if (player.channel)
            {
                player.itemAnimation = player.itemTime = player.itemAnimationMax;
                Projectile.timeLeft = player.itemAnimationMax;
                Projectile.Center = player.MountedCenter;
                Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld);

                Projectile.ai[0]++;

                if (Projectile.ai[0] == 1f)
                {
                    SoundEngine.PlaySound(SoundID.Item101, player.Center);
                }

                if (Projectile.ai[0] == 60f)
                {
                    SoundEngine.PlaySound(SoundID.Item29, player.Center);
                    
                    int numberOfDusts = 20;
                    float radius = 2;

                    for (int i = 0; i < numberOfDusts; i++) { Dust.NewDustPerfect(Projectile.Center, DustID.TerraBlade, Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numberOfDusts * i)) * radius, 0, default, 1.2f).noGravity = true; }
                }

                if (Projectile.ai[0] >= 60f)
                {
                    Projectile.ai[0] = 60f;
                }
            }
            else
            {
                if (Projectile.ai[0] >= 60f)
                {
                    delay++;

                    if (delay >= 3f && Projectile.ai[1] <= 3f)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.MountedCenter, (Projectile.velocity.SafeNormalize(Vector2.One) * 12f).RotatedByRandom(0.3f), ModContent.ProjectileType<LivingCoreSpearPro2>(), (int)(Projectile.damage * 0.5f),
                            Projectile.knockBack, Projectile.owner);

                        SoundEngine.PlaySound(SoundID.Item71, player.Center);

                        delay = 0f;
                        Projectile.ai[1]++;

                        player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity = 5f;
                    }
                }
            }

            int duration = player.itemAnimationMax;

            float halfDuration = duration * 0.5f;

            float progress;

            if (Projectile.timeLeft < halfDuration) { progress = Projectile.timeLeft / halfDuration; }
            else { progress = (duration - Projectile.timeLeft) / halfDuration; }

            Projectile.velocity = Vector2.Normalize(Projectile.velocity);
            Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * 0f, Projectile.velocity * 90f, progress);

            if (Projectile.timeLeft > duration) { Projectile.timeLeft = duration; }
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];

            SoundEngine.PlaySound(SoundID.Item1, player.Center);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 position = target.Center + Main.rand.NextVector2Circular(1f, 1f) * target.width;

            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), position, Vector2.Zero, ModContent.ProjectileType<LivingCoreSpearDamage>(), 0, 0f, Projectile.owner);

            for (int k = 0; k < 5; k++)
            {
                float speed = Main.rand.NextFloat(0.2f, 2f);
                Dust.NewDustPerfect(position, DustID.TerraBlade, new Vector2(0f, speed), 0, default, 1.2f).noGravity = true;

                speed = Main.rand.NextFloat(0.2f, 2f);
                Dust.NewDustPerfect(position, DustID.TerraBlade, new Vector2(speed, 0f), 0, default, 1.2f).noGravity = true;

                speed = Main.rand.NextFloat(0.2f, 2f);
                Dust.NewDustPerfect(position, DustID.TerraBlade, new Vector2(0f, -speed), 0, default, 1.2f).noGravity = true;

                speed = Main.rand.NextFloat(0.2f, 2f);
                Dust.NewDustPerfect(position, DustID.TerraBlade, new Vector2(-speed, 0f), 0, default, 1.2f).noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture + "Glow2").Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color color = Projectile.GetAlpha(new Color(255, 255, 255));

            SpriteEffects spriteEffects = SpriteEffects.None;

            if (timer >= MathHelper.TwoPi) { timer = 0f; }
            timer += 0.02f;

            for (int i = 0; i < 3; i++) { Main.EntitySpriteDraw(texture, position + Vector2.One.RotatedBy(timer + ((2 * i))) * radius, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0); }

            texture = ModContent.Request<Texture2D>(Texture).Value;
            color = Projectile.GetAlpha(lightColor);

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            texture = ModContent.Request<Texture2D>(Texture + "Glow").Value;
            color = Projectile.GetAlpha(Color.White);

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            return false;
        }
    }

    public class LivingCoreSpearPro2 : ModProjectile
    {
        public override string Texture => "Divergency/Content/Items/Weapons/LivingCore/LivingCoreSpearPro";

        public override void SetStaticDefaults()
        {
            //.setdefault("Living Core Spear");

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.scale = 1f;
            Projectile.Size = new Vector2(72);

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 2;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            Projectile.alpha += 11;

            if (Projectile.alpha >= 255f)
            {
                Projectile.Kill();
            }
        }

         public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 position = target.Center + Main.rand.NextVector2Circular(1f, 1f) * target.width;

            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), position, Vector2.Zero, ModContent.ProjectileType<LivingCoreSpearDamage>(), 0, 0f, Projectile.owner);

            for (int k = 0; k < 5; k++)
            {
                float speed = Main.rand.NextFloat(0.2f, 2f);
                Dust.NewDustPerfect(position, DustID.TerraBlade, new Vector2(0f, speed), 0, default, 1.2f).noGravity = true;

                speed = Main.rand.NextFloat(0.2f, 2f);
                Dust.NewDustPerfect(position, DustID.TerraBlade, new Vector2(speed, 0f), 0, default, 1.2f).noGravity = true;

                speed = Main.rand.NextFloat(0.2f, 2f);
                Dust.NewDustPerfect(position, DustID.TerraBlade, new Vector2(0f, -speed), 0, default, 1.2f).noGravity = true;

                speed = Main.rand.NextFloat(0.2f, 2f);
                Dust.NewDustPerfect(position, DustID.TerraBlade, new Vector2(-speed, 0f), 0, default, 1.2f).noGravity = true;
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

            texture = ModContent.Request<Texture2D>(Texture + "Glow").Value;
            color = Projectile.GetAlpha(Color.White);

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                position = (Projectile.oldPos[k] + new Vector2(texture.Width, texture.Height) / 2f) - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

                Main.EntitySpriteDraw(texture, position, null, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }

            texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Beam").Value;

            frameHeight = texture.Height / Main.projFrames[Projectile.type];
            frameY = frameHeight * Projectile.frame;

            sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            origin = sourceRectangle.Size() / 2f;
            position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            color = Projectile.GetAlpha(new Color(79, 214, 126, 100));

            spriteEffects = SpriteEffects.None;

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation - MathHelper.PiOver4 + MathHelper.PiOver2, origin, new Vector2(1f, 3f), spriteEffects, 0);

            return false;
        }
    }

    public class LivingCoreSpearDamage : ModProjectile
    {
        Color color = new Color(79, 214, 126, 100);

        float radius;

        public override string Texture => "Divergency/Assets/Textures/Empty";

        public override bool ShouldUpdatePosition() => false;

        public override void SetStaticDefaults()
        {
            //.setdefault("Living Core Spear");
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.Size = new Vector2(16);
            Projectile.scale = 1.2f;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            radius += 3f;
            Projectile.scale -= 0.1f;

            if (Projectile.scale < 0f)
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Beam").Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            Main.EntitySpriteDraw(texture, position + new Vector2(0f, radius), sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, position + new Vector2(radius, 0f), sourceRectangle, color, Projectile.rotation + (MathHelper.PiOver2), origin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, position + new Vector2(0f, -radius), sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, position + new Vector2(-radius, 0f), sourceRectangle, color, Projectile.rotation + (MathHelper.PiOver2), origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}