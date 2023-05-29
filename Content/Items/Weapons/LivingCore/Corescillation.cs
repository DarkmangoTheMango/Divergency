using Divergency.Content.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.LivingCore
{
    public class Corescillation : ModItem
    {
        private int wombocombo;

        public override Vector2? HoldoutOffset() => new Vector2(-10,-0);

        public override void SetStaticDefaults()
        {
            ////.setdefault("AW FUCK");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Magic;
            Item.damage = 32;
            Item.mana = 11;
            Item.knockBack = 3f;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<Projectiles.Magic.CorescillationProj>();
            Item.shootSpeed = 14f;
            Item.channel = true;

            Item.Size = new Vector2(30, 34);
            Item.scale = 1f;

            Item.useTime = Item.useAnimation = 38;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
            Item.useTurn = false;

            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundEngine.PlaySound(SoundID.DD2_DarkMageAttack, player.position);

            wombocombo++;
            if (wombocombo == 10)
            {
                wombocombo = 0;
            }
            int NumProjectiles = 1; // The humber of projectiles that this gun will shoot.
            if (wombocombo == 5 || wombocombo == 6)
            {
                Item.useTime = 10;
                Item.useAnimation = 40;
            }
            else
            {
                Item.useTime = 35;
                Item.useAnimation = 35;
            }
            if (wombocombo == 7)
            {
                NumProjectiles = 3;
            }
            else
            {
                NumProjectiles = 1;
            }
            if (wombocombo == 6 || wombocombo == 7 || wombocombo == 8)
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

        
    
