using Divergency.Content.Events.LivingCore;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ModLoader;

namespace Divergency.Events.LivingCore
{
	public class LivingCoreProgressBar : ModBossBar
	{
		private int HeadIndex = -1;

		public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
		{
			// Display the previously assigned head index
			if (HeadIndex != -1)
			{
				return TextureAssets.NpcHeadBoss[HeadIndex];
			}
			return null;
		}

        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax)
        {
   
			// Here the game wants to know if to draw the boss bar or not. Return false whenever the conditions don't apply.
			// If there is no possibility of returning false (or null) the bar will get drawn at times when it shouldn't, so write defensive code!

			NPC npc = Main.npc[info.npcIndexToAimAt];
			if (!npc.active)
				return false;

			// We assign bossHeadIndex here because we need to use it in GetIconTexture
			HeadIndex = npc.GetBossHeadTextureIndex();

			float progress = LivingCoreEvent.GetProgress();
			//lifePercent = Utils.Clamp(1f - progress, 0f, 1f);
			return true;
		}
	}
}
