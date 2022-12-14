using Divergency.Content.Dusts;
using Divergency.Content.Projectiles.Ranged;
using Divergency.Common.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Divergency.Common.Players;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent;
using Terraria.UI;

namespace Divergency.Content.Items.Weapons.Ranged
{
    public class SapphireShotgun : ModItem
    {
        public override Vector2? HoldoutOffset() => new Vector2(0f, 0f);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("(Work In Progress) Sapphire Shotgun");
            Tooltip.SetDefault("");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 8;
            Item.crit = 3;
            Item.knockBack = 5f;
            Item.noMelee = true;

            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ModContent.ProjectileType<SapphireBullet>();
            Item.shootSpeed = 20f;

            Item.Size = new Vector2(44, 18);
            Item.scale = 1f;

            Item.useTime = Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = new SoundStyle("Divergency/Assets/Sounds/Items/SapphireShotgun");
            Item.autoReuse = false;
            Item.useTurn = false;

            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (type == ProjectileID.Bullet) { type = Item.shoot; }

            Vector2 offset = Vector2.Normalize(velocity) * 52f;

            if (Collision.CanHit(position, 0, 0, position + offset, 0, 0)) { position += offset; }

            for (int k = 0; k < 2 + Main.rand.Next(2); k++)
            {
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                newVelocity *= 1f - Main.rand.NextFloat(0.5f);

                Projectile.NewProjectile(source, position, newVelocity, type, damage, knockback, player.whoAmI);
            }

            for (int k = 0; k < 5; k++)
            {
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(30));
                float scale = 1f - (Main.rand.NextFloat() * 0.3f);

                Dust dust = Dust.NewDustPerfect(position, DustID.GemSapphire, (perturbedSpeed * scale) * 0.5f, 0, default, 1f);
                dust.noGravity = true;

                Dust.NewDustPerfect(position, ModContent.DustType<Smoke>(), (perturbedSpeed * scale) * 0.5f, 0, new Color(0, 94, 255), 1f);
            }

            player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity += 5;

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Sapphire, 10)
                .AddIngredient(ItemID.Boomstick)
                .AddIngredient(ItemID.IllegalGunParts)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}