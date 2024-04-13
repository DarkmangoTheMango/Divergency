using Terraria.ModLoader;
using Terraria;

namespace Divergency.Common.Systems.SkillNodes.StatSkillsLarge
{
    public class MeleeDamage : SkillNode
    {
        public override int Size => 32;
        public override string Name => "+5 Flat and 20% Increased Melee Damage";
        public override string Description => "A test skill node thingy..";

        public void UpdateEquips(Player player)
        {
            player.GetDamage(DamageClass.Melee).Flat += 5f;
            player.GetDamage(DamageClass.Melee) *= 1.2f;
        }
    }
}
