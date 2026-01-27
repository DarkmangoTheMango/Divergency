using Divergency.Common.Players;
using Divergency.Content.Dusts;
using Divergency.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Buffs
{
	public class CoreBurn : ModBuff
	{
		public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<CoreBurnNPC>().shred = true;
        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<CoreBurnPlayer>().lifeRegenDebuff = true;

        public override void SetStaticDefaults()
		{
			//.setdefault("Shred");
			////.setdefault("Rapidly losing life");

			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			BuffID.Sets.LongerExpertDebuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}
    }
    public class CoreBurnPlayer : ModPlayer
    {
        public bool lifeRegenDebuff;

        public override void ResetEffects()
        {
            lifeRegenDebuff = false;
        }

       
        public override void UpdateBadLifeRegen()
        {
            if (lifeRegenDebuff)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                Player.lifeRegen -= 20;
            }
        }
        public override void PreUpdate()
        {
            Player player = Main.LocalPlayer;
            if (lifeRegenDebuff)
            {
                if (Main.rand.NextBool(4)) { Dust.NewDustDirect(player.position, player.width, player.height, DustID.PortalBoltTrail, Main.rand.NextFloat(-4,4), Main.rand.NextFloat(-4, 4), 0, Color.LimeGreen, 1f).noGravity = true; }

                Lighting.AddLight(player.Center, 0.2f, 1, 0.08f);
                player.AddBuff(BuffID.Slow, 10);
            }

            
        }


    }
    public class CoreBurnNPC : GlobalNPC
	{
        public override bool InstancePerEntity => true;

        public bool shred;

		public override void ResetEffects(NPC npc) => shred = false;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
			if (shred)
			{
				if (npc.lifeRegen > 0) { npc.lifeRegen = 0; }

				npc.lifeRegen -= 5;
				npc.AddBuff(BuffID.Slow,60);
				if (damage < 1) { damage = 1; }
			}
		}

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
			if (shred)
			{
				if (Main.rand.NextBool(4)) { Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.PortalBoltTrail, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 0, default, 1f).noGravity = true; }

				Lighting.AddLight(npc.Center, 0.2f, 1, 0.08f);
			}
		}

        
    }



	public class CoreBurnIcon : ModItem
    {
		public override string Texture => "Divergency/Content/Buffs/Shred";
	}
}
