using Divergency.Content.Projectiles.Magic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace Divergency.Content.Items.Weapons.Magic;

public class Photosynthesis : ModItem
{
    public override Vector2? HoldoutOffset() => Vector2.Zero;

    public override void SetStaticDefaults()
    {
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    // actually, too lazy to organize this :P
    public override void SetDefaults()
    {
        Item.DamageType = DamageClass.Magic;
        Item.damage = 25;
        Item.mana = 10;
        Item.knockBack = 3f;
        Item.noMelee = true;
        Item.shoot = ModContent.ProjectileType<PhotosynthesisBolt>();
        Item.shootSpeed = 32f;
        Item.Size = new(16);
        Item.useTime = Item.useAnimation = 5;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.UseSound = new SoundStyle("Divergency/Assets/Sounds/Items/InvocationShot") { PitchRange = (0.2f, 0.6f), Volume = 0.3f };
        Item.autoReuse = true;
        Item.useTurn = false;
        Item.value = Item.sellPrice(0, 5, 0, 0);
        Item.rare = ItemRarityID.Green;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        position = position + Vector2.UnitY.RotatedBy(velocity.ToRotation()) * Main.rand.NextFloat(-15f, 15f);
    }
}