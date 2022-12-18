using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Divergency.Content.Dusts;
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
    public class Shadowbrand : ModItem
    {
        public int attackDirection = 1;
        int setItemTime;

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowbrand");
            Tooltip.SetDefault("Striking enemies increases the blade's power"
                + "\n'As powerful as it is stylish'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.damage = 30;
            Item.knockBack = 6.5f;

            Item.shoot = ProjectileType<ShadowbrandPro>();
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

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustDirect(Item.position, Item.width, Item.height, DustID.GemAmethyst, Main.rand.NextFloat(1f, 3f), Main.rand.NextFloat(-1f, -3f), 0, default, 1.5f);
                dust.noGravity = true;
            }

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
                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(174, 0, 255, 50), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(140, 120, 255, 77), rotation, frameOrigin, scale, SpriteEffects.None, 0);
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
                spriteBatch.Draw(texture, position + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(174, 0, 255, 50), 0f, origin, scale, SpriteEffects.None, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, position + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(140, 120, 255, 77), 0f, origin, scale, SpriteEffects.None, 0);
            }

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

            Projectile.Center = player.Center + direction * 45;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp(2f * SwingDirection, -2f * SwingDirection, EaseFunction.EaseCircularInOut.Ease(1 - (Projectile.timeLeft / maxTimeLeft)));
            Projectile.scale = 1f + (float)Math.Sin(EaseFunction.EaseCircularInOut.Ease(1 - (Projectile.timeLeft / maxTimeLeft)) * MathHelper.Pi) * 0.6f * 0.6f;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            player.heldProj = Projectile.whoAmI;

            oldRotation.Add(Projectile.rotation);

            if (oldRotation.Count > 10) { oldRotation.RemoveAt(0); }

            if (player.GetModPlayer<PlayerCombo>().itemCombo == 3)
            {
                Dust dust = Dust.NewDustPerfect(player.Center + Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(40f, 90f), DustID.GemAmethyst, new Vector2(0f, 3f).RotatedBy(Projectile.rotation) * -SwingDirection, 0, default, 2f);
                dust.noGravity = true;
            }

            if (player.GetModPlayer<PlayerCombo>().itemCombo == 2)
            {
                if (Main.rand.NextBool(5))
                {
                    Dust dust = Dust.NewDustPerfect(player.Center + Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(40f, 90f), DustID.GemAmethyst, new Vector2(0f, 3f).RotatedBy(Projectile.rotation) * -SwingDirection, 0, default, 1.5f);
                    dust.noGravity = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];

            if (player.GetModPlayer<PlayerCombo>().itemCombo < 3)
            {
                player.GetModPlayer<PlayerCombo>().itemCombo += 1;
                SoundEngine.PlaySound(SoundID.AbigailUpgrade, target.Center);

                for (int i = 0; i < 10; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);

                    Dust dust = Dust.NewDustPerfect(player.Center + (speed * 100f), DustID.GemAmethyst, speed * -5f, 0, default, 2f);
                    dust.noGravity = true;
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
            Texture2D texture = Request<Texture2D>(Texture).Value;

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
                    Color color = Color.Lerp(Color.Magenta, Color.Transparent, 0f) * EaseFunction.EaseQuarticOut.Ease(progress) * 0.1f;

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
}