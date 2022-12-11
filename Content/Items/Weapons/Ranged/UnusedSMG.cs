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
    public class UnusedSMG : ModItem
    {
        public override Vector2? HoldoutOffset() => new Vector2(0f, 7f);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Generic SMG");
            Tooltip.SetDefault("Inflicts Shred");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 4;
            Item.crit = 3;
            Item.knockBack = 3.5f;
            Item.noMelee = true;

            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ModContent.ProjectileType<ShortBullet>();
            Item.shootSpeed = 10f;

            Item.width = Item.height = 16;
            Item.scale = 1.4f;

            Item.useTime = Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item40;
            Item.autoReuse = true;
            Item.useTurn = false;

            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
        }

        float overheat = 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (type == ProjectileID.Bullet) { type = ModContent.ProjectileType<ShortBullet>(); }

            Vector2 offset = Vector2.Normalize(velocity) * 52f;

            if (Collision.CanHit(position, 0, 0, position + offset, 0, 0)) { position += offset; }

            for (int k = 0; k < 2; k++)
            {
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(30));
                float scale = 1f - (Main.rand.NextFloat() * 0.9f);

                Dust.NewDustPerfect(position, DustID.Torch, (perturbedSpeed * scale) * 0.5f, 0, default, 2f).noGravity = true;
            }

            Dust.NewDustPerfect(position, ModContent.DustType<Smoke>(), velocity * 0.5f, 0, new Color(255, 217, 0), 0.5f);

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

            return false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(5));
        }
    }
}