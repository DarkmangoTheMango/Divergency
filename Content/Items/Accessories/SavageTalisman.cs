using Divergency.Content.Dusts;
using Divergency.Content.Items.Weapons.Melee;
using Divergency.Content.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Divergency.Content.Items.Accessories;

[AutoloadEquip(EquipType.Neck)]
public class SavageTalisman : ModItem
{

    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.Size = new(16);
        Item.scale = 1f;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 3, 4, 7);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {

    }
}

public class SavageTalismanPlayer : ModPlayer
{
    public bool Equipped;

    public override void ResetEffects()
    {
        Equipped = false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Equipped)
            target.AddBuff(ModContent.BuffType<SavageTalismanDebuff>(), 60);
    }
}

public class SavageTalismanDebuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
    }

    public override void Update(NPC npc, ref int buffIndex)
    {
        npc.GetGlobalNPC<SavageTalismanNPC>().Shred = true;
    }
}

public class SavageTalismanNPC : GlobalNPC
{
    private Player player => Main.LocalPlayer;

    public bool Shred;

    public override bool InstancePerEntity => true;

    public override void ResetEffects(NPC npc)
    {
        Shred = false;
    }

    public override void OnKill(NPC npc)
    {
        if (!Shred)
            return;

        CameraSystem.ScreenShake(5, 0.9f, npc.Center);
        SoundEngine.PlaySound(new SoundStyle($"Divergency/Assets/Sounds/Custom/Blood{Main.rand.Next(3)}"), npc.Center);

        for (int k = 0; k < 40; k++)
            Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<ThickBlood>(), 0, Main.rand.NextFloat(-6f), 0, default, Main.rand.NextFloat(1f) + 1);

        player.Heal(20);
    }
}