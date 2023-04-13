using Divergency.Content.Projectiles.Ranged.Doors;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

public class DoorAmmos : GlobalItem
{
    public override bool InstancePerEntity => true;

    int[] Doors = { ItemID.WoodenDoor, ItemID.BorealWoodDoor, ItemID.MushroomDoor, ItemID.BoneDoor };

    public override void SetDefaults(Item item)
    {
        if (item.type == ItemID.WoodenDoor)
        {
            item.ammo = item.type;
        }

        for (int k = 0; k < Doors.Length; k++)
        {
            if (item.type == Doors[k])
            {
                item.ammo = ItemID.WoodenDoor;
            }
        }
    }

    public override void PickAmmo(Item weapon, Item ammo, Player player, ref int type, ref float speed, ref StatModifier damage, ref float knockback)
    {
        if (weapon.useAmmo == ItemID.WoodenDoor)
        {
            switch (ammo.type)
            {
                case ItemID.WoodenDoor:
                    type = ModContent.ProjectileType<WoodenDoor>();
                    break;
                case ItemID.BorealWoodDoor:
                    type = ModContent.ProjectileType<BorealDoor>();
                    break;
                case ItemID.MushroomDoor:
                    type = ModContent.ProjectileType<MushroomDoor>();
                    break;
                case ItemID.BoneDoor:
                    type = ModContent.ProjectileType<BoneDoor>();
                    break;
                default:
                    break;
            }
        }
    }
}