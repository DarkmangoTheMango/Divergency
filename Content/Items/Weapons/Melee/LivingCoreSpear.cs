using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Divergency.Content.Projectiles.Magic;
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

namespace Divergency.Content.Items.Weapons.Melee
{
    public class LivingCoreSpear : ModItem
    {
        public int attackDirection = 1;
        public int AttackCounter = 1;

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("(Work In Progress) Living Core Spear");
            Tooltip.SetDefault("");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.damage = 30;
            Item.knockBack = 4f;

            Item.shoot = ModContent.ProjectileType<LivingCoreSpearPro>();
            Item.shootSpeed = 15f;

            Item.width = Item.height = 96;
            Item.scale = 1f;

            Item.useTime = Item.useAnimation = 45;
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
            attackDirection = -attackDirection;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, attackDirection, 0f);

            if (player.GetModPlayer<PlayerCombo>().itemCombo >= 4)
            {
                player.GetModPlayer<PlayerCombo>().itemCombo = 0;
            }

            player.GetModPlayer<PlayerCombo>().itemCombo++;
            player.GetModPlayer<PlayerCombo>().itemComboReset = 480;

            return false;
        }
    }

    public class LivingCoreSpearPro : ModProjectile
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Living Core Spear");

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.scale = 2f;
            Projectile.Size = new Vector2(90);

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        List<float> oldRotation = new List<float>();

        Vector2 direction;

        bool initialize = true;

        float maxTimeLeft;

        public float SwingDirection => Projectile.ai[0] * Math.Sign(direction.X);

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            if (player.GetModPlayer<PlayerCombo>().itemCombo == 4)
            {
                if (initialize)
                {
                    float attackSpeed = (player.GetTotalAttackSpeed(DamageClass.Melee) - 1f);
                    Projectile.timeLeft = (int)(player.HeldItem.useAnimation * (1f - attackSpeed));

                    maxTimeLeft = Projectile.timeLeft;
                    direction = Projectile.velocity;
                    direction.Normalize();

                    SoundEngine.PlaySound(SoundID.Item71, player.Center);

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<LivingLeaf>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 0f, 1);

                    initialize = false;
                }

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

                if (Projectile.timeLeft < maxTimeLeft / 2)
                {
                    Projectile.Center = player.MountedCenter + Vector2.Lerp(Projectile.velocity * 40, Projectile.velocity * 1, EaseFunction.EaseCircularOut.Ease(1 - (Projectile.timeLeft / maxTimeLeft)));
                }
                else
                {
                    Projectile.Center = player.MountedCenter + Vector2.Lerp(Projectile.velocity * 1, Projectile.velocity * 40, EaseFunction.EaseCircularIn.Ease(1 - (Projectile.timeLeft / maxTimeLeft)));
                }
            }
            else
            {
                if (initialize)
                {
                    float attackSpeed;

                    if (player.GetModPlayer<PlayerCombo>().itemCombo == 3)
                    {
                        attackSpeed = (player.GetTotalAttackSpeed(DamageClass.Melee) - 1f) * 1.2f;
                        Projectile.timeLeft = (int)(player.HeldItem.useAnimation * 1.2f * (1f - attackSpeed));

                        Projectile.localNPCHitCooldown = 0;
                    }
                    else
                    {
                        attackSpeed = player.GetTotalAttackSpeed(DamageClass.Melee) - 1f;
                        Projectile.timeLeft = (int)(player.HeldItem.useAnimation * (1f - attackSpeed));

                        Projectile.localNPCHitCooldown = -1;
                    }

                    maxTimeLeft = Projectile.timeLeft;
                    direction = Projectile.velocity;
                    direction.Normalize();
                    Projectile.rotation = Utils.ToRotation(direction);
                    Projectile.netUpdate = true;

                    if (player.GetModPlayer<PlayerCombo>().itemCombo == 3)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/LivingCoreSpearSpin"), player.Center);
                    }
                    else
                    {
                        SoundEngine.PlaySound(SoundID.Item71, player.Center);
                    }

                    initialize = false;
                }

                Projectile.Center = player.Center + direction * 45;

                if (player.GetModPlayer<PlayerCombo>().itemCombo == 3)
                {
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp(-MathHelper.Pi * 5f * SwingDirection, MathHelper.Pi * 5f * SwingDirection, EaseFunction.EaseCircularOut.Ease(1 - (Projectile.timeLeft / maxTimeLeft)));
                }
                else
                {
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp(-MathHelper.Pi * SwingDirection, MathHelper.Pi * SwingDirection, EaseFunction.EaseCircularOut.Ease(1 - (Projectile.timeLeft / maxTimeLeft)));
                }

                Projectile.scale = 1.5f + (float)Math.Sin(EaseFunction.EaseCircularOut.Ease(1 - (Projectile.timeLeft / maxTimeLeft)) * MathHelper.Pi) * 0.6f * 0.6f;

                player.heldProj = Projectile.whoAmI;

                oldRotation.Add(Projectile.rotation);

                if (oldRotation.Count > 10)
                {
                    oldRotation.RemoveAt(0);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            if (player.GetModPlayer<PlayerCombo>().itemCombo < 4)
            {
                Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

                Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame, 0, 0);
                Vector2 origin = sourceRectangle.Size() / 2f;
                Vector2 drawPosition = player.Center + Projectile.rotation.ToRotationVector2() * 45f - Main.screenPosition;

                SpriteEffects drawFlipped = player.direction == -1 ? SpriteEffects.FlipHorizontally : 0;

                float rotation = Projectile.rotation + MathHelper.PiOver4 + (player.direction == -1 ? MathHelper.PiOver2 : 0f);

                for (int k = 10; k > 0; k--)
                {
                    float progress = 1 - (float)(((float)(10 - k) / (float)10));
                    Color color = Color.Lerp(Color.Lime, Color.Transparent, 0f) * EaseFunction.EaseQuarticOut.Ease(progress) * 0.1f;

                    if (Projectile.timeLeft < 20)
                    {
                        color = Color.Lerp(color, Color.Transparent, 1f - (Projectile.timeLeft / 10f) * k);
                    }

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

                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0f);

                return false;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player player = Main.player[Projectile.owner];
            float collisionPoint = 0f;

            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.Center, player.Center + ((96 * Projectile.scale) * Projectile.rotation.ToRotationVector2()), 20, ref collisionPoint)) { return true; }

            return false;
        }
    }
}