using Divergency.Common.Players;
using Divergency.Content.Dusts;
using Divergency.Content.NPCs.LivingGrove;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Common.GlobalNPCs
{
    public class NPCIdenity : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool livingGroveFauna(NPC npc) => (npc.type == ModContent.NPCType<Coreling>() || npc.type == ModContent.NPCType<Corelossus>());

        float debuffSoundTimer;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (livingGroveFauna(npc) && (npc.HasBuff(BuffID.OnFire) || npc.HasBuff(BuffID.OnFire3)) && !npc.buffImmune[BuffID.OnFire] && !npc.buffImmune[BuffID.OnFire3])
            {
                if (debuffSoundTimer >= 30f)
                {
                    SoundEngine.PlaySound(npc.HitSound, npc.Center);
                    debuffSoundTimer = 0f;
                }

                debuffSoundTimer++;

                if (damage < 2) { damage = 2; }
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (livingGroveFauna(npc) && (npc.HasBuff(BuffID.OnFire) || npc.HasBuff(BuffID.OnFire3)) && !npc.buffImmune[BuffID.OnFire] && !npc.buffImmune[BuffID.OnFire3])
            {
                if (Main.rand.NextBool(10)) { Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<SmokeIncendiary>(), npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 0, default, 0.5f); }

                Lighting.AddLight(npc.Center, new Color(255, 192, 0).ToVector3());
            }
        }

        public class LivingCoreBattleTheme : ModSceneEffect
        {
            public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

            public override int Music => music;

            int music;

            public override bool IsSceneEffectActive(Player player)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].type == ModContent.NPCType<Coreling>() && Main.npc[i].active)
                    {
                        music = MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/LivingGroveBattle1");
                        return true;
                    }
                }

                return false;
            }
        }
    }
}