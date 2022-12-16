using Divergency.Content.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.Magic
{
    public class LivingLeafStorm : ModItem
    {
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) => velocity = velocity.RotatedByRandom(MathHelper.ToRadians(10));

        public override Vector2? HoldoutOffset() => Vector2.Zero;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("(Work In Progress) Living Leaf Storm");
            Tooltip.SetDefault("Summons explosive leaves\nRight click to detonate instantly");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Magic;
            Item.damage = 15;
            Item.mana = 10;
            Item.knockBack = 3.5f;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<LivingLeaf>();
            Item.shootSpeed = 20f;
            Item.channel = true;

            Item.width = Item.height = 16;
            Item.scale = 1f;

            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.DD2_LightningAuraZap;
            Item.autoReuse = true;
            Item.useTurn = false;

            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
        }
    }
}