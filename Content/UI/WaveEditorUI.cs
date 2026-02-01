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
using System.Xml.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using System.Windows.Forms;

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
    public class WaveEditorUIState : UIState
    {
        public LivingCoreAltarTileEntity TargetObject;
        private UIPanel mainPanel;
        private UIPanel tabScrollContainer;
        private UIPanel tabButtonPanel;
        private UIElement innerList;
        private UIPanel contentPanel;
        private int currentWaveIndex = 0;
        private List<UIPanel> waveContentPanels = new List<UIPanel>();
        private float tabScrollOffset = 0f;

        private UIPanel rewards;
        private UIElement rewardsInnerList;

        // Panel dragging
        private bool isDraggingPanel = false;
        private Vector2 dragOffset;
        private UIPanel dragHandle;

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
            rewards.OverflowHidden = false;
            mainPanel.Append(rewards);

            rewards.OnScrollWheel += (evt, element) => { // block scrolling somehow
                rewardsTargetScroll -= evt.ScrollWheelValue;

                rewardsTargetScroll = MathHelper.Clamp(rewardsTargetScroll, 0, rewardsMaxScroll);
            };

            rewardsInnerList = new UIElement();
            rewardsInnerList.Width.Set(0, 1f);
            rewardsInnerList.Height.Set(0, 1f);
            rewardsInnerList.OverflowHidden = false;
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
                        ItemSlot.Draw(spriteBatch, ref items[i], ItemSlot.Context.ChestItem, slotPos);
                        continue;
                    }

                    ItemSlot.Draw(spriteBatch, ref items[i], !TargetObject.ClaimedRewards[i] ? ItemSlot.Context.ChestItem : ItemSlot.Context.BankItem, slotPos);
                }
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
                StopEditing();
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

            innerList = new UIElement();
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

        private void RewardsInnerList_OnDraw(UIElement affectedElement)
        {
            throw new NotImplementedException();
        }

        public void RefreshUI()
        {
            if (TargetObject == null) return;

            // update waves
            innerList.RemoveAllChildren();
            waveContentPanels.Clear();

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

                UIPanel wavePanel = CreateWavePanel(waveIndex);
                waveContentPanels.Add(wavePanel);
            }

            innerList.Width.Set(currentLeft, 0f);
            innerList.Recalculate();
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

            rewardsInnerList.Width.Set(currentLeft, 0f);
            rewardsInnerList.Recalculate();
            foreach (UIElement panel in rewardsInnerList.Children)
            {
                panel.Recalculate();
            }

            rewardsMaxScroll = Math.Max(0f, currentLeft - mainPanel.Width.Pixels + 45);
            rewardsTargetScroll = MathHelper.Clamp(rewardsTargetScroll, 0, rewardsMaxScroll);


            UpdateContentPanel();
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
                RefreshUI();
            };
            item.Append(offsetXMinus);

            UIText offsetXPlus = new UIText("[+]", 0.75f);
            offsetXPlus.Top.Set(25, 0);
            offsetXPlus.Left.Set(145, 0);
            offsetXPlus.OnLeftClick += (evt, element) => {
                entity.SpawnOffset.X += 16;
                RefreshUI();
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
                RefreshUI();
            };
            item.Append(offsetYMinus);

            UIText offsetYPlus = new UIText("[+]", 0.75f);
            offsetYPlus.Top.Set(45, 0);
            offsetYPlus.Left.Set(145, 0);
            offsetYPlus.OnLeftClick += (evt, element) => {
                entity.SpawnOffset.Y += 16;
                RefreshUI();
            };
            item.Append(offsetYPlus);

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

        private void UpdateContentPanel()
        {
            contentPanel.RemoveAllChildren();
            if (currentWaveIndex < waveContentPanels.Count)
            {
                contentPanel.Append(waveContentPanels[currentWaveIndex]);
            }
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

            // Draw world-space indicators for spawn positions
            if (TargetObject == null || currentWaveIndex >= TargetObject.Waves.Count) return;

            // Get tile entity position (you'll need to get this from TargetObject)
            // Assuming TargetObject has Position property or similar
            Vector2 tileWorldPosition = TargetObject.Position.ToWorldCoordinates() + new Vector2(0, 0); // Adjust this to your actual property

            Texture2D pixel = TextureAssets.MagicPixel.Value;

            var wave = TargetObject.Waves[currentWaveIndex];
            foreach (var entity in wave.enemies)
            {
                // 1. Get the absolute position in the world
                Vector2 spawnWorldPos = tileWorldPosition + entity.SpawnOffset;

                // 2. Subtract screen position (this gets the offset from the top-left of the screen)
                Vector2 screenPos = spawnWorldPos - Main.screenPosition;

                // 3. TRANSFORM by the GameViewMatrix
                // This is the most important part. It accounts for the player's Zoom level.
                screenPos = Vector2.Transform(screenPos, Main.GameViewMatrix.TransformationMatrix);

                // 4. DIVIDE by UI Scale
                // Since your layer is 'InterfaceScaleType.UI', Terraria expects coordinates 
                // relative to the UI scale, not the raw screen pixels.
                screenPos /= Main.UIScale;

                // 5. Calculate size (optional: scale the box size with zoom so it doesn't look tiny)
                float scale = Main.GameViewMatrix.Zoom.X / Main.UIScale;
                int size = (int)(16 * scale);

                // 6. Draw the box
                Rectangle box = new Rectangle((int)screenPos.X - size / 2, (int)screenPos.Y - size / 2, size, size);

                // Draw your pixel and border using this 'box'
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, box, Color.Red * 0.5f);
                DrawBorder(spriteBatch, box, 2, Color.Yellow);
            }
        }

        private void DrawBorder(SpriteBatch spriteBatch, Rectangle rect, int thickness, Color color)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            // Top
            spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
            // Bottom
            spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y + rect.Height - thickness, rect.Width, thickness), color);
            // Left
            spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
            // Right
            spriteBatch.Draw(pixel, new Rectangle(rect.X + rect.Width - thickness, rect.Y, thickness, rect.Height), color);
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

            // Call RefreshUI when TargetObject changes
            if (TargetObject != null && waveContentPanels.Count != TargetObject.Waves.Count)
            {
                RefreshUI();
            }

            // Handle panel dragging
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


            if (_isEditing)
            {
                Main.LocalPlayer.mouseInterface = true;

                _tempText = Main.GetInputText(_tempText);
                _editingEntity.FullName = _tempText;
                RefreshUI();

                if (Main.inputTextEnter)
                {
                    StopEditing();
                }
                else if (Main.inputTextEscape)
                {
                    StopEditing();
                }
            }
        }
        private void StopEditing()
        {
            _isEditing = false;
            Main.blockInput = false; // Let the player move again
            Main.clrInput();         // Flush the buffer so 'E' doesn't open inventory
        }
    }
}
