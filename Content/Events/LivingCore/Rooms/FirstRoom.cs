using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Divergency.Content.Items.Weapons.Melee;
using Divergency.Content.NPCs.LivingGrove;

namespace Divergency.Content.Events.LivingCore.Rooms
{
    internal class FirstRoom : LivingCoreRoom
    {
        public override int Music => MusicLoader.GetMusicSlot("Divergency/Assets/Sounds/Music/LivingGroveBattle1");
        public override string RewardTexturePath => "Divergency/Assets/Rewards/LivingCoreSwordGlow";
        public override int RewardID => ModContent.ItemType<LivingCoreSword>();
        public override Vector2[] BlockingBlocks => new Vector2[] {
            new Vector2(-74, -10),
            new Vector2(-74, -11),
            new Vector2(-74, -12),
            new Vector2(-74, -13),
            new Vector2(-74, -14),
            new Vector2(-74, -15),
            new Vector2(-74, -16),
            new Vector2(-74, -17),
            new Vector2(-74, -18),
            new Vector2(-74, -19),
            new Vector2(-74, -20),

            new Vector2(74, 0),
            new Vector2(74, -1),
            new Vector2(74, -2),
            new Vector2(74, -3),
            new Vector2(74, -4),
            new Vector2(74, -5),
            new Vector2(74, -6),
            new Vector2(74, -7),
            new Vector2(74, -8),

            new Vector2(74, 1),
            new Vector2(74, 2),
            new Vector2(74, 3),
            new Vector2(74, 4),
            new Vector2(74, 5),
            new Vector2(74, 6),
            new Vector2(74, 7),
            new Vector2(74, 8),
        };


        public override Wave? getWave(int wave)
        {
            switch (wave)
            {
                case 1:
                    return new Wave("WAVE 1!",
                        new Instance[]
                        {
                            new Instance(ModContent.NPCType<Sage>(), new Vector2(300, 300)),
                            new Instance(ModContent.NPCType<Coreling>(), new Vector2(300, -300)),
                            new Instance(ModContent.NPCType<Coreling>(), new Vector2(-300, -300)),
                            new Instance(ModContent.NPCType<Coreling>(), new Vector2(-300, 300)),
                        });
                case 2:
                    return new Wave("WAVE 2!",
                        new Instance[]
                        {
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
