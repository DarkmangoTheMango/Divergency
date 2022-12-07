using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Weapons.Magic
{
    public class VineBoom : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vine Boom");
            Tooltip.SetDefault("'A fractured crystal that fused with the roots of the Living Tree'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.damage = 15;
            Item.crit = 3;
            Item.knockBack = 5f;

            Item.width = 50;
            Item.height = 76;
            Item.scale = 1.5f;

            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = false;

            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
        }
    }
}