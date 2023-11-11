using Divergency.Content.Events.LivingCore;
using Divergency.Tiles.LivingTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Events.LivingCore
{
	public class LivingCoreEventHandler : ModNPC
	{
		public override string Texture => "Divergency/Assets/Textures/Empty";

		public override void SetDefaults()
		{
			NPC.width = 1;
			NPC.height = 1;
			NPC.lifeMax = 1;
			NPC.immortal = true;
			NPC.friendly = false;
			NPC.dontTakeDamage = true;

			if (LivingCoreEvent.Room != null)
				Music = LivingCoreEvent.Room.Music;

			NPC.aiStyle = -1;
			NPC.noGravity = true;
			NPC.boss = true;
			///NPC.BossBar = ModContent.GetInstance<LivingCoreProgressBar>();
			NPC.ShowNameOnHover = false;
			NPC.alpha = 255;
		}

		public override void AI()
		{
			if (!LivingCoreEvent.Active)
				NPC.life = 0;
		}

		public override bool CheckActive()
		{
			return false;
		}
	}
}