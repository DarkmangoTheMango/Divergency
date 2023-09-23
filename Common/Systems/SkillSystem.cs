using Divergency.Common.Systems.Skills;
using Divergency.Content.Events.LivingCore;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using rail;
using System;
using Terraria.DataStructures;
using System.IO.Pipelines;
using Terraria.GameInput;
using Terraria.GameContent.UI.Elements;
using ReLogic.Content;
using static Terraria.GameContent.Animations.On_Actions.Sprites;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static System.Net.WebRequestMethods;

namespace Divergency.Common.Systems
{
    public class SkillTreePlayer : ModPlayer
    {
        private int SkillPointTotal = 100;
        private int SkillPointsUsed = 0;

        public int SkillPoints
        {
            get { return SkillPointTotal - SkillPointsUsed; }
        }

        public void GetSkill(SkillNode skill)
        {
            skill.Learned = true;
            SkillPointsUsed++;
        }

        public override void PreUpdate()
        {

        }

        public override void UpdateEquips()
        {

        }
    }

    public class SkillTreeHelper
    {
        SkillNode curNode;

        List<SkillNode> nodeHistory = new List<SkillNode>();

        Dictionary<string, int> label = new Dictionary<string, int>();

        Dictionary<string, SkillNode> savedAs = new Dictionary<string, SkillNode>();

        public SkillTreeHelper(SkillNode curNode)
        {
            this.curNode = curNode;
            nodeHistory.Add(curNode);
        }

        public void AddNSet(SkillNode skill, float rotation)
        {
            nodeHistory.Add(skill);
            curNode = curNode.AddBS(skill, rotation);
        }

        public void AddNSet(SkillNode skill, Vector2 skillOffset)
        {
            nodeHistory.Add(skill);
            curNode = curNode.AddBS(skill, skillOffset);
        }

        public void Connect(string s)
        {
            savedAs[s].Connect(curNode);
        }

        public void GOTOLabel(string s)
        {
            int traceTo = label[s];

            TrackeBack(nodeHistory.Count - traceTo);

            foreach (var pair in label)
            {
                if (pair.Value > nodeHistory.Count)
                    label.Remove(pair.Key);
            }
        }

        public void TrackeBack(int length)
        {
            nodeHistory.RemoveRange(nodeHistory.Count - length, length);

            curNode = nodeHistory.Last<SkillNode>();
        }

        public void SetLabel(string s)
        {
            label.Add(s, nodeHistory.Count);
        }

        public void SaveAs(string s)
        {
            savedAs.Add(s, curNode);
        }
    }

    public class SkillTreeSystem : ModSystem
    {
        public List<List<int>> SkillReq = new List<List<int>>();
        public List<SkillNode> Skills = new List<SkillNode>();

        public static ModKeybind ToggleSkillTree { get; private set; }

        bool IsSkillTreeActive = false;
        public Vector2 offset;
        public Vector2 prevMousePos;
        public SkillNode tryClickNode;
        public bool MouseIsDown;

        public SkillNode CoreSkill;

        public override void Load()
        {
            ToggleSkillTree = KeybindLoader.RegisterKeybind(Mod, "Toggle Skill Tree", "P");

            SkillNode.ClearAll();

            CoreSkill = new TestSkill();
            CoreSkill.Learned = true;

            SkillTreeHelper STH = new SkillTreeHelper(CoreSkill);
            STH.SetLabel("Core");

            STH.AddNSet(new TestSkillSmol(), new Vector2(1f, 0f).ToRotation());
                STH.AddNSet(new TestSkill(), -MathF.PI / 4f);
                    STH.TrackeBack(1);
                STH.AddNSet(new TestSkillSmol(), 0f);
                    STH.TrackeBack(1);
                STH.AddNSet(new TestSkill(), MathF.PI / 4f);
                    STH.GOTOLabel("Core"); // STH.TrackeBack(2);

            STH.AddNSet(new TestSkillSmol(), new Vector2(-1f, 0f).ToRotation());
                STH.AddNSet(new TestSkillSmol(), MathF.PI);
                    STH.AddNSet(new TestSkillSmol(), MathF.PI);
                        STH.SaveAs("LeftThingy");
                        STH.TrackeBack(2);
                STH.AddNSet(new TestSkill(), MathF.PI - MathF.PI / 4f);
                    STH.Connect("LeftThingy");
                    STH.TrackeBack(1);
                STH.AddNSet(new TestSkill(), MathF.PI + MathF.PI / 4f);
                    STH.Connect("LeftThingy");
                    STH.GOTOLabel("Core");

            STH.AddNSet(new TestSkill(), new Vector2(0f, -1f).ToRotation());
                STH.AddNSet(new TestSkill(), new Vector2(1, -1f).ToRotation());
                    STH.AddNSet(new TestSkill(), new Vector2(0f, -1f).ToRotation());
                        STH.AddNSet(new TestSkill(), new Vector2(-1f, -1f).ToRotation());
                            STH.SaveAs("Test");
                            STH.TrackeBack(3);
                STH.AddNSet(new TestSkill(), new Vector2(-1f, -1f).ToRotation());
                    STH.AddNSet(new TestSkill(), new Vector2(0, -1f).ToRotation());
                        STH.Connect("Test");
        }
        public override void Unload()
        {
            ToggleSkillTree = null;
        }

