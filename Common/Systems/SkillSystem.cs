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
using Terraria.GameContent;
using Divergency.Common.Helpers;
using System.Reflection;
using System.Drawing.Imaging;
using Humanizer;
using Terraria.Initializers;
using Divergency.Content.Tiles.LivingGrove;
using Terraria.Utilities;
using System.Text;
using Terraria.GameContent.Items;
using Terraria.ModLoader.IO;
using System.CodeDom;

namespace Divergency.Common.Systems
{
    struct SkillInvocation
    {
        public MethodInfo method;
        public object skillObject;

        public SkillInvocation(MethodInfo method, object skillObject)
        {
            this.skillObject = skillObject;
            this.method = method;
        }

        public void Invoke(object[] paramsIn)
        {
            method.Invoke(skillObject, paramsIn);
        }
    }

    public class SkillTreePlayer : ModPlayer
    {
        private int SkillPointTotal = 100; // set this to 0 when it out, 100 for testing...
        private int SkillPointsUsed = 0;
        private string SaveString = "";
        private float d = 46f;

        public SkillNode CoreSkill;

        public int SkillPoints
        {
            get { return SkillPointTotal - SkillPointsUsed; }
        }

        Dictionary<string, List<SkillInvocation>> skills = new Dictionary<string, List<SkillInvocation>>();

        public void GetSkill(SkillNode skill)
        {
            skill.Learned = true;
            SkillPointsUsed++;

            MethodInfo[] methods = skill.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                  .Where(method => !method.Name.StartsWith("get_") && !method.Name.StartsWith("set_")).ToArray();

            foreach (MethodInfo method in methods)
            {
                if (method.MemberType == MemberTypes.Method)
                {
                    if (!skills.ContainsKey(method.Name))
                        skills.Add(method.Name, new List<SkillInvocation>());

                    skills[method.Name].Add(new SkillInvocation(method, skill));
                }
            }
        }

        public void TryCall(string name, object[] paramsIn)
        {
            List<SkillInvocation> skillInvocations;
            if (!skills.TryGetValue(name, out skillInvocations))
                return;

            foreach (SkillInvocation skillInvocaion in skillInvocations)
            {
                skillInvocaion.Invoke(paramsIn);
            }
        }

        // public override void PreUpdate() { TryCall("PreUpdate", new object[] { Player }); }

        public override void UpdateEquips() { TryCall("UpdateEquips", new object[] { Player }); }
        // so you can do ^that^ to add override for any functions, basically... 
 
        public override void SaveData(TagCompound tag)
        {
            string saveString = "";
            foreach (SkillNode skill in SkillNode.AllSkills)
            {
                saveString += skill.Learned ? "1" : "0";
            }

            tag.Add("SkillNodeLearnedString", saveString);
            tag.Add("SkillPointTotal", SkillPointTotal);
            tag.Add("SkillPointsUsed", SkillPointsUsed);
        }

        public override void OnEnterWorld()
        {
            SetupTree();

            if (SaveString.Length == SkillNode.AllSkills.Count)
            {
                for (int i = 0; i < SkillNode.AllSkills.Count; i++)
                {
                    if (SkillNode.AllSkills[i].Learned == false && SaveString[i] == '1')
                    {
                        GetSkill(SkillNode.AllSkills[i]);
                    }
                }
            }
        }

        public override void LoadData(TagCompound tag)
        {
            SaveString = tag.GetString("SkillNodeLearnedString");
            SkillPointTotal = tag.GetInt("SkillPointTotal");
            SkillPointsUsed = tag.GetInt("SkillPointsUsed");

            SkillTreeSystem.IsSkillTreeActive = false; // idk where this should be...
        }

