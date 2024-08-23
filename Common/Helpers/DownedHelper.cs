using Divergency.Content.Events.LivingCore;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Divergency.Common.Helpers
    {
    // Acts as a container for "downed boss" flags.
    // Set a flag like this in your bosses OnKill hook:
    //    NPC.SetEventFlagCleared(ref DownedBossSystem.downedMinionBoss, -1);

    // Saving and loading these flags requires TagCompounds, a guide exists on the wiki: https://github.com/tModLoader/tModLoader/wiki/Saving-and-loading-using-TagCompound
    public class DownedHelper : ModSystem
    {
        public static bool[][] livingCoreRoomCompletionTracker;


        public override void OnWorldLoad()
        {
            livingCoreRoomCompletionTracker = new bool[LivingCoreEvent.lcrList.Length][];
            for (int i = 0; i < LivingCoreEvent.lcrList.Length; i++)
            {
                LivingCoreRoom room = (LivingCoreRoom)Activator.CreateInstance(LivingCoreEvent.lcrList[i]);

                livingCoreRoomCompletionTracker[i] = new bool[room.Rewards.Count];

                for (int rewardIdx = 0; rewardIdx < livingCoreRoomCompletionTracker.Length; rewardIdx++)
                {
                    livingCoreRoomCompletionTracker[i][rewardIdx] = false;
                }
            }
        }

        public override void OnWorldUnload()
        {
            livingCoreRoomCompletionTracker = null;
        }

        // We save our data sets using TagCompounds.
        // NOTE: The tag instance provided here is always empty by default.
        public override void SaveWorldData(TagCompound tag)
        {
            for (int i = 0; i < livingCoreRoomCompletionTracker.Length; i++)
            {
                tag.Add("RoomStatus"+i, livingCoreRoomCompletionTracker[i]);
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            for (int i = 0; i < livingCoreRoomCompletionTracker.Length; i++)
            {
                if (tag.TryGet<bool[]>("RoomStatus" + i, out bool[] val))
                {
                    livingCoreRoomCompletionTracker[i] = val;
                }
            }
        }

        /*
        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            for (int i = 0; i < livingCoreRoomCompletionTracker.Length; i++)
            {
                flags[i] = livingCoreRoomCompletionTracker[i];
            }
            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            for (int i = 0; i < livingCoreRoomCompletionTracker.Length; i++)
            {
                livingCoreRoomCompletionTracker[i] = flags[i];
            }
        }
        */
    }
}