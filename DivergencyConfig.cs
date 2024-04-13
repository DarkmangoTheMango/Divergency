using Divergency.Common.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.ComponentModel;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.WorldBuilding;

namespace Divergency
{
    public class SkillTreeConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;
        /*
        [Header("$Config.SkillTree.Header")]
        [Label("$Config.SkillTree.Scale.Label")]
        [Tooltip("$Config.Scale.Tooltip")]
        */
        [Header("SkillTree")]
        [Label("Scale")]
        [Tooltip("The scale of the skill tree")]
        [DefaultValue(1f)]
        [Range(0.5f, 4f)]
        public float Scale;
    }
}