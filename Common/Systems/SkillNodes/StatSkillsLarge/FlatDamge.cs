using Terraria.ModLoader;
using Terraria;
using System;
using Terraria.GameContent.RGB;

namespace Divergency.Common.Systems.SkillNodes.StatSkillsLarge
{
    public class FlatDamage : SkillNode
    {
        public override int Size => 32;
        public override string Name => "+2 Flat damage for each upgrade";
        public override string Description => "A test skill node thingy..";

        public override int MaxSkillLevel => 10;

        public void UpdateEquips(Player player)
        {
            player.GetDamage(DamageClass.Generic).Flat += 2f * SkillLevel;
        }
    }
}
