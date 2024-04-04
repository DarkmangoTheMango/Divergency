using Divergency.Common.Helpers;
using Divergency.Common.Players;
using Divergency.Content.Buffs;
using Divergency.Content.Dusts;
using Divergency.Content.Particles;
using Divergency.Content.Projectiles.Hostile;
using Divergency.Content.Projectiles.Summoner.Minions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using static Divergency.Content.Projectiles.Magic.CorescillationProj;

namespace Divergency.Content.NPCs.LivingGrove
{

    public class CoreRottenBlossom : ModNPC
    {
        public bool initialize { get; private set; }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Scale = 0.2f };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            Main.npcFrameCount[NPC.type] = 6;
            //.setdefault("Core Sage");


        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 280;
            NPC.damage = 0;
            NPC.defense = 8;
            NPC.knockBackResist = 0f;

            NPC.noTileCollide = false;

            NPC.scale = 0.2f;
            NPC.Size = new Vector2(84, 70);

            NPC.HitSound = SoundID.NPCHit49;
            NPC.DeathSound = SoundID.NPCDeath51;
            NPC.value = Item.sellPrice(0, 0, 0, 0);
           //NPC.hide = true;
            NPC.aiStyle = -1;
            NPC.noGravity = false;

        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("A powerful warlock that can focus the energy radiating from living crystals in powerful magic.")
            });
        }
   


       
        public override void AI()
        {

            if (!initialize)
            {

                initialize = true;

            }
            else
            {
                NPC.ai[0]++;
                Player player = Main.player[NPC.target];

                NPC.velocity.Y += 3;

                for (int io = 0; io < Main.maxNetPlayers; io++)
                {
                    Player target = Main.player[io];
                    if (target.Distance(NPC.Center) < 500 && !target.HasBuff(ModContent.BuffType<CoreInfection>()))
                    {
                        if (!target.HasBuff(ModContent.BuffType<CoreInfectionII>()))
                        {
                            target.AddBuff(ModContent.BuffType<CoreInfection>(), 180);

                        }
                        target.AddBuff(BuffID.Slow, 180);
                    }
                }
                if (NPC.ai[0] == 200)
                {
                    DivergencyDraw.SpawnCirclePulse(NPC.Center, Color.DarkGreen, 0.6f);
                    for (int i = 0; i < 100; i++)
                    {
                        Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);

                        Dust dust = Dust.NewDustPerfect(NPC.Center + (speed * 100f), DustID.TerraBlade, speed * 35f, 0, default, 1f);
                        NPC.ai[0] = 0;
                    }
                }
                
            }

            
        }


        public override void HitEffect(NPC.HitInfo hit)
        {

            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }


            if (NPC.life <= 0)
            {
                //NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)NPC.Bottom.Y, ModContent.NPCType<SageDeath>());
            }
        }

     



        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
                           //base textures

                Texture2D a = TextureAssets.Npc[Type].Value; 
        
                Vector2 drawOrigin = new(a.Width / 2, a.Height / 2);

                Main.EntitySpriteDraw(a, NPC.Center - Main.screenPosition + new Vector2(0, 0), null, drawColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);

            
            return false;
        }
    }

}
