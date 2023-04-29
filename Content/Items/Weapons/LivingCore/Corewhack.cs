using Divergency.Content.Projectiles.Summoner.Minions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;


namespace Divergency.Content.Items.Weapons.LivingCore
{
    public class Corewhack : ModItem
    {
        public override void SetStaticDefaults()
        {
            //.setdefault("Corewhack");

            ////.setdefault("Stuff");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Summon;
            Item.damage = 16;
            Item.crit = 0;
            Item.shootSpeed = 12f;
            Item.mana = 10;
            Item.width = 52;
            Item.height = 84;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.RaiseLamp; //ItemUseStyleID.Swing;

            Item.shoot = ModContent.ProjectileType<Corewhack_Summon>();

            Item.rare = ItemRarityID.Green;

            Item.autoReuse = true;
            Item.useTurn = false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(ModContent.BuffType<CorewhackBuff>(), 100);

            Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI);

            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, -30);
        }
    }
    public class CorewhackBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            //.setdefault("Corewhack Buff");
            ////.setdefault(" -- || --");

            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 10; // make it last forever
        }
    }


}