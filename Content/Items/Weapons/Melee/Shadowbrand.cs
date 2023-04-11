using Divergency.Content.Particles;
using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Divergency.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.Melee
{
    public class Shadowbrand : ModItem
    {
        public int attackDirection = 1;
        int setItemTime;

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowbrand");
            Tooltip.SetDefault("Striking enemies increases the blade's power");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.damage = 30;
            Item.knockBack = 5f;

            Item.shoot = ModContent.ProjectileType<ShadowbrandPro>();
            Item.shootSpeed = 1f;

            Item.width = Item.height = 82;
            Item.scale = 1f;

            Item.useTime = Item.useAnimation = 60;
            setItemTime = Item.useTime;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useTurn = false;

            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
        }

        public override void HoldItem(Player player)
        {
            if (player == Main.LocalPlayer)
            {
                if (player.ItemAnimationActive) { player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation - MathHelper.PiOver2 * player.direction); }
                else { player.SetCompositeArmFront(false, default, default); }

                Item.useTime = Item.useAnimation = setItemTime - (player.GetModPlayer<PlayerCombo>().itemCombo * 10);
            }
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (player.GetModPlayer<PlayerCombo>().itemCombo == 3)
            {
                Item.damage = 45;
            }
            else
            {
                Item.damage = 30;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            attackDirection = -attackDirection;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, attackDirection, 0f);
            Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<ShadowbrandArc>(), (int)(damage * 0.85f), knockback, player.whoAmI);

            if (player.GetModPlayer<PlayerCombo>().itemCombo == 3)
            {
                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/SwingStyleInflamed")
                {
                    Volume = 100f,
                    MaxInstances = -1,
                    Pitch = attackDirection == -1 ? 0f : 0.3f
                }, player.Center);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/SwingStyleLight")
                {
                    MaxInstances = -1,
                    Pitch = attackDirection == -1 ? 0f : 0.3f
                }, player.Center);
            }

            return false;
        }

        float timer;

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/Items/Weapons/Melee/ShadowbrandGlow").Value;
            Rectangle frame;

            if (Main.itemAnimations[Item.type] != null) { frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]); }
            else { frame = texture.Frame(); }

            Vector2 origin = frame.Size() / 2f;
            Vector2 offset = new Vector2(Item.width / 2 - origin.X, Item.height - frame.Height);
            Vector2 position = Item.position - Main.screenPosition + origin + offset;

            if (timer >= MathHelper.TwoPi) { timer = 0f; }
            timer += 0.02f;

            for (int i = 0; i < 3; i++) { spriteBatch.Draw(texture, position + Vector2.One.RotatedBy(timer + ((2 * i))) * 2f, frame, new Color(174, 0, 255, 100), rotation, origin, scale, SpriteEffects.None, 0f); }

            return true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/Items/Weapons/Melee/ShadowbrandGlow").Value;

            if (timer >= MathHelper.TwoPi) { timer = 0f; }
            timer += 0.01f;

            for (int i = 0; i < 3; i++) { spriteBatch.Draw(texture, position + Vector2.One.RotatedBy(timer + ((2 * i))), frame, new Color(174, 0, 255, 100), 0f, origin, scale, SpriteEffects.None, 0f); }

            return true;
        }
    }

    public class ShadowbrandPro : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowbrand");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.scale = 3f;
            Projectile.Size = new Vector2(32);

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ownerHitCheck = true;
        }

        List<float> oldRotation = new List<float>();
        Vector2 direction;
        bool initialize = true;
        float maxTimeLeft;
        float SwingDirection => Projectile.ai[0] * Math.Sign(direction.X);

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (initialize)
            {
                float attackSpeed = player.GetTotalAttackSpeed(DamageClass.Melee) - 1f;

                Projectile.timeLeft = (int)(player.HeldItem.useAnimation * (1f - attackSpeed));
                maxTimeLeft = Projectile.timeLeft;

                direction = Projectile.velocity;
                direction.Normalize();
                Projectile.rotation = Utils.ToRotation(direction);
                Projectile.netUpdate = true;

                initialize = false;
            }

            Projectile.frame = player.GetModPlayer<PlayerCombo>().itemCombo;

            player.itemRotation = Projectile.rotation + MathHelper.PiOver2;

            Projectile.Center = player.Center + direction * 45;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp(2f * SwingDirection, -2f * SwingDirection, EaseFunction.EaseCircularInOut.Ease(1 - (Projectile.timeLeft / maxTimeLeft)));
            Projectile.scale = 1f + (float)Math.Sin(EaseFunction.EaseCircularInOut.Ease(1 - (Projectile.timeLeft / maxTimeLeft)) * MathHelper.Pi) * 0.6f * 0.6f;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            player.heldProj = Projectile.whoAmI;

            oldRotation.Add(Projectile.rotation);

            if (oldRotation.Count > 10) { oldRotation.RemoveAt(0); }

            if (player.GetModPlayer<PlayerCombo>().itemCombo == 3)
            {
                Dust.NewDustPerfect(player.Center + Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(40f, 90f), DustID.GemAmethyst, new Vector2(0f, 3f).RotatedBy(Projectile.rotation) * -SwingDirection, 0, default, 2f).noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];

            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center + (Main.rand.NextVector2Circular(1f, 1f) * target.width), Vector2.Zero, ModContent.ProjectileType<ShadowbrandFlash>(), 0, 0f, Projectile.owner);

            if (player.GetModPlayer<PlayerCombo>().itemCombo < 3)
            {
                player.GetModPlayer<PlayerCombo>().itemCombo += 1;
                SoundEngine.PlaySound(SoundID.AbigailUpgrade, target.Center);

                for (int i = 0; i < 10; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                    
                    Dust.NewDustPerfect(player.Center + (speed * 100f), DustID.GemAmethyst, speed * -5f, 0, default, 1f).noGravity = true;
                }
            }

            player.GetModPlayer<PlayerCombo>().itemComboReset = 300;

            if (player.GetModPlayer<PlayerCombo>().itemCombo == 3)
            {
                target.AddBuff(BuffID.ShadowFlame, 300);
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, target.Center);
                player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 2;
            }
            else
            {
                if (Main.rand.NextBool(4)) { target.AddBuff(BuffID.ShadowFlame, 180); }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame, 0, 0);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 drawPosition = player.Center + Projectile.rotation.ToRotationVector2() * 60f - Main.screenPosition;

            SpriteEffects drawFlipped = player.direction == -1 ? SpriteEffects.FlipHorizontally : 0;

            float rotation = Projectile.rotation + MathHelper.PiOver4 + (player.direction == -1 ? MathHelper.PiOver2 : 0f);

            if (player.GetModPlayer<PlayerCombo>().itemCombo == 3)
            {
                for (int k = 10; k > 0; k--)
                {
                    float progress = 1 - (float)(((float)(10 - k) / (float)10));
                    Color color = Color.Lerp(new Color(174, 0, 255), Color.Transparent, 0f) * EaseFunction.EaseQuarticOut.Ease(progress) * 0.1f;

                    if (Projectile.timeLeft < 20) { color = Color.Lerp(color, Color.Transparent, 1f - (Projectile.timeLeft / 10f) * k); }

                    color.A = 0;

                    if (k > 0 && k < oldRotation.Count)
                    {
                        Main.spriteBatch.Draw(texture, drawPosition, sourceRectangle, color, oldRotation[k] + MathHelper.PiOver4 + (player.direction == -1 ? MathHelper.PiOver2 : 0f), origin, Projectile.scale * 1.2f, drawFlipped,
                        0f);
                    }
                }
            }

            Main.spriteBatch.Draw(texture, drawPosition, sourceRectangle, lightColor, rotation, origin, Projectile.scale, drawFlipped, 0f);

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player player = Main.player[Projectile.owner];
            float collisionPoint = 0f;

            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.Center, player.Center + ((82 * Projectile.scale) * Projectile.rotation.ToRotationVector2()), 20, ref collisionPoint)) { return true; }

            return false;
        }
    }

    public class ShadowbrandArc : ModProjectile
    {
        Color[] colorList = { new Color(191, 74, 165, 100), new Color(118, 62, 130, 100), new Color(255, 255, 255, 100) };

        float radius;

        public override string Texture => "Divergency/Assets/Textures/Empty";

        public override bool ShouldUpdatePosition() => false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowbrand");
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.Size = new Vector2(170f);
            Projectile.scale = 1.5f;
            Projectile.alpha = 255;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.Center = player.MountedCenter;

            if (player.direction > 0)
            {
                Projectile.rotation = player.itemRotation - MathHelper.PiOver2;

                Dust.NewDustPerfect(Projectile.Center + ((player.itemRotation - MathHelper.PiOver2).ToRotationVector2() * Main.rand.NextFloat(30f, radius)), DustID.GemAmethyst, Vector2.One.RotatedBy(player.itemRotation), 0, default, 1f).noGravity = true;
            }
            else
            {
                Projectile.rotation = player.itemRotation + MathHelper.PiOver2;

                Dust.NewDustPerfect(Projectile.Center + ((player.itemRotation - MathHelper.PiOver2).ToRotationVector2() * Main.rand.NextFloat(30f, radius)), DustID.GemAmethyst, new Vector2(-1f, 1f).RotatedBy(player.itemRotation), 0, default, 
                    1f).noGravity = true;
            }

            Projectile.spriteDirection = player.direction;
            Projectile.scale = 1.5f + MathHelper.Lerp(1.5f, 0f, (float)(player.itemAnimation) / player.itemAnimationMax);

            Projectile.Size += new Vector2(10f);

            radius = 80f * Projectile.scale;

            if (player.itemAnimation > player.itemAnimationMax / 2f)
            {
                Projectile.alpha = (int)MathHelper.Lerp(0f, 255f, (float)(player.itemAnimation) / player.itemAnimationMax);
            }
            else
            {
                Projectile.alpha = (int)MathHelper.Lerp(255, 0f, (float)(player.itemAnimation) / player.itemAnimationMax);
            }

            if (player.ItemAnimationEndingOrEnded) { Projectile.Kill(); }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Vector2 position = target.Center + Main.rand.NextVector2Circular(1f, 1f) * target.width;

            //Projectile.NewProjectile(Projectile.GetSource_OnHit(target), position, Vector2.Zero, ModContent.ProjectileType<CactusEdgeDamage>(), 0, 0f, Projectile.owner);

            /*for (int k = 0; k < 5; k++)
            {
                float speed = Main.rand.NextFloat(0.2f, 2f);

                Dust.NewDustPerfect(position, DustID.TerraBlade, new Vector2(speed), 0, default, 1.2f).noGravity = true;

                speed = Main.rand.NextFloat(0.2f, 2f);
                Dust.NewDustPerfect(position, DustID.TerraBlade, new Vector2(-speed), 0, default, 1.2f).noGravity = true;

                speed = Main.rand.NextFloat(0.2f, 2f);
                Dust.NewDustPerfect(position, DustID.TerraBlade, new Vector2(-speed, speed), 0, default, 1.2f).noGravity = true;

                speed = Main.rand.NextFloat(0.2f, 2f);
                Dust.NewDustPerfect(position, DustID.TerraBlade, new Vector2(speed, -speed), 0, default, 1.2f).noGravity = true;
            }*/
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/MeleeArc0").Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            SpriteEffects spriteEffects = SpriteEffects.None;

            if (player.direction < 0)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }

            Main.EntitySpriteDraw(texture, position, sourceRectangle, colorList[1] * Projectile.Opacity, Projectile.rotation + 0.2f, origin, Projectile.scale, spriteEffects, 0); //Front
            Main.EntitySpriteDraw(texture, position, sourceRectangle, colorList[1] * Projectile.Opacity, Projectile.rotation - 0.2f, origin, Projectile.scale, spriteEffects, 0); //Back
            Main.EntitySpriteDraw(texture, position, sourceRectangle, colorList[0] * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0); //Top

            texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/MeleeArc3").Value;

            Main.EntitySpriteDraw(texture, position, sourceRectangle, colorList[2] * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, position, sourceRectangle, colorList[2] * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale * 0.75f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, position, sourceRectangle, colorList[2] * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale * 0.5f, spriteEffects, 0);

            texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Star").Value;

            frameHeight = texture.Height / Main.projFrames[Projectile.type];
            frameY = frameHeight * Projectile.frame;

            sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            origin = sourceRectangle.Size() / 2f;

            if (player.direction < 0)
            {
                position = Projectile.Center + ((player.itemRotation - MathHelper.PiOver4) - MathHelper.PiOver2).ToRotationVector2() * radius - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            }
            else
            {
                position = Projectile.Center + (player.itemRotation - MathHelper.PiOver4).ToRotationVector2() * radius - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            }

            Main.EntitySpriteDraw(texture, position, sourceRectangle, colorList[2] * Projectile.Opacity, MathHelper.PiOver4, origin, 3f, spriteEffects, 0);

            return false;
        }
    }

    public class ShadowbrandFlash : ModProjectile
    {
        public override string Texture => "Divergency/Assets/Textures/FadedLine";

        public override bool ShouldUpdatePosition() => false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowbrand");
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = false;
            Projectile.hostile = false;

            Projectile.scale = 1f;
            Projectile.Size = new Vector2(16);
            Projectile.alpha = 0;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;

            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        bool initilize = true;

        public override void AI()
        {
            if (initilize)
            {
                Projectile.rotation = Main.rand.NextFloat(0f, 360f);
                initilize = false;
            }

            Projectile.scale *= 0.9f;

            Projectile.alpha += 20;
            if (Projectile.alpha >= 255) { Projectile.Kill(); }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color color = Projectile.GetAlpha(new Color(174, 0, 255, 0));

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}