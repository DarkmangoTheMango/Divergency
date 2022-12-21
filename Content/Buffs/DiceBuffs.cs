using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.Buffs
{
    public class Ace : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ace");
			Description.SetDefault("Major decrease to all stats \nCannot regenerate life");

			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<DicePlayer>().ace = true;
			player.statLifeMax2 -= 50;
			player.statDefense -= 5;
			player.statManaMax -= 10;
			player.lifeRegen = 0;
			player.GetDamage(DamageClass.Generic) -= 0.20f;
			player.GetAttackSpeed(DamageClass.Generic) -= 0.20f;
		}
	}

	public class Deuce : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Deuce");
			Description.SetDefault("You are defended by two probes");

			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<DicePlayer>().deuce = true;
			player.twinsMinion = true;
		}
	}

	public class DicePlayer : ModPlayer
	{
        public bool ace;

		public bool deuce;

		public override void ResetEffects()
		{
			ace = false;
			deuce = false;
		}

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
			if (ace) { if (Main.rand.NextBool(4)) { Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.GemRuby, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 0, default, 1f).noGravity = true; } }
		}
    }
}
