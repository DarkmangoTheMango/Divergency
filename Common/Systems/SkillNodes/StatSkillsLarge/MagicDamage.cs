using Terraria.ModLoader;
using Terraria;

namespace Divergency.Common.Systems.SkillNodes.StatSkillsLarge
{
    public class MagicDamage : SkillNode
    {
        public override int Size => 32;
        public override string Name => "+5 Flat and 20% Increased Magic Damage";
        public override string Description => "A test skill node thingy..";

        public void UpdateEquips(Player player)
        {
            player.GetDamage(DamageClass.Magic).Flat += 5f;
            player.GetDamage(DamageClass.Magic) *= 1.2f;
        }
    }
}
