using Terraria.ID;

namespace Divergency.Content.Items;

public class LivingShard : ModItem
{
    public override Color? GetAlpha(Color lightColor) => new(255, 255, 255);

    public override void SetStaticDefaults()
    {
        ItemID.Sets.ItemIconPulse[Item.type] = true;
    }

    public override void SetDefaults()
    {
        Item.Size = new(22, 34);
        Item.scale = 1;
        Item.value = Item.sellPrice(0, 0, 50, 0);
        Item.rare = ItemRarityID.Green;
        Item.maxStack = Item.CommonMaxStack;
    }

    public override void Update(ref float gravity, ref float maxFallSpeed)
    {
        if (!Main.dedServ)
            if (Main.rand.NextBool(10))
                Dust.NewDustPerfect(Item.position + new Vector2(Item.width, Item.height) * Main.rand.NextVector2Square(0, 1), DustID.Terra, -Vector2.UnitY * (Main.rand.NextFloat(1f) + 1), 0, default, 0.7f).rotation = Main.rand.NextFloat(MathHelper.TwoPi);

        Lighting.AddLight(Item.Center, new Vector3(0, 0.1f, 0));
    }
}