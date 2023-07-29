using Divergency.Content.Dusts;
using Divergency.Content.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.Melee
{
    public class LivingWoodGuitar : ModItem
    {
        public float charge = 0;

        public override void SetStaticDefaults()
        {
            // //.setdefault("Doorlauncher"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            ////.setdefault("'It sounds horrible'");
            //.setdefault("Nature's Serenade");
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (charge < 3)
            {
                Item.UseSound = SoundID.Item133;
                Item.damage = 15;
            }
            else
            {
                Item.UseSound = SoundID.Item136;
                Item.damage = 25;
            }
        }

        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.DamageType = DamageClass.Melee;
            Item.width = 1;
            Item.height = 1;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Guitar;
            Item.knockBack = 1;
            Item.value = 10000;
            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<LivingResonance>();
            Item.UseSound = SoundID.Item133;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.Green;
        }

        
        
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (charge < 5)
            {
                Item.UseSound = SoundID.Item133;

                for (int i = 0; i < 10; i++)
                {
                    Dust dust = Dust.NewDustPerfect(position, ModContent.DustType<NoteDust>(), Main.rand.NextVector2Circular(1f, 1f) * 8, 0, default, 2f);
                    dust.noGravity = true;
                }
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 1, 0);
            }
            else
            {
                Item.UseSound = SoundID.Item136;

                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<LivingResonance2>(), damage, knockback, player.whoAmI, 1, 0);
                for (int i = 0; i < 20; i++)
                {
                    Dust dust = Dust.NewDustPerfect(position, ModContent.DustType<NoteDust>(), Main.rand.NextVector2Circular(1f, 1f) * 11, 0, default, 2.6f);
                    dust.noGravity = true;
                }
                charge = 0;
            }
            charge++;


            return false; // return true to allow tmodloader to call Projectile.NewProjectile as normal
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DirtBlock, 10);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}