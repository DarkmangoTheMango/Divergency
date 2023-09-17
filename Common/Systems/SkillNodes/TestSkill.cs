using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Divergency.Common.Systems.Skills
{
    public class TestSkill : SkillNode
    {
        public override int Size => 32;
        public override string Name => "Test";
        public override string Description => "A test skill node thingy.\n Gives +20% Damage.";
    }
}
