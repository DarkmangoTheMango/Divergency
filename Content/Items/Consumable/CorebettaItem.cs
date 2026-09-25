using Divergency.Content.NPCs.LivingGrove;
using Terraria;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Consumable;

public class CorebettaItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToCapturedCritter(ModContent.NPCType<Corebetta>());
    }
}