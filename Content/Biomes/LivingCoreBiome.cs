using Divergency.Content.Events.LivingCore;
using Divergency.Content.Particles.ParticleSystems;
using Divergency.Content.Tiles.LivingGrove;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Generation;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;
using static Terraria.ModLoader.ModContent;
using SubworldLibrary;
using System.Collections.Generic;
using System.Threading;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Divergency.Content.Biomes;

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
        if (LivingCoreSubworld.Active)
            return true;
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

public class LivingCoreSubworld : Subworld
{
    public static bool Active => SubworldSystem.IsActive<LivingCoreSubworld>();

    public override int Width => 2000;

    public override int Height => 800;

    public override bool ShouldSave => true;

    public override List<GenPass> Tasks => // TODO: Add actual generation so these run
    [
        new PassLegacy("Living Core Fake Loading", (progress, configuration) =>
        {
            Thread.Sleep(1000);
        }),
        new PassLegacy("Living Core Settings", LivingCoreSubworldSettings)
    ];

    private static void LivingCoreSubworldSettings(GenerationProgress progress, GameConfiguration configurations)
    {
        Main.worldSurface = Main.maxTilesY;
        Main.rockLayer = Main.maxTilesY + 42;
    }

    public override void OnLoad()
    {
        SubworldSystem.hideUnderworld = true;
    }
}

public class LivingCoreSubworldSystem : ModSystem
{
    public override void PreUpdateWorld()
    {
        if (!SubworldSystem.IsActive<LivingCoreSubworld>())
            return;

        Main.cloudAlpha = 0f;
        Main.cloudBGActive = 0f;

        foreach (Cloud cloud in Main.cloud)
            cloud.active = false;

        Main.raining = false;
        Main.time = 27000;
        Main.dayTime = true;
        Main.eclipse = false;
        Main.windSpeedTarget = 0.2f;
        Main.windSpeedCurrent = 0.2f;

        Wiring.UpdateMech();

        TileEntity.UpdateStart();

        foreach (TileEntity te in TileEntity.ByID.Values)
            te.Update();

        TileEntity.UpdateEnd();

        if (++Liquid.skipCount > 1)
        {
            Liquid.UpdateLiquid();
            Liquid.skipCount = 0;
        }
    }
}