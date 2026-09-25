using Divergency.Content.Biomes;
using Divergency.Content.NPCs.LivingGrove;
using System.Collections.Generic;

namespace Divergency.Common.GlobalNPCs;

public class LivingCoreSpawnOverride : GlobalNPC
{
    public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
    {
        if (!spawnInfo.Player.InModBiome(ModContent.GetInstance<LivingCoreBiome>()))
            return;

        pool.Clear();

        pool[ModContent.NPCType<Corebetta>()] = spawnInfo.Water ? 0.85f : 0;
        pool[ModContent.NPCType<Coreminnow>()] = spawnInfo.Water ? 0.85f : 0;
    }
}