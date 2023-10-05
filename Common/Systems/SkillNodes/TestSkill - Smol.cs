using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Divergency.Common.Systems.Skills
{
    public class TestSkillSmol : SkillNode
    {
        public override int Size => 16;
        public override string Name => "+1 Damage";
        public override string Description => "Adds +1 flat damage.";

        public void UpdateEquips(Player player)
        {
            player.GetDamage(DamageClass.Generic).Flat += 1f;
        }
    }
}
