using Divergency.Content.NPCs.LivingGrove;
using Terraria;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Consumable;

public class CoreminnowItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToCapturedCritter(ModContent.NPCType<Coreminnow>());
    }
}