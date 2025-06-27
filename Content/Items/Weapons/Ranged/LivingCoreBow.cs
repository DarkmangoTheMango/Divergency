using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Divergency.Content.Projectiles.Ranged;

namespace Divergency.Content.Items.Weapons.Ranged
{
    public class LivingCoreBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            
        }

        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 0;
            Item.height = 0;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.value = 10000;
            Item.noMelee = true;
            Item.rare = ItemRarityID.Green;
            Item.crit = 10;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<LivingCoreArrow>();
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Arrow;
            // Item.scale = 1.4f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<LivingCoreArrow>(), damage, knockback, player.whoAmI);

            return false; // base.Shoot(player, source, position, velocity, ModContent.ProjectileType<Projectiles.Weapons.Ranged.LivingCoreArrow>(), damage, knockback);
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3, 0);
        }
    }
}

