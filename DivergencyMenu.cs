using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ID;

namespace Divergency
{
	public class DivergencyMenu : ModMenu
	{
		public override Asset<Texture2D> Logo => Mod.Assets.Request<Texture2D>("Assets/Textures/Title");

		public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/TheDivergencyIsReal");

		public override string DisplayName => "Divergency";

		public override void OnSelected()
		{
			SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/FuckYou"));
		}
	}
}
