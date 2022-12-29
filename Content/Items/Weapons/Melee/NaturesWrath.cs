using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Divergency.Content.Dusts;
using Divergency.Content.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    public class NaturesWrath : ModItem
    {
        public override bool AltFunctionUse(Player player) => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nature's Wrath");
            Tooltip.SetDefault("<right> to throw a branch \nhitting branches with your fists amplifies your damage");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.damage = 8;
            Item.knockBack = 3.25f;

            Item.shoot = ModContent.ProjectileType<NaturesWrathPro>();
            Item.shootSpeed = 5f;

            Item.Size = new Vector2(26, 28);
            Item.scale = 1f;

            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useTurn = false;

            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
        }

        public override bool CanUseItem(Player Player)
        {
            if (Player.altFunctionUse == 2)
            {
                Item.shoot = ModContent.ProjectileType<LivingBranch>();
                Item.shootSpeed = 10f;
                Item.useAnimation = 60;

                Item.useStyle = ItemUseStyleID.Swing;
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<NaturesWrathPro>();
                Item.shootSpeed = 5f;

                Item.useStyle = ItemUseStyleID.Shoot;
            }

            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<LivingBranch>(), damage / 2, knockback, player.whoAmI);
            }
            else
            {
                int offsetFactor = Main.rand.Next(5, 10);
                int offset = Main.rand.Next(-10, 10);
                float rotation = position.DirectionTo(Main.MouseWorld).ToRotation();
                Vector2 offsetV = new Vector2(Main.rand.Next(-offsetFactor, offsetFactor), offset);

                Projectile.NewProjectileDirect(source, position + offsetV.RotatedBy(rotation), velocity, ModContent.ProjectileType<NaturesWrathPro>(), damage, knockback, player.whoAmI, 0f);
            }

            return false;
        }
    }

    public class NaturesWrathPro : ModProjectile
    {
        public Vector2 directionVector = Vector2.Zero;

        public override void SetStaticDefaults() { DisplayName.SetDefault("Nature's Wrath"); }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.scale = 1f;
            Projectile.Size = new Vector2(16);

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 50;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ownerHitCheck = true;
        }

        bool initilize = true;

        int maxTimeLeft;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (initilize)
            {
                maxTimeLeft = Projectile.timeLeft;

                initilize = false;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.timeLeft == maxTimeLeft / 2)
            {
                Projectile.ai[1] = 1;
            }

            if (Projectile.ai[1] == 1)
            {
                Projectile.damage = 0;
                Projectile.alpha += 10;
                Projectile.velocity *= 0.9f;
            }

            if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
            }

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];

                if (projectile.type == ModContent.ProjectileType<LivingBranch>() && projectile.active && Projectile.Hitbox.Intersects(projectile.Hitbox) && projectile.friendly)
                {
                    projectile.Kill();
                    player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 5;

                    SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            SoundEngine.PlaySound(SoundID.Grass, Projectile.position);

            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Leaf>(), Main.rand.NextVector2Circular(1f, 1f) * 10, 0, default, 1f);
                dust.noGravity = true;
            }

            Projectile.ai[1] = 1;
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

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}