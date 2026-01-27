using Divergency.Content.Dusts;
using Terraria.Audio;

namespace Divergency.Content.Tiles.LivingGrove;

public class CradleWood : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileMergeDirt[Type] = true;
        Main.tileBlockLight[Type] = true;

        AddMapEntry(new Color(64, 39, 40));

        DustType = ModContent.DustType<CradleWoodFurniture>();
        HitSound = new SoundStyle("Divergency/Assets/Sounds/Tiles/WoodDig", 3) with { PitchVariance = 0.1f };
    }
}

public class CradleWoodItem : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<CradleWood>(), 0);
    }
}