        public void SetupTree()
        {
            SkillNode.ClearAll();

            CoreSkill = new TestSkill();
            CoreSkill.Learned = true;

            // make base tree branches
            SkillTreeHelper STH = new SkillTreeHelper(CoreSkill);
            STH.SetLabel("Core");

            STH.BaseRotation = MathF.PI / 2f * 0f;
            SetupBranch(STH, "Magic");

            STH.BaseRotation = MathF.PI / 2f * 1f;
            SetupBranch(STH, "Ranged");

            STH.BaseRotation = MathF.PI / 2f * 2f;
            SetupBranch(STH, "Melee");

            STH.BaseRotation = MathF.PI / 2f * 3f;
            SetupBranch(STH, "Summon");

            // make "shared" notes

            SkillNode[] skillOneLinks = new SkillNode[] { new BlankSkill(), new BlankSkill(), new BlankSkill(), new BlankSkill() };
            SkillNode[] statNodeInBetween = new SkillNode[] { new BlankSkill(), new BlankSkill(), new BlankSkill(), new BlankSkill() };
            SkillNode[] skillTwoLinks = new SkillNode[] { new BlankSkill(), new BlankSkill(), new BlankSkill(), new BlankSkill() };

            string[][] exitNames = new string[][]
            {
                new string[] { "TopExitMagic", "BotExitSummon" },
                new string[] { "TopExitRanged", "BotExitMagic" },
                new string[] { "TopExitMelee", "BotExitRanged" },
                new string[] { "TopExitSummon", "BotExitMelee" },
            };

            string[][] links = new string[][]
            {
                new string[] { "P1Magic", "P2Magic", "P3Summon", "P4Summon", "P5Magic" , "P6Summon" },
                new string[] { "P3Magic", "P4Magic", "P1Ranged", "P2Ranged", "P6Magic" , "P5Ranged" },
                new string[] { "P1Melee", "P2Melee", "P3Ranged", "P4Ranged", "P5Melee", "P6Ranged" },
                new string[] { "P3Melee", "P4Melee", "P1Summon", "P2Summon", "P6Melee", "P5Summon" },
            };

            for (int linkIDX = 0; linkIDX < links.Length; linkIDX++)
            {
                string[] link = links[linkIDX];

                SkillNode s1 = STH.GetSaved(link[0]);
                SkillNode s2 = STH.GetSaved(link[1]);
                SkillNode s3 = STH.GetSaved(link[2]);
                SkillNode s4 = STH.GetSaved(link[3]);

                SkillNode sm1 = skillOneLinks[linkIDX];
                sm1.position = new Vector2(((s1.position + s2.position) / 2f).X, ((s3.position + s4.position) / 2f).Y);

                foreach (string linkName in link)
                {
                    sm1.Connect(STH.GetSaved(linkName));
                }

                SkillNode s5 = STH.GetSaved(link[4]);
                SkillNode s6 = STH.GetSaved(link[5]);

                Vector2 center = (s5.position + s6.position) / 2f;
                center = center + (center - sm1.position);

                SkillNode sm2 = statNodeInBetween[linkIDX];
                sm2.position = center;

                sm2.Connect(s5);
                sm2.Connect(s6);

                SkillNode sm3 = skillTwoLinks[linkIDX];
                sm3.position = Vector2.Normalize(sm2.position) * STH.GetSaved("LS").position.Length();

                sm3.Connect(sm2);

                SkillTreeHelper STHTmp = new SkillTreeHelper(sm3);
                STHTmp.BaseRotation = sm3.position.ToRotation();

                STHTmp.AddNSet(new BlankSkill(), (-MathF.PI / 4f), d);
                STHTmp.AddNSet(new BlankSkill(), (-MathF.PI / 4f * 2f), d);
                STHTmp.SaveAs("BotEqu");

                STHTmp.TrackeBack(2);

                STHTmp.AddNSet(new BlankSkill(), (MathF.PI / 4f), d);
                STHTmp.AddNSet(new BlankSkill(), (MathF.PI / 4f * 2f), d);
                STHTmp.SaveAs("TopEqu");

                SkillNode ExitNodeBot = new BlankSkill(); // supposed to be exit node ofc;

                SkillNode exb1 = STH.GetSaved(exitNames[linkIDX][1]);
                SkillNode exb2 = STHTmp.GetSaved("BotEqu");

                Vector2 exitBot = (exb1.position + exb2.position) / 2f;
                exitBot = exitBot + Vector2.Normalize(exitBot) * d; // maby half?
                ExitNodeBot.position = exitBot;

                ExitNodeBot.Connect(exb1);
                ExitNodeBot.Connect(exb2);


                SkillNode ExitNodeTop = new BlankSkill(); // supposed to be exit node ofc;

                SkillNode ext1 = STH.GetSaved(exitNames[linkIDX][0]);
                SkillNode ext2 = STHTmp.GetSaved("TopEqu");

                Vector2 exitTop = (ext1.position + ext2.position) / 2f;
                exitTop = exitTop + Vector2.Normalize(exitTop) * d; // maby half?
                ExitNodeTop.position = exitTop;

                ExitNodeTop.Connect(ext1);
                ExitNodeTop.Connect(ext2);
            }
        }

