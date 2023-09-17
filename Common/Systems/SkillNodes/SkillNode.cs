using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;

namespace Divergency.Common.Systems.Skills
{
    public abstract class SkillNode
    {
        private Vector2 offset = Vector2.Zero;

        public bool learnt;

        private List<SkillNode> branchingSkills = new List<SkillNode>();
        private List<SkillNode> parentSkills = new List<SkillNode>();

        public abstract int Size { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }


        public SkillNode AddBS(SkillNode skill, float rotation) // AddBranchingSkill
        {
            skill.offset = rotation.ToRotationVector2() * ((float)skill.Size + (float)Size);
            branchingSkills.Add(skill);
            skill.parentSkills.Add(this);
            return skill;
        }
        public SkillNode AddBS(SkillNode skill, Vector2 skillOffset) // AddBranchingSkill
        {
            skill.offset = skillOffset;
            branchingSkills.Add(skill);
            skill.parentSkills.Add(this);
            return skill;
        }

        private static Texture2D Pixel = ModContent.Request<Texture2D>("Divergency/Assets/Textures/WhitePixel", AssetRequestMode.ImmediateLoad).Value;
        private static Texture2D Square = ModContent.Request<Texture2D>("Divergency/Common/Systems/SkillNodes/SquareGlow", AssetRequestMode.ImmediateLoad).Value;
        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            foreach (SkillNode skill in branchingSkills)
            {
                int xS = (int)(skill.offset).Length();
                int yS = 4;

                Vector2 lineSize = new Vector2(xS, yS);

                spriteBatch.Draw(Pixel, position + skill.offset / 2f, new Rectangle(0, 0, xS, yS), Color.White, skill.offset.ToRotation(), lineSize / 2f, 1f, SpriteEffects.None, 0f);

                skill.Draw(spriteBatch, position + skill.offset);
            }

            spriteBatch.Draw(Square, position - new Vector2(Size, Size) / 2f, null, Color.White, 0f, Vector2.Zero, (float)Size/ Square.Width, SpriteEffects.None, 0f);
        }

        virtual public void PostUpdateEquipment() { }
    }
}
