
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;


namespace Divergency.Content.Items.Accessories
{
    public class AttackSpeeder : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rapidity Glove");
			Tooltip.SetDefault("Doubles your ranged weapons fire rate and enables Auto-Shoot, however ranged damage is decreased by 50% \n'Fire quicker than your own shadow!'");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(10);	
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;


		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			if (player.HeldItem.DamageType == DamageClass.Ranged)
			{
				player.HeldItem.autoReuse = true;
			}
			 player.GetDamage(DamageClass.Ranged) *= 0.5f; // Increase ALL player damage by 100%
             player.GetAttackSpeed(DamageClass.Ranged) *= 2f;


		}




	}

}