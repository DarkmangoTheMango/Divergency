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
using ReLogic.Content;
using Divergency.Content.Tiles.LivingGrove;
using Divergency.Content.Particles.ParticleSystems;
using Divergency.Content.Events.LivingCore;

namespace Divergency.Content.Biomes
{
	public class LivingCoreBiome : ModBiome
	{

		// Select all the scenery
		//public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("ExampleMod/ExampleUndergroundBackgroundStyle");

		// Select Music
		public override int Music => MusicLoader.GetMusicSlot("Divergency/Assets/Sounds/Music/LivingGrove");

		// Sets how the Scene Effect associated with this biome will be displayed with respect to vanilla Scene Effects. For more information see SceneEffectPriority & its values.
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeLow; // We have set the SceneEffectPriority to be BiomeLow for purpose of example, however default behavour is BiomeLow.
       // public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("Divergency/LivingCoreBiomeBackgroundStyle");
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("Divergency/LivingCoreBiomeSurfaceStyle");
        // Populate the Bestiary Filter
        public override string BestiaryIcon => "Divergency/Assets/Backgrounds/LivingCoreBiomeIcon";
        public override string BackgroundPath => "Divergency/Assets/Backgrounds/LivingCoreBiomeMap";
        public override Color? BackgroundColor => Color.Teal;
        public override int BiomeTorchItemType => ModContent.ItemType<CoreTorch>();
        public override string MapBackground => BackgroundPath;
        // Use SetStaticDefaults to assign the display name
        public override void SetStaticDefaults()
		{
			//.setdefault("Living Core Biome");
	        
		}
		
        public override ModWaterStyle WaterStyle => ModContent.GetInstance<LivingCoreWater>();

        public int timer { get; private set; }

        public override bool IsBiomeActive(Player player)
		{
			// Limit the biome height to be underground in either rock layer or dirt layer
			return (player.ZoneRockLayerHeight || player.ZoneDirtLayerHeight || player.ZoneOverworldHeight) &&
				// Check how many tiles of our biome are present, such that biome should be active
				ModContent.GetInstance<BiomeTileCount>().BlockCount >= 100;
		}
        public override void OnInBiome(Player player)
        {
			timer++;
		
            Vector2 newVelocity = new Vector2(1).RotatedByRandom(MathHelper.ToRadians(360));

            // Decrease velocity randomly for nicer visuals.
            newVelocity *= 1f - Main.rand.NextFloat(0.7f, 1.5f);
            if (timer == 30)
            {
				if (LivingCoreEvent.Active)
				{
               //     ParticleSystemManager.ExampleQuadSystem.NewParticle(player.position + (newVelocity * Main.rand.NextFloat(1000, 3000)), new Vector2(1) * 1.5f, ParticleSystemManager.QuadParticleTest);

                }
				{
                    ParticleSystemManager.ExampleQuadSystem.NewParticle(player.position + (newVelocity * Main.rand.NextFloat(500, 2000)), new Vector2(1) * 20f, ParticleSystemManager.CoreGroveVisualBackGround);
                    ParticleSystemManager.ExampleQuadSystem.NewParticle(player.position + (newVelocity * Main.rand.NextFloat(500, 2000)), new Vector2(1) * 1f, ParticleSystemManager.CoreGroveVisualForeground);

                }
                timer = 0;
				
            }
        }
    }
	public class BiomeTileCount : ModSystem
	{
		public int BlockCount;

		public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
		{
			BlockCount = tileCounts[ModContent.TileType<CradleWood>()];
		}
	}
   
   
}