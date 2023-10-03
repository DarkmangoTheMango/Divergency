using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace Divergency.Common.Systems.Skills
{
    public class TestSkill : SkillNode
    {
        public override int Size => 32;
        public override string Name => "+5 flat & 20% Damage";
        public override string Description => "A test skill node thingy.\nGives +5 flat damage and +20% Damage.";

        public void UpdateEquips(Player player)
        {
            player.GetDamage(DamageClass.Generic).Flat += 5f;
            player.GetDamage(DamageClass.Generic) *= 1.2f;
        }
    }
}
