using Divergency.Common.Helpers;
using Divergency.Content.Items.Weapons.LivingCore;
using Divergency.Content.NPCs.LivingGrove;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace Divergency.Content.Events.LivingCore.Rooms
{
    public class FirstRoom : LivingCoreRoom
    {
        public override int Music => MusicLoader.GetMusicSlot("Divergency/Assets/Sounds/Music/LivingGroveBattle1");
        public override List<Reward> Rewards => new List<Reward> {
            new Reward(ModContent.ItemType<LivingCoreSword>(), "Divergency/Assets/Rewards/LivingCoreSwordGlow"),
            new Reward(ModContent.ItemType<LivingCoreSword>(), "Divergency/Assets/Rewards/LivingCoreSwordGlow"),
            new Reward(ModContent.ItemType<LivingCoreSword>(), "Divergency/Assets/Rewards/LivingCoreSwordGlow"),
        };

        public override Vector2[] BlockingBlocks => new Vector2[] {
  
  
        };

        public override Wave? getWave(int wave)
        {
            int claimedRewards = LivingCoreEvent.GetObtainedRewards(this).Select(b => b == true ? 1 : 0).Sum();

            switch (wave)
            {
                case 1:
                    return new Wave("WAVE 1!",
                        new Instance[]
                        {
                            new Instance(ModContent.NPCType<CoreBlockadeRight>(), new Vector2(300, -300)),
                            new Instance(ModContent.NPCType<CoreBlockadeLeft>(), new Vector2(-300, -300)),
                        });
                case 2:
                    return new Wave("WAVE 2!",
                        new Instance[]
                        {
                            new Instance(ModContent.NPCType<Corennector>(), new Vector2(0, -100)),
                            new Instance(ModContent.NPCType<Coreling>(), new Vector2(300, 300)),
                            new Instance(ModContent.NPCType<Coreling>(), new Vector2(300, -300)),
                            new Instance(ModContent.NPCType<Coreling>(), new Vector2(-300, -300)),
                            new Instance(claimedRewards == 0 ? ModContent.NPCType<Coreling>() : ModContent.NPCType<Sage>(), new Vector2(-300, 300)),
                        });
            }
            return null;
        }

        public override int getWaves()
        {
            return 2;
        }
    }
}
