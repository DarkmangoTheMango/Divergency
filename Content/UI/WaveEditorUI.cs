using Divergency.Common.Helpers;
using Divergency.Content.Events.LivingCore;
using Divergency.Tiles.LivingTree;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.UI.Elements.Base;
using ReLogic.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace Divergency.Content.UI
{
    internal class WaveEditorUI : ModSystem
    {
        internal UserInterface InspectorInterface;
        internal WaveEditorUIState InspectorState;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                InspectorInterface = new UserInterface();
                InspectorState = new WaveEditorUIState();
                InspectorState.Activate();
            }
        }

        public void OpenMenu(LivingCoreAltarTileEntity target)
        {
            InspectorState.TargetObject = target;
            InspectorInterface.SetState(InspectorState);
            InspectorState.RefreshUI();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (InspectorInterface?.CurrentState != null)
            {
                InspectorInterface.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Divergency: Wave Editor",
                    delegate {
                        InspectorInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
    public class InnerUI : UIElement { public override bool ContainsPoint(Vector2 point) { return true; } }
    // makes you not be able to click - but at least it makes scrolling stuff work...
    
    public class WaveEditorUIState : UIState
    {
        public LivingCoreAltarTileEntity TargetObject;
        private UIPanel mainPanel;
        private UIPanel tabButtonPanel;
        private InnerUI innerList;
        private UIPanel contentPanel;
        private int currentWaveIndex = 0;

        private UIPanel rewards;
        private InnerUI rewardsInnerList;

        private bool isDraggingPanel = false;
        private Vector2 dragOffset;
        private UIPanel dragHandle;

        private int draggingNpc = -1;

        private string _tempText = "";
        private bool _isEditing = false;

        private float targetScroll = 0f;
        private float currentScroll = 0f;
        private float maxScroll = 1000f;

        private float rewardsTargetScroll = 0f;
        private float rewardsCurrentScroll = 0f;
        private float rewardsMaxScroll = 1000f;

        private Item[] items = [];
        private UIPanel[] panels = [];

        private Action[] entityPositionChange = [];

        public override void OnInitialize()
        {
            mainPanel = new UIPanel();
            mainPanel.Width.Set(500, 0);
            mainPanel.Height.Set(480, 0);
            mainPanel.HAlign = 0.5f;
            mainPanel.VAlign = 0.5f;
            mainPanel.BackgroundColor = new Color(73, 94, 171) * 0.9f;

            rewards = new UIPanel();
            rewards.Width.Set(0, 1f);
            rewards.Height.Set(60, 0);
            rewards.Top.Set(-60f, 1f);
            rewards.BackgroundColor = new Color(73, 94, 171) * 0.9f;
            rewards.OverflowHidden = true;
            mainPanel.Append(rewards);

            rewards.OnScrollWheel += (evt, element) => { // block scrolling somehow
                rewardsTargetScroll -= evt.ScrollWheelValue;

                rewardsTargetScroll = MathHelper.Clamp(rewardsTargetScroll, 0, rewardsMaxScroll);
            };

            rewardsInnerList = new InnerUI();
            rewardsInnerList.Width.Set(0, 1f);
            rewardsInnerList.Height.Set(0, 1f);
            rewards.Append(rewardsInnerList);

            rewardsInnerList.OnUpdate += (element) => {
                rewardsCurrentScroll = MathHelper.Lerp(rewardsCurrentScroll, rewardsTargetScroll, 0.15f);
                element.Left.Set(-rewardsCurrentScroll, 0);
            };


            rewardsInnerList.OnDraw += (_) =>
            {
                SpriteBatch spriteBatch = Main.spriteBatch;

                for (int i = 0; i < items.Length; i++)
                {
                    CalculatedStyle dims = panels[i].GetDimensions();
                    Vector2 slotPos = dims.Position();

                    Main.LocalPlayer.mouseInterface = true;
                    if (i >= TargetObject.Rewards.Count)
                    {
                        ItemSlot.Draw(spriteBatch, ref items[i], ItemSlot.Context.EquipAccessory, slotPos);
                        continue;
                    }

                    ItemSlot.Draw(spriteBatch, ref items[i], !TargetObject.ClaimedRewards[i] ? ItemSlot.Context.EquipAccessory : ItemSlot.Context.ChestItem, slotPos);
                }

                RasterizerState originalState = spriteBatch.GraphicsDevice.RasterizerState;
                Rectangle originalScissor = spriteBatch.GraphicsDevice.ScissorRectangle;
                spriteBatch.End();

                spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
                spriteBatch.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);

                for (int i = 0; i < items.Length; i++)
                {
                    CalculatedStyle dims = panels[i].GetDimensions();
                    Vector2 slotPos = dims.Position() + new Vector2(dims.Width, dims.Height) / 2f;

                    if (i >= TargetObject.Rewards.Count)
                        continue;
                    
                    if (dims.ToRectangle().Contains(Main.mouseX, Main.mouseY) && ModContent.RequestIfExists<Texture2D>(TargetObject.Rewards[i].texturePath, out var texture))
                    {
                        Rectangle rect2 = new(0, 0, texture.Value.Width, texture.Value.Height);
                        Main.spriteBatch.Draw(texture.Value, slotPos + new Vector2(0, 48 + texture.Value.Height / 2f), rect2, Color.White, 0f, new Vector2(texture.Value.Width, texture.Value.Height) / 2f, 1f, SpriteEffects.None, 0);
                    }
                }

                spriteBatch.End();

                spriteBatch.GraphicsDevice.ScissorRectangle = originalScissor;
                spriteBatch.GraphicsDevice.RasterizerState = originalState;

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, originalState, null, Main.UIScaleMatrix);
            };

            UIText title = new UIText("Wave Editor", 1f);
            title.HAlign = 0f;
            title.VAlign = 0f;
            title.Left.Set(10, 0);
            title.Top.Set(5, 0);
            mainPanel.Append(title);

            dragHandle = new UIPanel();
            dragHandle.Width.Set(0, 1f);
            dragHandle.Height.Set(30, 0);
            dragHandle.BackgroundColor = Color.Transparent;
            dragHandle.BorderColor = Color.Transparent;
            mainPanel.Append(dragHandle);

            UIText closeText = new UIText("X", 1.2f);
            closeText.HAlign = 1f;
            closeText.VAlign = 0f;
            closeText.Left.Set(-10, 0);
            closeText.Top.Set(5, 0);
            closeText.OnLeftClick += (evt, element) =>
            {
                ModContent.GetInstance<WaveEditorUI>().InspectorInterface.SetState(null);
            };

            mainPanel.Append(closeText);

            tabButtonPanel = new UIPanel();
            tabButtonPanel.Top.Set(30, 0);
            tabButtonPanel.Width.Set(0, 1f);
            tabButtonPanel.Height.Set(40, 0);
            tabButtonPanel.OverflowHidden = true;

            tabButtonPanel.OnScrollWheel += (evt, element) => { // block scrolling somehow
                targetScroll -= evt.ScrollWheelValue;

                targetScroll = MathHelper.Clamp(targetScroll, 0, maxScroll);
            };

            mainPanel.Append(tabButtonPanel);

            innerList = new InnerUI();
            innerList.Width.Set(0, 1f);
            innerList.Height.Set(0, 1f);
            tabButtonPanel.Append(innerList);

            innerList.OnUpdate += (element) => {
                currentScroll = MathHelper.Lerp(currentScroll, targetScroll, 0.15f);

                element.Left.Set(-currentScroll, 0);
            };

            contentPanel = new UIPanel();
            contentPanel.Width.Set(-20, 1f);
            contentPanel.Height.Set(-95 - 80, 1f);
            contentPanel.Top.Set(85, 0);
            contentPanel.Left.Set(10, 0);
            contentPanel.BackgroundColor = new Color(33, 43, 70) * 0.8f;
            mainPanel.Append(contentPanel);

            UIText addWaveButton = new UIText("[+] Add Wave", 0.9f);
            addWaveButton.Top.Set(-10 - 80, 1f);
            addWaveButton.Left.Set(-10, 0);
            addWaveButton.OnLeftClick += (evt, element) => AddNewWave();
            mainPanel.Append(addWaveButton);

            Append(mainPanel);
        }

        public void RefreshUI()
        {
            draggingNpc = -1;
            if (TargetObject == null) return;

            // update waves
            innerList.RemoveAllChildren();
            contentPanel.RemoveAllChildren();

            float currentLeft = 5;

            for (int i = 0; i < TargetObject.Waves.Count; i++)
            {
                int waveIndex = i;
                UIPanel tabButton = new UIPanel();
                tabButton.Width.Set(100, 0);
                tabButton.Height.Set(40, 0);
                tabButton.Left.Set(currentLeft, 0);
                currentLeft += 105;

                tabButton.Top.Set(0, 0);
                tabButton.BackgroundColor = waveIndex == currentWaveIndex ? new Color(100, 120, 200) : new Color(50, 65, 120);

                UIText tabText = new UIText($"Wave", 0.8f); //  {i + 1}
                tabText.HAlign = 0.5f;
                tabText.VAlign = 0.5f;
                tabButton.Append(tabText);

                tabButton.OnLeftClick += (evt, element) => SwitchToWave(waveIndex);
                tabButton.OnRightClick += (evt, element) =>
                {
                    TargetObject.Waves.RemoveAt(waveIndex);
                    RefreshUI();
                };

                innerList.Append(tabButton);
            }

            Console.WriteLine(currentWaveIndex);
            UIPanel wavePanel = CreateWavePanel(currentWaveIndex);
            contentPanel.Append(wavePanel);

            // innerList.Width.Set(currentLeft, 0f);

            maxScroll = Math.Max(0f, currentLeft - mainPanel.Width.Pixels + 45);
            targetScroll = MathHelper.Clamp(targetScroll, 0, maxScroll);

            // update rewards
            rewardsInnerList.RemoveAllChildren();
            items = new Item[TargetObject.Rewards.Count+1];
            panels = new UIPanel[TargetObject.Rewards.Count+1];

            currentLeft = 5;

            for (int i = 0; i < TargetObject.Rewards.Count + 1; i++)
            {
                UIPanel itemSlotPanel = new UIPanel();
                itemSlotPanel.Width.Set(32, 0);
                itemSlotPanel.Height.Set(52, 0);
                itemSlotPanel.Left.Set(currentLeft, 0);
                itemSlotPanel.BackgroundColor = Color.Transparent;
                itemSlotPanel.BorderColor = Color.Transparent;
                currentLeft += 40;

                items[i] = i < TargetObject.Rewards.Count ? ItemIO.Load(TargetObject.Rewards[i].item) : new Item();
                panels[i] = itemSlotPanel;

                int index = i;

                itemSlotPanel.OnLeftClick += (evt, element) => {
                    if (Main.LocalPlayer.HeldItem != null)
                    {
                        string clipboardText = Platform.Get<IClipboard>().Value;
                        if (!ModContent.RequestIfExists<Texture2D>(clipboardText, out _))
                        {
                            Console.WriteLine("Found, " + clipboardText + " in clipboard; not a valid texture.");
                            return;
                        }

                        if (index >= TargetObject.Rewards.Count)
                        {
                            TargetObject.Rewards.Add(new Reward(null, ""));
                            TargetObject.ClaimedRewards.Add(false);
                        }

                        TargetObject.Rewards[index].item = ItemIO.Save(Main.LocalPlayer.HeldItem);

                        TargetObject.Rewards[index].texturePath = clipboardText;

                        Console.WriteLine("As texture, pasted " + clipboardText + " from clipboard.");

                        RefreshUI();
                    }
                };

                itemSlotPanel.OnRightClick += (evt, element) => {
                    if (index >= TargetObject.Rewards.Count)
                        return;

                    TargetObject.Rewards.RemoveAt(index);
                    TargetObject.ClaimedRewards.RemoveAt(index);
                    RefreshUI();
                };

                itemSlotPanel.OnMiddleClick += (evt, element) => {
                    if (index >= TargetObject.Rewards.Count)
                        return;

                    TargetObject.ClaimedRewards[index] = !TargetObject.ClaimedRewards[index];
                    RefreshUI();
                };

                rewardsInnerList.Append(itemSlotPanel);
            }

            // rewardsInnerList.Width.Set(currentLeft, 0f);

            rewardsMaxScroll = Math.Max(0f, currentLeft - mainPanel.Width.Pixels + 45);
            rewardsTargetScroll = MathHelper.Clamp(rewardsTargetScroll, 0, rewardsMaxScroll);
        }

        private UIPanel CreateWavePanel(int waveIndex)
        {
            UIPanel panel = new UIPanel();
            panel.Width.Set(0, 1f);
            panel.Height.Set(0, 1f);
            panel.BackgroundColor = Color.Transparent;
            panel.BorderColor = Color.Transparent;

            if (TargetObject == null || waveIndex >= TargetObject.Waves.Count) return panel;

            var wave = TargetObject.Waves[waveIndex];

            UIList entityList = new UIList();
            entityList.Width.Set(-15, 1f);
            entityList.Height.Set(-50, 1f);
            entityList.Top.Set(30, 0);
            entityList.Left.Set(-10, 0);
            entityList.ListPadding = 5f;
            panel.Append(entityList);

            UIScrollbar scrollbar = new UIScrollbar();
            scrollbar.SetView(100f, 1000f);
            scrollbar.Height.Set(-50, 1f);
            scrollbar.Top.Set(30, 0);
            scrollbar.HAlign = 1f;
            panel.Append(scrollbar);
            entityList.SetScrollbar(scrollbar);

            UIText header = new UIText("Entities in this wave:", 0.9f);
            header.Top.Set(-5f, 0f);
            header.Left.Set(-5f, 0f);
            panel.Append(header);

            UIText addEntityButton = new UIText("[+] Add Entity", 0.85f);
            addEntityButton.Top.Set(0, 1f);
            addEntityButton.Left.Set(-20, 0f);
            addEntityButton.OnLeftClick += (evt, element) => AddEntityToWave(waveIndex);
            panel.Append(addEntityButton);

            entityPositionChange = new Action[wave.enemies.Count];
            for (int i = 0; i < wave.enemies.Count; i++)
            {
                int entityIndex = i;
                UIPanel entityItem = CreateEntityItem(waveIndex, entityIndex);
                entityList.Add(entityItem);
            }

            return panel;
        }

        Instance _editingEntity;

        private UIPanel CreateEntityItem(int waveIndex, int entityIndex)
        {
            var entity = TargetObject.Waves[waveIndex].enemies[entityIndex];

            UIPanel item = new UIPanel();
            item.Width.Set(0, 1f);
            item.Height.Set(80, 0);
            item.BackgroundColor = new Color(44, 57, 105) * 0.9f;

            UISearchBar nameInput = new UISearchBar(Terraria.Localization.Language.GetText(""), 0.8f);
            nameInput.Width.Set(0, 1f);
            nameInput.Height.Set(30, 0);
            nameInput.Top.Set(-5, 0);
            nameInput.Left.Set(-5, 0);

            nameInput.SetContents(entity.FullName);

            nameInput.OnContentsChanged += (text) => {
                entity.FullName = text;
            };

            nameInput.OnLeftClick += (evt, element) => {
                nameInput.ToggleTakingText();
                Main.clrInput();
            };

            item.Append(nameInput);

            UIText offsetXLabel = new UIText($"Offset X: {entity.SpawnOffset.X}", 0.75f);
            offsetXLabel.Top.Set(25, 0);
            offsetXLabel.Left.Set(5, 0);
            item.Append(offsetXLabel);

            UIText offsetXMinus = new UIText("[-]", 0.75f);
            offsetXMinus.Top.Set(25, 0);
            offsetXMinus.Left.Set(120, 0);
            offsetXMinus.OnLeftClick += (evt, element) => {
                entity.SpawnOffset.X -= 16;
                offsetXLabel.SetText($"Offset X: {entity.SpawnOffset.X}");
            };
            item.Append(offsetXMinus);

            UIText offsetXPlus = new UIText("[+]", 0.75f);
            offsetXPlus.Top.Set(25, 0);
            offsetXPlus.Left.Set(145, 0);
            offsetXPlus.OnLeftClick += (evt, element) => {
                entity.SpawnOffset.X += 16;
                offsetXLabel.SetText($"Offset X: {entity.SpawnOffset.X}");
            };
            item.Append(offsetXPlus);

            UIText offsetYLabel = new UIText($"Offset Y: {entity.SpawnOffset.Y}", 0.75f);
            offsetYLabel.Top.Set(45, 0);
            offsetYLabel.Left.Set(5, 0);
            item.Append(offsetYLabel);

            UIText offsetYMinus = new UIText("[-]", 0.75f);
            offsetYMinus.Top.Set(45, 0);
            offsetYMinus.Left.Set(120, 0);
            offsetYMinus.OnLeftClick += (evt, element) => {
                entity.SpawnOffset.Y -= 16;
                offsetYLabel.SetText($"Offset Y: {entity.SpawnOffset.Y}");
            };
            item.Append(offsetYMinus);

            UIText offsetYPlus = new UIText("[+]", 0.75f);
            offsetYPlus.Top.Set(45, 0);
            offsetYPlus.Left.Set(145, 0);
            offsetYPlus.OnLeftClick += (evt, element) => {
                entity.SpawnOffset.Y += 16;
                offsetYLabel.SetText($"Offset Y: {entity.SpawnOffset.Y}");
            };
            item.Append(offsetYPlus);

            entityPositionChange[entityIndex] = () =>
            {
                offsetXLabel.SetText($"Offset X: {entity.SpawnOffset.X}");
                offsetYLabel.SetText($"Offset Y: {entity.SpawnOffset.Y}");
            };

            UIText deleteButton = new UIText("[Remove]", 0.75f);
            deleteButton.Top.Set(45, 0);
            deleteButton.HAlign = 1f;
            deleteButton.Left.Set(-5, 0);
            deleteButton.OnLeftClick += (evt, element) => RemoveEntity(waveIndex, entityIndex);
            item.Append(deleteButton);

            return item;
        }

        private void SwitchToWave(int waveIndex)
        {
            currentWaveIndex = waveIndex;
            RefreshUI();
        }

        private void AddNewWave()
        {
            if (TargetObject == null) return;
            // Assuming Wave class exists with a constructor
            TargetObject.Waves.Add(new Wave("", []));
            currentWaveIndex = TargetObject.Waves.Count - 1;
            RefreshUI();
        }

        private void AddEntityToWave(int waveIndex)
        {
            if (TargetObject == null || waveIndex >= TargetObject.Waves.Count) return;
            // Assuming WaveEntity class exists
            TargetObject.Waves[waveIndex].enemies.Add(new Instance("Divergency/", Vector2.Zero));
            RefreshUI();
        }

        private void RemoveEntity(int waveIndex, int entityIndex)
        {
            if (TargetObject == null || waveIndex >= TargetObject.Waves.Count) return;
            if (entityIndex >= TargetObject.Waves[waveIndex].enemies.Count) return;
            TargetObject.Waves[waveIndex].enemies.RemoveAt(entityIndex);
            RefreshUI();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (TargetObject == null || currentWaveIndex >= TargetObject.Waves.Count) return;

            Vector2 tileWorldPosition = TargetObject.Position.ToWorldCoordinates();

            Texture2D pixel = TextureAssets.MagicPixel.Value;

            var wave = TargetObject.Waves[currentWaveIndex];

            bool allowPassthrough = draggingNpc == -1;
            for (int i = 0; i < wave.enemies.Count; i ++)
            {
                var entity = wave.enemies[i];
                Vector2 spawnWorldPos = tileWorldPosition + entity.SpawnOffset + new Vector2(16, 0);
                Vector2 screenPos = spawnWorldPos - Main.screenPosition;

                screenPos = Vector2.Transform(screenPos, Main.GameViewMatrix.TransformationMatrix);
                screenPos /= Main.UIScale;

                float scale = Main.GameViewMatrix.Zoom.X / Main.UIScale;

                Texture2D npcTexture = TextureAssets.MagicPixel.Value;
                int width = (int)(16 * scale);
                int height = (int)(16 * scale);

                if (entity.NPCID != -1)
                {
                    if (Int64.TryParse(entity.FullName, out long _)) // if vanilla (just a number) - then make sure it's loaded
                    {
                        Main.instance.LoadNPC(entity.NPCID);
                    }

                    npcTexture = TextureAssets.Npc[entity.NPCID].Value;
                    int frameCount = Main.npcFrameCount[entity.NPCID];
                    int frameHeight = npcTexture.Height / frameCount;
                    width = (int)(npcTexture.Width * scale);
                    height = (int)(frameHeight * scale);
                }

                Rectangle box = new Rectangle((int)screenPos.X - width / 2, (int)screenPos.Y - height / 2, width, height);
                Rectangle srcRect = new Rectangle(0, 0, width, height);

                spriteBatch.Draw(npcTexture, box, srcRect, Color.White);

                if (allowPassthrough && box.Contains(Main.mouseX, Main.mouseY) && Main.mouseMiddle)
                {
                    draggingNpc = i;
                }
            }

            foreach (Point16 vec in TargetObject.BlockingBlocks)
            {
                Vector2 removePosition = tileWorldPosition + vec.ToVector2() * 16f;
                Vector2 screenRemovePos = removePosition - Main.screenPosition;

                screenRemovePos = Vector2.Transform(screenRemovePos, Main.GameViewMatrix.TransformationMatrix);
                screenRemovePos /= Main.UIScale;

                float scale = Main.GameViewMatrix.Zoom.X / Main.UIScale;

                int width = (int)(18 * scale);
                int height = (int)(18 * scale);

                Rectangle box = new Rectangle((int)screenRemovePos.X - width / 2, (int)screenRemovePos.Y - height / 2, width, height);
                Rectangle srcRect = new Rectangle(0, 0, width, height);

                spriteBatch.Draw(TextureAssets.MagicPixel.Value, box, srcRect, Color.Red);
            }
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);

            // Check if clicking on drag handle (title bar area)
            if (dragHandle != null && dragHandle.IsMouseHovering)
            {
                isDraggingPanel = true;
                Vector2 mousePos = new Vector2(Main.mouseX, Main.mouseY);
                CalculatedStyle panelDimensions = mainPanel.GetDimensions();
                dragOffset = mousePos - new Vector2(panelDimensions.X, panelDimensions.Y);
            }
        }
        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            isDraggingPanel = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (isDraggingPanel)
            {
                Vector2 mousePos = new Vector2(Main.mouseX, Main.mouseY);
                Vector2 newPosition = mousePos - dragOffset;

                // Convert to relative position (0-1 range based on screen size)
                float relativeX = newPosition.X / Main.screenWidth;
                float relativeY = newPosition.Y / Main.screenHeight;

                // Clear the HAlign and VAlign to use absolute positioning
                mainPanel.Left.Set(newPosition.X, 0);
                mainPanel.Top.Set(newPosition.Y, 0);
                mainPanel.HAlign = 0f;
                mainPanel.VAlign = 0f;

                mainPanel.Recalculate();
            }

            if (Main.mouseMiddleRelease)
            {
                draggingNpc = -1;
            }

            if (draggingNpc != -1)
            {
                var wave = TargetObject.Waves[currentWaveIndex];
                var entity = wave.enemies[draggingNpc];

                entity.SpawnOffset = entity.SpawnOffset + new Vector2(Main.mouseX - Main.lastMouseX, Main.mouseY - Main.lastMouseY) + (Main.screenPosition - Main.screenLastPosition);

                entityPositionChange[draggingNpc]();
            }


            if (!KeybindSystem.BlockingBlocks.Current)
                return;

            Point16 vec = new Point16(Player.tileTargetX, Player.tileTargetY) - TargetObject.Position;

            if (Main.mouseLeft)
            {
                if (!TargetObject.BlockingBlocks.Any(v => v.X == vec.X && v.Y == vec.Y))
                {
                    TargetObject.BlockingBlocks.Add(vec);
                }
            }
            else if (Main.mouseRight)
            {
                TargetObject.BlockingBlocks.RemoveAll(v => v.X == vec.X && v.Y == vec.Y);
            }
        }
    }
}
