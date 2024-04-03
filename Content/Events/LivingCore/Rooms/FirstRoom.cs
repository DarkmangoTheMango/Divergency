using Divergency.Content.Items.Weapons.LivingCore;
using Divergency.Content.NPCs.LivingGrove;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Divergency.Content.Events.LivingCore.Rooms
{
    internal class FirstRoom : LivingCoreRoom
    {
        public override int Music => MusicLoader.GetMusicSlot("Divergency/Assets/Sounds/Music/LivingGroveBattle1");
        public override string RewardTexturePath => "Divergency/Assets/Rewards/LivingCoreSwordGlow";
        public override int RewardID => ModContent.ItemType<LivingCoreSword>();
        public override Vector2[] BlockingBlocks => new Vector2[] {
  
  
        };


        public override Wave? getWave(int wave)
        {
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
                            new Instance(ModContent.NPCType<Corennector>(), new Vector2(40, -100)),
                            new Instance(ModContent.NPCType<Coreling>(), new Vector2(300, 300)),
                            new Instance(ModContent.NPCType<Coreling>(), new Vector2(300, -300)),
                            new Instance(ModContent.NPCType<Coreling>(), new Vector2(-300, -300)),
                            new Instance(ModContent.NPCType<Coreling>(), new Vector2(-300, 300)),
                        });
                case 3:
                    return new Wave("WAVE 3!",
                        new Instance[]
                        {
                            new Instance(ModContent.NPCType<Coreling>(), new Vector2(300, 300)),
                            new Instance(ModContent.NPCType<Coreling>(), new Vector2(300, -300)),
                            new Instance(ModContent.NPCType<Coreling>(), new Vector2(-300, -300)),
                            new Instance(ModContent.NPCType<Coreling>(), new Vector2(-300, 300)),
                        });
                case 4:
                    return new Wave("WAVE 4!",
                        new Instance[]
                        {
                            new Instance(ModContent.NPCType<Coreling>(), new Vector2(300, 300)),
                            new Instance(ModContent.NPCType<Coreling>(), new Vector2(300, -300)),
                            new Instance(ModContent.NPCType<Coreling>(), new Vector2(-300, -300)),
                            new Instance(ModContent.NPCType<Coreling>(), new Vector2(-300, 300)),
                        });
            }
            return null;
        }
        public override int getWaves()
        {
            return 4;
        }
    }
}