        public override void PreUpdatePlayers()
        {
            if (ToggleSkillTree.JustPressed)
            {
                IsSkillTreeActive = !IsSkillTreeActive;
                offset = new Vector2();
            }

            if (IsSkillTreeActive && GetTreeRectangle().Contains(Main.mouseX, Main.mouseY))
            {
                Main.blockMouse = true;
                // Main.hoverSomething...
            }

            if (Main.mouseLeft && Main.mouseLeftRelease && GetTreeRectangle().Contains(Main.mouseX, Main.mouseY))
            {
                //Load();
                MouseIsDown = true;
                prevMousePos = new Vector2(Main.mouseX, Main.mouseY);

                Vector2 Center = Main.ScreenSize.ToVector2() / 2f - offset;
                tryClickNode = SkillNode.GetSkillMousedOver(Center, new Vector2(Main.mouseX, Main.mouseY));
            }

            if (Main.mouseLeft && MouseIsDown)
            {
                if (tryClickNode == null)
                {
                    Vector2 newMousePos = new Vector2(Main.mouseX, Main.mouseY);

                    offset = offset + (prevMousePos - newMousePos);
                    prevMousePos = newMousePos;
                }
            }
            else if (!Main.mouseLeft && MouseIsDown)
            {
                MouseIsDown = false;

                Vector2 Center = Main.ScreenSize.ToVector2() / 2f - offset;
                SkillNode curHover = SkillNode.GetSkillMousedOver(Center, new Vector2(Main.mouseX, Main.mouseY));

                Console.WriteLine("node");
                if (tryClickNode != null && curHover == tryClickNode)
                {
                    SkillTreePlayer STP = Main.LocalPlayer.GetModPlayer<SkillTreePlayer>();

                    Console.WriteLine(tryClickNode.IsUnlockable() + " - " + (STP.SkillPoints > 0));

                    if (tryClickNode.IsUnlockable() && STP.SkillPoints > 0)
                    {
                        STP.GetSkill(tryClickNode);
                    }
                }
            }
            else
                MouseIsDown = false;


            base.PreUpdatePlayers();
        }

        public Rectangle GetTreeRectangle()
        {
            return new Rectangle(Main.screenWidth / 4, Main.screenHeight / 4, Main.screenWidth / 2, Main.screenHeight / 2);
        }

        public override void PostUpdatePlayers()
        {
            base.PostUpdatePlayers();
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (!IsSkillTreeActive)
                return;

            int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));

            if (resourceBarIndex != -1)
            {
                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "Test: Displaying Skill Tree",
                    delegate
                    {
                        SkillTreeUI(Main.spriteBatch);
                        /*
                        if (recasting != 0f)
                            UIDrawReCasting(Main.spriteBatch);
                        else
                            UIDrawPicking(Main.spriteBatch);
                        */

                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }

            /*
            Console.WriteLine("START");
            foreach (var layer in layers)
            {
                Console.WriteLine(layer.Name);  
            }
            Console.WriteLine("END");
            */
        }

        private static readonly RasterizerState OverflowHiddenRasterizerState = new RasterizerState
        {
            CullMode = CullMode.None,
            ScissorTestEnable = true
        };
        private static Texture2D Pixel = ModContent.Request<Texture2D>("Divergency/Assets/Textures/WhitePixel", AssetRequestMode.ImmediateLoad).Value;
        private void SkillTreeUI(SpriteBatch spriteBatch) // also updates
        {
            Vector2 Center = Main.ScreenSize.ToVector2() / 2f - offset;

            Rectangle clippingRectangle = GetTreeRectangle();
            spriteBatch.Draw(Pixel, clippingRectangle, Color.SaddleBrown);

            /*
            Rectangle rec = new Rectangle(0, 0, size, size);
            Vector2 origin = new Vector2(size/2f, size/2f);
            */

            RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
            Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;

            Vector2 mousePos = new Vector2(Main.mouseX, Main.mouseY);
            SkillNode curHover = SkillNode.GetSkillMousedOver(Center, mousePos);

            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = clippingRectangle;
            spriteBatch.GraphicsDevice.RasterizerState = OverflowHiddenRasterizerState;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, OverflowHiddenRasterizerState);

