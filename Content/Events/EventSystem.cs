using Divergency.Content.Events.LivingCore;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Divergency.Content.Events.LivingCore
{
    public class EventSystem : ModSystem
	{
		public override void PreUpdateInvasions()
		{
			if (LivingCoreEvent.Active)
				LivingCoreEvent.Update();

            if (KeybindSystem.Begin.JustPressed)
            {
                LivingCoreEvent.Begin(LivingCoreEvent.lastI, LivingCoreEvent.lastJ, new InfiniteRoom());
            }
        }

		public override void PostDrawTiles()
		{
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			if (LivingCoreEvent.Active)
				LivingCoreEvent.Draw(Main.spriteBatch);

			Main.spriteBatch.End();
		}

		public override void OnWorldLoad()
		{
			LivingCoreEvent.Load();
		}

		public override void OnWorldUnload()
		{
			LivingCoreEvent.Unload();
		}
	}
}
