using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Divergency.Content.Events.LivingCore.Rooms;
using Divergency.Tiles.LivingTree;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Divergency.Content.Events.LivingCore
{
    public static class LivingCoreEvent
    {
        // TODO: Make progress bar since     may not work without an NPC

        public static Type[] lcrList = new Type[] // add future room to this
        {
            typeof(FirstRoom)
        };

        public static bool HasRoomBeenCleared(Type t)
        {
            if (!lcrList.Contains(t))
                return false;

            for (int i = 0; i < lcrList.Length; i++)
            {
                if (lcrList[i] == t)
                    return DownedHelper.livingCoreRoomCompletionTracker[i];
            }

            return false;
        }

        public static void RoomCleared(Type t)
        {
            for (int i = 0; i < lcrList.Length; i++)
            {
                if (lcrList[i] == t)
                {
                    DownedHelper.livingCoreRoomCompletionTracker[i] = true;
                    return;
                }
            }

            Console.WriteLine("Room missing from lcrList");
        }

        public static bool Active { get; private set; }
        public static Tile Altar { get; private set; }
        public static int X { get; private set; }
        public static int Y { get; private set; }
        public static Vector2 Position { get => new(X * 16f, Y * 16f); }
        public static LivingCoreRoom Room { get; private set; }

        public static void Update()
        {
            if (Room != null)
                Room.Update();
        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            if (Room != null)
                Room.Draw(spriteBatch);
        }
        public static void Begin(int i, int j, LivingCoreRoom room)
        {
            if (Active)
                return;
            if (Main.tile[i, j].TileType != ModContent.TileType<LivingCoreAltarTile1>())
                return;

            Active = true;
            Altar = Main.tile[i, j];
            X = i;
            Y = j;
            Room = room;

            Player player = Main.LocalPlayer;
            // TODO: Add extension methods for fetching globals to allow player.Divergency().X = X;
            player.GetModPlayer<ScreenShakePlayer>().ScreenShakeIntensity = 50;

            int left = i - Main.tile[i, j].TileFrameX / 18;
            int top = j - Main.tile[i, j].TileFrameY / 18;
            
            room.Begin(left, top);
        }
        public static void End()
        {
            if (Room != null)
                Room.End();

            Active = false;
            Altar = default;
            X = 0;
            Y = 0;
            Room = default;
        }
        public static void Load()
        {
            End();
        }
        public static void Unload()
        {
            End();
        }

        public static void AddProgress(int amount)
        {
            return;
            if (Room != null)
            {
                Room.Kills += amount;
            }
        }

        public static void RemoveProgress(int amount)
        {
            if (Room != null)
                Room.Kills -= amount;
        }

        public static float GetProgress()
        {
            if (Room != null)
                return Room.Progress;
            return 0f;
        }
	}
}
