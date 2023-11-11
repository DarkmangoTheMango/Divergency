using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using static Terraria.ModLoader.PlayerDrawLayer;
using System.Linq;
using Terraria.GameContent;
using Terraria.UI.Chat;
using Mono.Cecil;
using Divergency.Content.Items.Weapons.Melee;
using Steamworks;
using Terraria.GameContent.UI.Elements;

namespace Divergency.Common.Systems.Skills
{
    public abstract class SkillNode
    {
        private static List<SkillInteractor> AllInteractors = new List<SkillInteractor>();
        public static List<SkillNode> AllSkills = new List<SkillNode>();

        public static float SkillTreeScale = 1f;

        public static void ClearAll() { AllSkills.Clear(); AllInteractors.Clear(); }

        public Vector2 position = Vector2.Zero;

        private struct SkillInteractor
        {

            public SkillNode skillNode1;
            public SkillNode skillNode2;

            public SkillInteractor(SkillNode skillNode1, SkillNode skillNode2)
            {
                this.skillNode1 = skillNode1;
                this.skillNode2 = skillNode2;

                SkillNode.AllInteractors.Add(this);
            }

            public SkillNode GetOtherNode(SkillNode oneSkill)
            {
                if (skillNode1 == oneSkill)
                    return skillNode2;
                else if (skillNode2 == oneSkill)
                    return skillNode1;

                return null;
            }

            private static Texture2D LineTexture = ModContent.Request<Texture2D>("Divergency/Common/Systems/SkillNodes/SkillLine", AssetRequestMode.ImmediateLoad).Value;
            public void Draw(SpriteBatch spriteBatch, Vector2 offset)
            {
                Vector2 Line = skillNode1.position - skillNode2.position;

                int xS = (int)(Line.Length());  
                int yS = 7;

                Vector2 lineSize = new Vector2(xS, yS);

                Color color = Color.Gray;

                if (skillNode1.Learned && skillNode2.Learned)
                    color = Color.White;
                else if (skillNode1.Learned != skillNode2.Learned)
                    color = Color.Yellow;


                spriteBatch.Draw(LineTexture, (skillNode2.position + Line / 2f) * SkillTreeScale + offset, new Rectangle(0, 0, xS, yS), color, Line.ToRotation(), lineSize / 2f, SkillTreeScale, SpriteEffects.None, 0f);
            }
        }

        private List<SkillInteractor> touchingSkills = new List<SkillInteractor>();

        public virtual string Texture => "Divergency/Common/Systems/SkillNodes/SkillBase";

        public abstract int Size { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }
        private string[] DescLines;

        public bool Learned = false;

        public SkillNode()
        {
            SkillNode.AllSkills.Add(this);
        }

        public static void DrawLines(SpriteBatch spriteBatch, Vector2 offset)
        {
            foreach (SkillInteractor si in AllInteractors)
            {
                si.Draw(spriteBatch, offset);
            }
        }

        public static SkillNode GetSkillMousedOver(Vector2 offset, Vector2 m)
        {
            foreach (SkillNode s in AllSkills)
            {
                Vector2 p = s.position * SkillTreeScale + offset - new Vector2(s.Size, s.Size) / 2f * SkillTreeScale;
                if (m.X > p.X && m.X < p.X+s.Size * SkillTreeScale && m.Y > p.Y && m.Y < p.Y + s.Size * SkillTreeScale)
                {
                    return s;
                }
            }

            return null;
        }

        public static void DrawSkills(SpriteBatch spriteBatch, Vector2 offset)
        {
            foreach (SkillNode s in AllSkills)
            {
                s.Draw(spriteBatch, offset);
            }
        }

        public bool IsUnlockable()
        {
            if (touchingSkills.Count == 0)
                return true;

            foreach (SkillInteractor SI in touchingSkills)
            {
                if (SI.GetOtherNode(this).Learned)
                    return true;
            }

            return false;
        }

        private static Texture2D Pixel = ModContent.Request<Texture2D>("Divergency/Assets/Textures/WhitePixel", AssetRequestMode.ImmediateLoad).Value;
        public void DrawDesc(SpriteBatch spriteBatch, Vector2 mousePos)
        {
            mousePos += new Vector2(20f, 20f);
            Vector2 DescSize = Vector2.Zero;

            Vector2 Border = new Vector2(6f, 6f);

            float extraLineWidth = 0; // seems to not be needed
            Color baseColor = Color.White;

            int X = (int)MathF.Ceiling(mousePos.X + Border.X);
            int Y = (int)MathF.Ceiling(mousePos.Y + Border.Y);

            Vector2 NameDimensions = ChatManager.GetStringSize(FontAssets.MouseText.Value, Name, Vector2.One, -1f);
            float NameHeight = NameDimensions.Y * 1.2f;

            if (DescLines == null)
                DescLines = Description.Split("\n");

            foreach (string line in DescLines)
            {
                Vector2 stringSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, line, Vector2.One, -1f);
                if (stringSize.X > DescSize.X)
                {
                    DescSize.X = stringSize.X;
                }
                DescSize.Y += stringSize.Y + extraLineWidth;
            }

            DescSize += Border * 2f;
            DescSize.Y += NameHeight;
            DescSize.X += NameDimensions.X;

            Rectangle DescBG = new Rectangle((int)MathF.Ceiling(mousePos.X), (int)MathF.Ceiling(mousePos.Y), (int)MathF.Ceiling(DescSize.X), (int)MathF.Ceiling(DescSize.Y));

            spriteBatch.Draw(Pixel, DescBG, Color.Orange);

            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, Name, new Vector2(X, Y), baseColor, 0f, Vector2.Zero, Vector2.One, -1f, 2f);

            Y += (int)MathF.Ceiling(NameHeight);

            foreach (string line in DescLines)
            {
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, line, new Vector2(X, Y), baseColor, 0f, Vector2.Zero, Vector2.One, -1f, 2f);
                Y += (int)(FontAssets.MouseText.Value.MeasureString(line).Y + extraLineWidth);
            }
        }

        public SkillNode AddBS(SkillNode skill, float rotation) // AddBranchingSkill
        {
            return AddBS(skill, rotation.ToRotationVector2() * ((float)skill.Size + (float)Size));
        }

        public SkillNode AddBS(SkillNode skill, Vector2 skillOffset) // AddBranchingSkill
        {
            skill.position = position + skillOffset;

            return Connect(skill);
        }
        public SkillNode Connect(SkillNode skill) // Meant as path to get to the skill...
        {
            SkillInteractor SI = new SkillInteractor(this, skill);

            touchingSkills.Add(SI);
            skill.touchingSkills.Add(SI);
            return skill;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            Texture2D DrawTexture = ModContent.Request<Texture2D>(Texture, AssetRequestMode.ImmediateLoad).Value;
            spriteBatch.Draw(DrawTexture, (position - new Vector2(Size, Size) / 2f) * SkillTreeScale + offset, null, Learned ? Color.White : Color.Gray, 0f, Vector2.Zero, (float)Size / DrawTexture.Width * SkillTreeScale, SpriteEffects.None, 0f);
        }

        virtual public void PostUpdateEquipment() { }
    }
}