        private void SetupBranch(SkillTreeHelper STH, string branchName)
        {
            BlankSkill._idx = 0;
            STH.GOTOLabel("Core");

            STH.AddNSet(new BlankSkill(), 0f, d);
            STH.AddNSet(new BlankSkill(), -MathF.PI / 4f, d);
            STH.SaveAs("tmp");

            STH.AddNSet(new BlankSkill(), -MathF.PI / 8f * 3f, d * 0.8f);
            STH.SaveAs("P1" + branchName);

            STH.TrackeBack(2);
            STH.AddNSet(new BlankSkill(), MathF.PI / 4f, d);

            STH.AddNSet(new BlankSkill(), MathF.PI / 8f * 3f, d * 0.8f);
            STH.SaveAs("P3" + branchName);
            STH.TrackeBack(1);

            STH.AddNSet(new BlankSkill(), -MathF.PI / 4f, d);
            STH.Connect("tmp");

            STH.AddNSet(new BlankSkill(), -MathF.PI / 4f, d);
            STH.SaveAs("tmp");

            STH.AddNSet(new BlankSkill(), -MathF.PI / 8f * 5f, d * 0.8f);
            STH.SaveAs("P2" + branchName);

            STH.TrackeBack(2);
            STH.AddNSet(new BlankSkill(), MathF.PI / 4f, d);

            STH.AddNSet(new BlankSkill(), MathF.PI / 8f * 5f, d * 0.8f);
            STH.SaveAs("P4" + branchName);
            STH.TrackeBack(1);

            STH.AddNSet(new BlankSkill(), -MathF.PI / 4f, d);
            STH.Connect("tmp");

            STH.AddNSet(new BlankSkill(), MathF.PI / 4f, d);

            STH.AddNSet(new BlankSkill(), MathF.PI * 0.45f, d * 1.2f);
            STH.AddNSet(new BlankSkill(), MathF.PI * 0.9f, d * 1.2f);
            STH.SaveAs("P6" + branchName);

            STH.TrackeBack(2);

            STH.AddNSet(new BlankSkill(), MathF.PI / 8f, d);
            STH.SaveAs("tmp");

            STH.TrackeBack(2);

            STH.AddNSet(new BlankSkill(), -MathF.PI / 4f, d);

            STH.AddNSet(new BlankSkill(), -MathF.PI * 0.45f, d * 1.2f);
            STH.AddNSet(new BlankSkill(), -MathF.PI * 0.9f, d * 1.2f);
            STH.SaveAs("P5" + branchName);

            STH.TrackeBack(2);

            STH.AddNSet(new BlankSkill(), -MathF.PI / 8f, d);

            float tmp = ((MathF.PI / 4f).ToRotationVector2().Y + (MathF.PI / 8f).ToRotationVector2().Y) * d;

            STH.AddNSet(new BlankSkill(), new Vector2(MathF.Cos(MathF.PI / 8f * 3f) * tmp, tmp));
            STH.SaveAs("LS");
            STH.Connect("tmp");

            STH.AddNSet(new BlankSkill(), (-MathF.PI / 4f), d);
            STH.AddNSet(new BlankSkill(), (-MathF.PI / 4f * 2f), d);
            STH.SaveAs("TopExit" + branchName);

            STH.TrackeBack(2);

            STH.AddNSet(new BlankSkill(), (MathF.PI / 4f), d);
            STH.AddNSet(new BlankSkill(), (MathF.PI / 4f * 2f), d);
            STH.SaveAs("BotExit" + branchName);
        }
    }

    public class SkillTreeHelper
    {
        SkillNode curNode;

        List<SkillNode> nodeHistory = new List<SkillNode>();

