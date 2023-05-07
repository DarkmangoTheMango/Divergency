using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Divergency.Common.Players;

namespace Divergency.Content.Items.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class LivingWoodGreaves : ModItem
	{
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.defense = 2;
        }

        public override void UpdateEquip(Player player)
        {


            player.moveSpeed += 0.10f;

        }
    }
    [AutoloadEquip(EquipType.Body)]

    public class LivingWoodChestplate : ModItem
    {

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
          
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {

            //will unlock one guardian slot

            player.GetDamage(DamageClass.Generic) += 0.1f; // Increase dealt damage for all weapon classes by 10%

        }
    }
    [AutoloadEquip(EquipType.Head)]
    public class LivingWoodHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
         
         
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.defense = 60;
            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {


            player.GetDamage(DamageClass.Generic) += 0.05f; // Increase dealt damage for all weapon classes by 5%
            player.GetCritChance(DamageClass.Generic) += 5;

        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<LivingWoodChestplate>() && legs.type == ModContent.ItemType<LivingWoodGreaves>();

        }
        public override void UpdateArmorSet(Player player)
        {
            if (!player.controlDown)
            {
                player.setBonus = "Enhances various stats in the near of a tree, press down for more info"; // This is the setbonus tooltip

            }
            else
            {
                player.setBonus = "Increased life regen, mana regen and damage while in the near of trees"
                + "\nIncreases damage dealt by 15%'"
                + "\nIncreases life regen by 3'"
                + "\nIncreases mana regen'"
                + "\nIncreases defense by 2'"
                + "\nIncreases damage reduction by 5%'"
                + "\nIncreases movement speed by 30%'";


            }
            player.GetModPlayer<TreePlayer>().treeCheck = true;
            if (player.GetModPlayer<TreePlayer>().treeNear != 0 && player.GetModPlayer<TreePlayer>().treeCheck)
            {
                player.GetDamage(DamageClass.Generic) += 0.15f;
                player.manaRegenCount += 8;
                player.statDefense += 2;
                player.lifeRegenCount += 3;
                player.endurance += 0.05f;
                player.moveSpeed += 0.3f;

            }
        }
    }

}

