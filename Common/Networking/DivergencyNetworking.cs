using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Common.Networking
{
    public static class DivergencyNetworking

    {
    public static ModPacket WriteToPacket(int capacity, MessageType type)
    {
        ModPacket packet = Divergency.Instance.GetPacket(capacity);
        packet.Write((byte)type);
        return packet;
    }

    public static ModPacket WriteToPacket(int capacity, MessageType type, Action<ModPacket> action) 
    {
        ModPacket packet = Divergency.Instance.GetPacket(capacity);
        packet.Write((byte)type);
        action?.Invoke(packet);
        return packet;
    }

    public static ModPacket WriteToPacketAndSend(int capacity, MessageType type, Action<ModPacket> beforeSend)
    {
        var packet = WriteToPacket(capacity, type, beforeSend);
        packet.Send();
        return packet;
    }
        public static void HandlePacket(BinaryReader reader, int whoAmI)
        {
            var id = (MessageType)reader.ReadByte();
            byte player;
            switch (id)
            {
                case MessageType.TileRightClickProj:

                    if (Main.netMode == NetmodeID.Server)
                    {
                        player = reader.ReadByte();
                        int bossType = reader.ReadInt32();
                        int npcCenterX = reader.ReadInt32();
                        int npcCenterY = reader.ReadInt32();

                       

                       // int ProjectileID = ModContent.GetModProjectile(ModContent.ProjectileType);
                       // Main.projectile[ProjectileID].netUpdate2 = true;

                    }
                    break;

            }
        }
       // public static void TileRightClickProj(byte whoAmI, int type, int x, int y) => Divergency.WriteToPacket(Divergency.Instance.GetPacket(), (byte)MessageType.TileRightClickProj, whoAmI, type, x, y).Send(-1);
    }
}
