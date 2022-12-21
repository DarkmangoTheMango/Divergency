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

namespace Divergency.Content.Items.Weapons.Melee
{
    public class Rootbuster : ModItem
    {
        public int attackDirection = 1;
        int setItemTime;

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("(Rework Pending) Rootbuster");
            Tooltip.SetDefault("");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.damage = 30;
            Item.knockBack = 5f;

            Item.shoot = ModContent.ProjectileType<RootbusterPro>();
            Item.shootSpeed = 10f;

            Item.Size = new Vector2(44, 42);
            Item.scale = 1f;

            Item.useTime = Item.useAnimation = 50;
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
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            attackDirection = -attackDirection;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, attackDirection, 0f);

            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/SwingStyleHammer")
            {
                MaxInstances = -1,
                Pitch = attackDirection == -1 ? 0f : 0.3f
            }, player.Center);

            return false;
        }
    }

    public class RootbusterPro : ModProjectile
    {
        public override string Texture => "Divergency/Content/Items/Weapons/Melee/Rootbuster";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rootbuster");
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

        int pauseTimer;

        int oldTimeleft;

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

            if (--pauseTimer > 0) { Projectile.timeLeft = oldTimeleft; }

            Projectile.frame = player.GetModPlayer<PlayerCombo>().itemCombo;

            if (pauseTimer <= 0) { Projectile.Center = player.Center + direction * 45; }
            if (pauseTimer <= 0) { Projectile.scale = 2f + (float)Math.Sin(EaseFunction.EaseCircularInOut.Ease(1 - (Projectile.timeLeft / maxTimeLeft)) * MathHelper.Pi) * 0.6f * 0.6f; }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp(3f * SwingDirection, -3f * SwingDirection, EaseFunction.EaseCircularInOut.Ease(1 - (Projectile.timeLeft / maxTimeLeft)));

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            player.heldProj = Projectile.whoAmI;

            oldRotation.Add(Projectile.rotation);

            if (oldRotation.Count > 10) { oldRotation.RemoveAt(0); }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];

            player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 8;

            oldTimeleft = Projectile.timeLeft;
            pauseTimer = 8;
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

            Main.spriteBatch.Draw(texture, drawPosition, sourceRectangle, lightColor, rotation, origin, Projectile.scale, drawFlipped, 0f);

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player player = Main.player[Projectile.owner];
            float collisionPoint = 0f;

            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.Center, player.Center + ((44 * Projectile.scale) * Projectile.rotation.ToRotationVector2()), 20, ref collisionPoint)) { return true; }

            return false;
        }
    }

    public class RootbusterNPC : GlobalNPC
    {

    }
}