            SkillNode.DrawLines(spriteBatch, Center);
            SkillNode.DrawSkills(spriteBatch, Center);

            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
            spriteBatch.GraphicsDevice.RasterizerState = rasterizerState;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);

            if (curHover != null)
                curHover.DrawDesc(spriteBatch, mousePos);

            /*
            // draw lines
            for (int i = 0; i < Skills.Count; i++)
            {
                SkillNode Skill = Skills[i];
                Vector2 Pos = Center + Skill.PosCenter - origin;

                foreach (int req in SkillReq[i])
                {
                    Vector2 otherPos = Center + Skills[req].PosCenter - origin;

                    int xS = (int)(Pos - otherPos).Length();
                    int yS = size / 8;

                    Vector2 lineSize = new Vector2(xS, yS);

                    spriteBatch.Draw(Pixel, (Pos + otherPos) / 2f - lineSize / 2 + origin, new Rectangle(0, 0, xS, yS), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            }

            // draw lines
            for (int i = 0; i < Skills.Count; i++)
            {
                SkillNode Skill = Skills[i];
                Vector2 Pos = Center + Skill.PosCenter - origin;

                spriteBatch.Draw(Pixel, Pos, rec, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            */
        }

        /*
        private void UIDrawReCasting(SpriteBatch spriteBatch) // also updates
        {
            float percent = (float)recasting / recastTime;
            float movement = MathF.Min(1f, (-percent + 1f) * 2f);

            Texture2D Options = (Texture2D)ModContent.Request<Texture2D>("Testing/Options");
            int iWidth = Options.Width;
            float scale = 2f * movement;
            Vector2 origin = new Vector2(iWidth / 2, iWidth / 2);

            spriteBatch.Draw(Options, Main.ScreenSize.ToVector2() / 2f - new Vector2(0, 64) * movement, new Rectangle(0, iWidth * picked.First(), iWidth, iWidth), Color.White, 0, origin, scale, SpriteEffects.None, 0);

            recasting -= 1;

            if (recasting == 0)
            {
                picked.RemoveAt(0);

                if (picked.Count == 0)
                {
                    picking = false;
                    return;
                }

                recasting = recastTime;
            }
        }

        private void UIDrawPicking(SpriteBatch spriteBatch) // also updates
        {
            Texture2D Options = (Texture2D)ModContent.Request<Texture2D>("Testing/Options");
            Texture2D OptionLine = (Texture2D)ModContent.Request<Texture2D>("Testing/OptionLine");

            Vector2 position = pickCenter;

            int iWidth = Options.Width;
            float scale = 2f;
            float halfWidth = iWidth / (2f) * scale;

            Vector2 origin = new Vector2(iWidth / 2, iWidth / 2);

            float P2 = (MathF.PI * 2);

            /*
            if (picked.Count != 0)
            {
                float rotation = P2 / optionCount * picked[0];
                Vector2 Offset = rotation.ToRotationVector2() * 80f;
                position += Offset;
            }
            *

            Rectangle lineRect = new Rectangle(0, 0, OptionLine.Width, (int)(optionDistance / 2f + OptionLine.Width / 2f));
            Vector2 lineOrigin = new Vector2((int)(OptionLine.Width / 2f), (int)((optionDistance / 2f + OptionLine.Width / 2f) / 2));

            for (int i = 0; i < picked.Count; i++)
            {
                int p = picked[i];

                float rotation = P2 / optionCount * p;
                Vector2 Offset = rotation.ToRotationVector2() * optionDistance;

                if (i != 0)
                    spriteBatch.Draw(OptionLine, position + Offset / 2, lineRect, Color.Wheat, rotation + MathF.PI / 2f, lineOrigin, scale, SpriteEffects.None, 0);

                position += Offset;
            }

            position = pickCenter;
            for (int i = 0; i < picked.Count; i++)
            {
                int p = picked[i];

                float rotation = P2 / optionCount * p;
                Vector2 Offset = rotation.ToRotationVector2() * optionDistance;

                position += Offset;

                spriteBatch.Draw(Options, position, new Rectangle(0, iWidth * p, iWidth, iWidth), Color.White, 0, origin, scale, SpriteEffects.None, 0);
            }

            for (int i = 0; i < optionCount; i++)
            {
                float rotation = P2 / optionCount * i;

                Vector2 Offset = rotation.ToRotationVector2() * optionDistance;

                Vector2 nPos = position + Offset;

                spriteBatch.Draw(Options, nPos, new Rectangle(0, iWidth * i, iWidth, iWidth), Color.White, 0, origin, scale, SpriteEffects.None, 0);

                /*
                // box
                if (nPos.X - halfWidth < Main.mouseX && nPos.X + halfWidth > Main.mouseX && nPos.Y - halfWidth < Main.mouseY && nPos.Y + halfWidth > Main.mouseY)
                {
                    Console.WriteLine(i + "- Box");
                }
                *

                // circle
                if ((nPos - Main.MouseScreen).Length() < halfWidth)
                {
                    //Console.WriteLine(i + "- Circle");

                    if (picked.Count > 0 && picked.Last() != i)
                        picked.Add(i);
                    else if (picked.Count == 0)
                        picked.Add(i);
                }
            }
        }
        */
    }
}