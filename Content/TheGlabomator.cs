using Terraria.Audio;
using Terraria.ID;

namespace Divergency.Content;

public class TheGlabomator : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.DamageType = DamageClass.Ranged;
        Item.noMelee = true;
        Item.damage = 67;
        Item.knockBack = 2f;
        Item.shoot = ProjectileID.Bullet;
        Item.shootSpeed = 13f;
        Item.width = 16;
        Item.height = 16;
        Item.useTime = 40;
        Item.useAnimation = 40;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.autoReuse = true;
        Item.value = Item.sellPrice(0, 4, 0, 0);
        Item.rare = ItemRarityID.Green;
        Item.UseSound = SoundID.Item41;
    }
}