using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Divergency.Common.Players;

namespace Divergency.Content.Items.Accessories
{
    public class AcornRepellentSpray : ModItem
	{

		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("'Being annoyed by noisy Acorns? Then this is what you were searching for! Favorite to activate its effects.'");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(10);
			Item.rare = ItemRarityID.Green;
	


		}

        public override void UpdateInventory(Player player)
        {
			if (Item.favorited)
			{
				player.GetModPlayer<AcornPlayer>().checkTree = false;
			}
            else
            {
				player.GetModPlayer<AcornPlayer>().checkTree = true;

			}
		}

	}
	
}