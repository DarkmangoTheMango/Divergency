using Terraria.ModLoader;
using Terraria;

namespace Divergency.Common.Systems.SkillNodes.StatSkillsLarge
{
    public class SummonDamage : SkillNode
    {
        public override int Size => 32;
        public override string Name => "+5 Flat and 20% Increased Summon Damage";
        public override string Description => "A test skill node thingy..";

        public void UpdateEquips(Player player)
        {
            player.GetDamage(DamageClass.Summon).Flat += 5f;
            player.GetDamage(DamageClass.Summon) *= 1.2f;
        }
    }
}
