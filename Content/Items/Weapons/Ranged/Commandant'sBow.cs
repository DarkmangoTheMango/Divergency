using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.Ranged
{
    public class CommandantsBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            //.setdefault("Commandant's Bow"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            ////.setdefault("'Heavy. Too heavy.'");
        }

        public int wombocombo = 0;

        public override void SetDefaults()
        {
            Item.damage = 12;
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
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Arrow;
            Item.scale = 1.4f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3, 0);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 Offset = Vector2.Normalize(velocity) * 10f;

            if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
            {
                position += Offset;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundEngine.PlaySound(SoundID.Item98, player.position);

            wombocombo++;
            if (wombocombo == 7)
            {
                wombocombo = 0;
            }
            int NumProjectiles = 1; // The humber of projectiles that this gun will shoot.
            if (wombocombo == 2 || wombocombo == 3)
            {
                Item.useTime = 10;
                Item.useAnimation = 40;
            }
            else
            {
                Item.useTime = 35;
                Item.useAnimation = 35;
            }
            if (wombocombo == 4)
            {
                NumProjectiles = 3;
            }
            else
            {
                NumProjectiles = 1;
            }
            if (wombocombo == 3 || wombocombo == 4 || wombocombo == 5)
            {
                for (int i = 0; i < NumProjectiles; i++)
                {
                    // Rotate the velocity randomly by 30 degrees at max.
                    Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));

                    // Decrease velocity randomly for nicer visuals.
                    newVelocity *= 1f - Main.rand.NextFloat(0.6f);

                    // Create a projectile.
                    Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
                }
            }
            else
            {
                for (int i = 0; i < NumProjectiles; i++)
                {
                    // Rotate the velocity randomly by 30 degrees at max.
                    Vector2 newVelocity = velocity.RotatedBy(MathHelper.ToRadians(1));

                    // Decrease velocity randomly for nicer visuals.

                    // Create a projectile.
                    Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
                }
            }

            return false;
        }
    }
}