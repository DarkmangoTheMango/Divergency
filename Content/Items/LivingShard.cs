using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items
{
    public class LivingShard : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Shard");
            Tooltip.SetDefault("Used for crafting living core items");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.Size = new Vector2(40);
            Item.scale = 1f;

            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
        }
    }
}