        Dictionary<string, int> label = new Dictionary<string, int>();

        Dictionary<string, SkillNode> savedAs = new Dictionary<string, SkillNode>();

        public float BaseRotation = 0f;

        public SkillTreeHelper(SkillNode curNode)
        {
            this.curNode = curNode;
            nodeHistory.Add(curNode);
        }

        public void AddNSet(SkillNode skill, float rotation, float distance)
        {
            nodeHistory.Add(skill);
            curNode = curNode.AddBS(skill, (rotation + BaseRotation).ToRotationVector2() * distance);
        }

        public void AddNSet(SkillNode skill, float rotation)
        {
            nodeHistory.Add(skill);
            curNode = curNode.AddBS(skill, rotation + BaseRotation);
        }

        public void AddNSet(SkillNode skill, Vector2 skillOffset)
        {
            nodeHistory.Add(skill);
            curNode = curNode.AddBS(skill, skillOffset.RotatedBy(BaseRotation));
        }

        public void Connect(string s)
        {
            savedAs[s].Connect(curNode);
        }

        public SkillNode GetSaved(string s)
        {
            return savedAs[s];
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
            if (label.ContainsKey(s))
                label[s] = nodeHistory.Count;
            else
                label.Add(s, nodeHistory.Count);
        }

        public void SaveAs(string s)
        {
            if (savedAs.ContainsKey(s))
                savedAs[s] = curNode;
            else
                savedAs.Add(s, curNode);
        }
    }

    public class SkillTreeSystem : ModSystem
    {
        public List<List<int>> SkillReq = new List<List<int>>();
        public List<SkillNode> Skills = new List<SkillNode>();

        public static ModKeybind ToggleSkillTree { get; private set; }

        public static bool IsSkillTreeActive = false;
        public Vector2 offset;
        public Vector2 prevMousePos;
        public SkillNode tryClickNode;
        public bool MouseIsDown;

        // private int zoom = 0;

        public override void Load()
        {
            /*
            d = 46f * 1.8f;
            SkillNode.SkillTreeScale = 1.2f;
            */

            ToggleSkillTree = KeybindLoader.RegisterKeybind(Mod, "Toggle Skill Tree", "P");
        }

        public override void Unload()
        {
            ToggleSkillTree = null;
        }

        public override void PreUpdatePlayers()
        {
            Player player = Main.LocalPlayer;
            SkillTreePlayer STP = Main.LocalPlayer.GetModPlayer<SkillTreePlayer>();
            SkillNode CoreSkill = STP.CoreSkill;

            Vector2 mouseVec = new Vector2(Main.mouseX, Main.mouseY);
            if (ToggleSkillTree.JustPressed)
            {
                IsSkillTreeActive = !IsSkillTreeActive;
                offset = new Vector2();
                SkillNode.SkillTreeScale = 1f;
            }

            if (IsSkillTreeActive && GetTreeRectangle().Contains(Main.mouseX, Main.mouseY))
            {
                Main.blockMouse = true;
                // Main.hoverSomething...
            }

            if (Main.mouseLeft && Main.mouseLeftRelease && GetTreeRectangle().Contains(Main.mouseX, Main.mouseY))
            {
                MouseIsDown = true;
                prevMousePos = mouseVec;

                Vector2 Center = Main.ScreenSize.ToVector2() / 2f - offset;
                tryClickNode = SkillNode.GetSkillMousedOver(Center, mouseVec);
            }

            if (Main.mouseLeft && MouseIsDown)
            {
                if (tryClickNode == null)
                {
                    Vector2 newMousePos = mouseVec;

                    offset = offset + (prevMousePos - newMousePos);
                    prevMousePos = newMousePos;
                }
            }
            else if (!Main.mouseLeft && MouseIsDown)
            {
                MouseIsDown = false;

                Vector2 Center = Main.ScreenSize.ToVector2() / 2f - offset;
                SkillNode curHover = SkillNode.GetSkillMousedOver(Center, mouseVec);

                if (tryClickNode != null && curHover == tryClickNode)
                {
                    Console.WriteLine(STP.SkillPoints);

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
            spriteBatch.Draw(Pixel, clippingRectangle, Color.Gray);

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