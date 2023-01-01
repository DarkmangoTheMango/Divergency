
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using System;
using Terraria.Audio;
using Divergency.Content.Projectiles;

namespace Divergency.Content.Items.Accessories
{
    public class LivingCoreConductor : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Combines the powers of heart and conductor");

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
            player.GetModPlayer<HeartDrop>().Orbs = true;
            player.GetModPlayer<AcornDrop>().Acorns = true;
        }




    }

  
   
}