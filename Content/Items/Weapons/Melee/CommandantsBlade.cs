using Divergency.Common.Systems3D;
using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Divergency.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.Melee
{
    public class CommandantsBlade : ModItem, I3D
    {
        float smoothInOut(int time, int maxTime)
        {
            float x = (float)time / (float)maxTime;
            return x < 0.5 ? 4 * x * x * x : 1 - MathF.Pow(-2 * x + 2, 3) / 2;
        }

        public Vector3 StartRotation => new Vector3(0f, 0f, 0f);
        public Vector2 ConstOffset => new Vector2(0, 80);
        public Vector2 StartOffset => new Vector2(0, 0);
        public string SwordTexture => "Divergency/Content/Items/Weapons/Melee/CommandantsBlade";

        public AnimKeyframes SwordFrames => new AnimKeyframes(new SwordAnimation[] {
            new SwordAnimation(new Vector3(MathF.PI / 2, 0, 0), 10),
            new SwordAnimation(new Vector3(MathF.PI / 2, 1.2f, 0), 12),
            new SwordAnimation(new Vector3(MathF.PI / 2, 1.2f, MathF.PI*2), 30),
        });

        public int attackDirection = 1;
        public int AttackCounter = 1;

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Commandant's Blade");
            Tooltip.SetDefault($"Inflicts Flesh Wound [i:{ModContent.ItemType<FleshWoundIcon>()}]");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.damage = 35;
            Item.knockBack = 5f;

            Item.shoot = ModContent.ProjectileType<CommandantsBladePro>();
            Item.shootSpeed = 1f;

            Item.width = Item.height = 90;
            Item.scale = 1f;

            Item.useTime = Item.useAnimation = 30;
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

            //Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, attackDirection, 0f);

            Handler3D.Swing(player, source, Item.type, damage, knockback);

            return false;
        }
    }

    public class CommandantsBladePro : ModProjectile
    {
        List<float> oldRotation = new List<float>();

        Vector2 direction;

        bool initialize = true;

        bool halfPoint = true;

        float maxTimeLeft;

        int pauseTimer;

        int maxHits = 5;

        int oldTimeleft;

        public float SwingDirection => Projectile.ai[0] * Math.Sign(direction.X);

        public override string Texture => "Divergency/Content/Items/Weapons/Melee/CommandantsBlade";

        public override bool? CanHitNPC(NPC target) => maxHits >= 1;

        public override void SetStaticDefaults() { DisplayName.SetDefault("Commandant's Blade"); }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.scale = 1f;
            Projectile.Size = Vector2.Zero;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.ownerHitCheck = true;
        }

        public override void AI()
        {
            if (--pauseTimer > 0 && maxHits > 0) { Projectile.timeLeft = oldTimeleft; }

            Player player = Main.player[Projectile.owner];

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            player.ChangeDir(Projectile.velocity.X > 0 ? 1 : -1);

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
            
            Projectile.Center = player.Center + direction * 45;
            Projectile.scale = 1f + (float)Math.Sin(EaseFunction.EaseCircularInOut.Ease(1 - (Projectile.timeLeft / maxTimeLeft)) * MathHelper.Pi) * 0.6f * 0.6f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp(2f * SwingDirection, -2f * SwingDirection, EaseFunction.EaseCircularInOut.Ease(1 - (Projectile.timeLeft / maxTimeLeft)));

            player.heldProj = Projectile.whoAmI;
            oldRotation.Add(Projectile.rotation);

            if (oldRotation.Count > 10) { oldRotation.RemoveAt(0); }

            if (Projectile.timeLeft <= maxTimeLeft / 2f && halfPoint)
            {
                SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/SwingHeavy") with { Pitch = Main.rand.NextFloat(-0.1f, 0.1f) }, player.Center);

                Projectile.rotation = Projectile.velocity.ToRotation();

                halfPoint = false;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];

            player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 2;

            target.AddBuff(ModContent.BuffType<FleshWound>(), 180);

            SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/CommandantsBladeHit") { Pitch = Main.rand.NextFloat(-0.3f, 0.3f) }, player.Center);

            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(target.position, target.width, target.height, DustID.Blood, target.DirectionTo(player.Center).X * -Main.rand.NextFloat(0f, 10f), target.DirectionTo(player.Center).Y * -Main.rand.NextFloat(0f, 10f), 0, default, 2f);
            }

            oldTimeleft = Projectile.timeLeft;
            pauseTimer = 10;
            maxHits--;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);

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

            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.Center, player.Center + ((96f * Projectile.scale) * Projectile.rotation.ToRotationVector2()), 20, ref collisionPoint)) { return true; }

            return false;
        }
    }
}