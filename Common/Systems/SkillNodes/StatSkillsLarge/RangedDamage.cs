using Terraria.ModLoader;
using Terraria;

namespace Divergency.Common.Systems.SkillNodes.StatSkillsLarge
{
    public class RangedDamage : SkillNode
    {
        public override int Size => 32;
        public override string Name => "+5 Flat and 20% Increased Ranged Damage";
        public override string Description => "A test skill node thingy..";

        public void UpdateEquips(Player player)
        {
            player.GetDamage(DamageClass.Ranged).Flat += 5f;
            player.GetDamage(DamageClass.Ranged) *= 1.2f;
        }
    }
}
