using Terraria.ModLoader;
using Terraria;

namespace Divergency.Common.Systems.SkillNodes.StatSkills
{
    public class SpeedSkill : SkillNode
    {
        public override int Size => 16;
        public override string Name => "Swing faster";
        public override string Description => "Adds +2% attack speed.";

        // and then you just make it like v that v...
        public void UpdateEquips(Player player)
        {
            player.GetAttackSpeed(DamageClass.Generic) += 0.02f;
        }
    }
}
