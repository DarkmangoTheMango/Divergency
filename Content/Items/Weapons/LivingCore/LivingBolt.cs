using Divergency.Content.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.LivingCore
{
    public class LivingBolt : ModItem
    {
        public override Vector2? HoldoutOffset() => Vector2.Zero;

        public override void SetStaticDefaults()
        {
            ////.setdefault("AW FUCK");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Magic;
            Item.damage = 25;
            Item.mana = 10;
            Item.knockBack = 3f;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<Projectiles.Magic.LivingBolt>();
            Item.shootSpeed = 9f;
            Item.channel = true;

            Item.Size = new Vector2(30, 34);
            Item.scale = 1f;

            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
            Item.useTurn = false;

            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
        }
    }
}