using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Divergency.Content.Items.Weapons.Melee
{
    public class Sacrony : ModItem
    {
        public int attackDirection = 1;
        public int AttackCounter = 1;

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("(Work In Progress) Sacrony");
            Tooltip.SetDefault("");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.damage = 30;
            Item.knockBack = 4f;

            Item.shoot = ProjectileType<SacronyPro>();
            Item.shootSpeed = 2f;

            Item.width = Item.height = 96;
            Item.scale = 1f;

            Item.useTime = Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useTurn = false;

            Item.value = Item.sellPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Green;
        }

        public override void HoldItem(Player player)
        {
            if (player == Main.LocalPlayer)
            {
                if (player.ItemAnimationActive) { player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation - MathHelper.PiOver2 * player.direction); }
                else { player.SetCompositeArmFront(false, default, default); }
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.GetModPlayer<ComboSystem>().itemCombo <= 2) { attackDirection = -attackDirection; }

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, attackDirection, 0f);
            
            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/SwingStyleSacrony")
            {
                MaxInstances = -1,
            }, player.Center);

            if (player.GetModPlayer<ComboSystem>().itemCombo >= 5) { player.GetModPlayer<ComboSystem>().itemCombo = 0; }

            player.GetModPlayer<ComboSystem>().itemCombo++;
            player.GetModPlayer<ComboSystem>().itemComboReset = 480;

            return false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.GetModPlayer<ComboSystem>().itemCombo >= 3)
            {
                velocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
            }
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Rectangle frame;

            if (Main.itemAnimations[Item.type] != null) { frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]); }
            else { frame = texture.Frame(); }

            Vector2 frameOrigin = frame.Size() / 2f;
            Vector2 offset = new Vector2(Item.width / 2 - frameOrigin.X, Item.height - frame.Height);
            Vector2 drawPos = Item.position - Main.screenPosition + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Item.timeSinceItemSpawned / 240f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f) { time = 2f - time; }

            time = time * 0.5f + 0.5f;

            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(235, 52, 198, 50), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(235, 52, 198, 77), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Item.timeSinceItemSpawned / 240f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f) { time = 2f - time; }

            time = time * 0.5f + 0.5f;

            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, position + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(235, 52, 198, 50), 0f, origin, scale, SpriteEffects.None, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, position + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(235, 52, 198, 77), 0f, origin, scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }

    public class SacronyPro : ModProjectile
    {
        public override string Texture => "Divergency/Content/Items/Weapons/Melee/Sacrony";

        public override bool ShouldUpdatePosition() => false;

        public override void SetStaticDefaults() { DisplayName.SetDefault("Sacrony"); }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.scale = 1f;
            Projectile.Size = new Vector2(180);

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

        public float SwingDirection => Projectile.ai[0] * Math.Sign(direction.X);

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.GetModPlayer<ComboSystem>().itemCombo <= 2)
            {
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            }
            else
            {
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() - MathHelper.PiOver2);
            }

            if (initialize)
            {
                float attackSpeed = player.GetTotalAttackSpeed(DamageClass.Melee) - 1f;
                Projectile.timeLeft = (int)(player.HeldItem.useAnimation * (1f - attackSpeed));
                maxTimeLeft = Projectile.timeLeft;
                direction = Projectile.velocity;
                direction.Normalize();

                if (player.GetModPlayer<ComboSystem>().itemCombo <= 2)
                {
                    Projectile.rotation = Utils.ToRotation(direction);
                    Projectile.netUpdate = true;
                }

                initialize = false;
            }

            if (player.GetModPlayer<ComboSystem>().itemCombo <= 2)
            {
                Projectile.Center = player.Center + direction * 45;

                if (player.GetModPlayer<ComboSystem>().itemCombo == 3)
                {
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp(2f * SwingDirection, -8.3f * SwingDirection, EaseFunction.EaseCircularInOut.Ease(1 - (Projectile.timeLeft / maxTimeLeft)));
                }
                else
                {
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp(2f * SwingDirection, -2f * SwingDirection, EaseFunction.EaseCircularInOut.Ease(1 - (Projectile.timeLeft / maxTimeLeft)));
                }

                Projectile.scale = 0.8f + (float)Math.Sin(EaseFunction.EaseCircularInOut.Ease(1 - (Projectile.timeLeft / maxTimeLeft)) * MathHelper.Pi) * 0.6f * 0.6f;

                player.heldProj = Projectile.whoAmI;

                oldRotation.Add(Projectile.rotation);

                if (oldRotation.Count > 10) { oldRotation.RemoveAt(0); }

                Dust dust = Dust.NewDustPerfect(player.Center + Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(30f, 110f), DustID.PortalBolt, new Vector2(0f, 3f).RotatedBy(Projectile.rotation) * -SwingDirection, 0, Color.BlueViolet, 2f);
                dust.noGravity = true;
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

                if (Projectile.timeLeft < maxTimeLeft / 2)
                {
                    Projectile.Center = player.MountedCenter + Vector2.Lerp(Projectile.velocity * 80, Projectile.velocity * 20, EaseFunction.EaseCircularInOut.Ease(1 - (Projectile.timeLeft / maxTimeLeft)));
                }
                else
                {
                    Projectile.Center = player.MountedCenter + Vector2.Lerp(Projectile.velocity * 20, Projectile.velocity * 80, EaseFunction.EaseCircularInOut.Ease(1 - (Projectile.timeLeft / maxTimeLeft)));
                }

                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10f, 10f), DustID.PortalBolt, Projectile.velocity * 2f, 0, Color.BlueViolet, 2f);
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            if (player.GetModPlayer<ComboSystem>().itemCombo <= 2)
            {
                Texture2D texture = Request<Texture2D>(Texture).Value;

                Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);
                Vector2 origin = sourceRectangle.Size() / 2f;
                Vector2 drawPosition = player.Center + Projectile.rotation.ToRotationVector2() * 60f - Main.screenPosition;

                SpriteEffects drawFlipped = player.direction == -1 ? SpriteEffects.FlipHorizontally : 0;

                float rotation = Projectile.rotation + MathHelper.PiOver4 + (player.direction == -1 ? MathHelper.PiOver2 : 0f);

                for (int k = 10; k > 0; k--)
                {
                    float progress = 1 - (float)(((float)(10 - k) / (float)10));
                    Color color = Color.Lerp(Color.Magenta, Color.Transparent, 0f) * EaseFunction.EaseQuarticOut.Ease(progress) * 0.1f;

                    if (Projectile.timeLeft < 20) { color = Color.Lerp(color, Color.Transparent, 1f - (Projectile.timeLeft / 10f) * k); }

                    color.A = 0;

                    if (k > 0 && k < oldRotation.Count)
                    {
                        Main.spriteBatch.Draw(texture, drawPosition, sourceRectangle, color, oldRotation[k] + MathHelper.PiOver4 + (player.direction == -1 ? MathHelper.PiOver2 : 0f), origin, Projectile.scale, drawFlipped,
                        0f);
                    }
                }

                Main.spriteBatch.Draw(texture, drawPosition, sourceRectangle, lightColor, rotation, origin, Projectile.scale, drawFlipped, 0f);

                return false;
            }
            else
            {
                SpriteEffects spriteEffects = SpriteEffects.None;
                Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
                Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                Vector2 origin = sourceRectangle.Size() / 2f;

                Color drawColor = Projectile.GetAlpha(lightColor);
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), sourceRectangle, drawColor, Projectile.rotation, origin, 1, spriteEffects, 0f);

                return false;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player player = Main.player[Projectile.owner];

            if (player.GetModPlayer<ComboSystem>().itemCombo <= 2)
            {
                float collisionPoint = 0f;

                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.Center, player.Center + ((114 * Projectile.scale) * Projectile.rotation.ToRotationVector2()), 20, ref collisionPoint)) { return true; }

                return false;
            }
            else
            {
                return base.Colliding(projHitbox, targetHitbox);
            }
        }
    }
}