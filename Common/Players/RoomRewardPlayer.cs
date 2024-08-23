using Divergency.Content.Events.LivingCore;
using Divergency.Content.NPCs.Forest;
using Microsoft.Xna.Framework;
using System.Drawing;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Common.Players
{
    public class RoomRewardPlayer : ModPlayer
    {
        public override void PreUpdate()
        {
            if (Main.mouseLeft)
                LivingCoreEvent.RequestReward();
        }
    }
}