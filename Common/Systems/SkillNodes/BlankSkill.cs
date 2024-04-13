using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace Divergency.Common.Systems.SkillNodes
{
    public class BlankSkill : SkillNode
    {
        public override int Size => 16;
        public override string Name => "Blank Skill nr: " + idx;
        public override string Description => "";
        private int idx;
        public static int _idx;

        public BlankSkill()
        {
            _idx++;
            idx = _idx;
        }
    }
}
