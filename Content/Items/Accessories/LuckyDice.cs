using Divergency.Content.Buffs;
using Divergency.Content.Projectiles.Summoner.Minions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Items.Accessories
{
    public class LuckyDice : ModItem
    {
        public override void SetStaticDefaults()
        {
            ////.setdefault("");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;

            Item.Size = new Vector2(30, 32);
            Item.scale = 1f;

            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
        }

        float luckyDiceCooldown;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (Main.rand.NextBool(4) && !hideVisual)
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                Dust.NewDustPerfect(player.Center + (speed * 100f), DustID.GemRuby, speed * -5f, 0, default, 1f).noGravity = true;
            }

            player.AddBuff(ModContent.BuffType<SpazmatismBuff>(), 60);

            if (player.ownedProjectileCounts[ModContent.ProjectileType<SpazmatismMinion>()] <= 0)
            {
                Projectile.NewProjectileDirect(player.GetSource_Buff(player.FindBuffIndex(ModContent.BuffType<SpazmatismBuff>())), player.Center, Vector2.Zero,
                ModContent.ProjectileType<SpazmatismMinion>(), 10, 2f, Main.myPlayer);
            }

            if (player.ownedProjectileCounts[ModContent.ProjectileType<RetinazerMinion>()] <= 0)
            {
                Projectile.NewProjectileDirect(player.GetSource_Buff(player.FindBuffIndex(ModContent.BuffType<SpazmatismBuff>())), player.Center, Vector2.Zero,
                ModContent.ProjectileType<RetinazerMinion>(), 5, 2f, Main.myPlayer);
            }
        }
    }
}