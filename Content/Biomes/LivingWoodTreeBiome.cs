using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;
using Terraria.GameContent;

namespace Divergency.Content.Biomes
{
	public class LivingWoodTreeBiome : ModBiome
	{

		// Select all the scenery
		//public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("ExampleMod/ExampleUndergroundBackgroundStyle");

		// Select Music
		public override int Music => MusicLoader.GetMusicSlot("DivergencyMod/Sounds/Music/LivingWoodTreeTheme");

		// Sets how the Scene Effect associated with this biome will be displayed with respect to vanilla Scene Effects. For more information see SceneEffectPriority & its values.
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh; // We have set the SceneEffectPriority to be BiomeLow for purpose of example, however default behavour is BiomeLow.

		// Populate the Bestiary Filter
		//public override string BestiaryIcon => base.BestiaryIcon;
		//public override string BackgroundPath => base.BackgroundPath;
		//public override Color? BackgroundColor => base.BackgroundColor;

		// Use SetStaticDefaults to assign the display name
		public override void SetStaticDefaults()
		{
			//.setdefault("Example Underground");
	       
		}
		public override ModWaterStyle WaterStyle => base.WaterStyle;
		// Calculate when the biome is active.
		public override bool IsBiomeActive(Player player)
		{
			// Limit the biome height to be underground in either rock layer or dirt layer
			return (player.ZoneOverworldHeight || player.ZoneNormalSpace) &&
				// Check how many tiles of our biome are present, such that biome should be active
				ModContent.GetInstance<BiomeTileCountLivingWood>().BlockCount >= 50;
		}
	}
	public class BiomeTileCountLivingWood : ModSystem
	{
		public int BlockCount;

		public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
		{
			BlockCount = tileCounts[TileID.LivingWood];
		}
	}
}