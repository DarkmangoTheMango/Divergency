using Divergency.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Buffs
{
    public class PipedDown : ModBuff
	{

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.HeldItem.type == ModContent.ItemType<HeavyMetal>())
			{
				
			}
        }
        public override void SetStaticDefaults()
		{
			//.setdefault("Bullet Mark");
			////.setdefault("Deadly Napalm is stuck to you");

			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			BuffID.Sets.LongerExpertDebuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}
    }

	
}
