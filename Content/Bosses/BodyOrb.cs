using System;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
﻿using Microsoft.Xna.Framework;
using Divergency.Content.NPCs.LivingGrove;



namespace Divergency.Content.Bosses
{
    public class BodyOrb : ModNPC
    {
        public static float Magnitude(Vector2 mag)
        {
            return (float)Math.Sqrt(mag.X * mag.X + mag.Y * mag.Y);
        }

       


        public float State = 0;

        public float AI_Timer;
        public float Timer;
        private bool SoundPlayed;
        private int frametimer;

        public override void SetStaticDefaults()
        {

            Main.npcFrameCount[NPC.type] = 1; // make sure to set this for your modNPCs.
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0);
   
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 6000;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.knockBackResist = 0f;
            NPC.width = 22;
            NPC.height = 22;
            NPC.value = Item.buyPrice(0, 3, 0, 0);
            // NPC.dontTakeDamage = true;
            NPC.friendly = false;
            //NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            //NPC.dontTakeDamageFromHostiles = true;
            NPC.behindTiles = false;
            NPC.ShowNameOnHover = false;
            NPC.dontTakeDamage = true;
            NPC.immortal = true;

        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
      

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("its orbin time")
            });
        }
        public NPC cachedNPC;
        private bool spawned;

        public override void AI()

        {
            if (!spawned)
            {
                bool found = false;

                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC taggedNPC = Main.npc[k];

                    if (taggedNPC.active && taggedNPC.ModNPC is WraithBody)
                    {
                        cachedNPC = taggedNPC;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    NPC.active = false;
                    return;
                }

                spawned = true;
            }
            if (!cachedNPC.active)
            {
                NPC.active = false;
            }
            if (spawned)
            {




                Vector3 RGB = new Vector3(1.45f, 2.55f, 0.94f);
                float multiplier = 1;
                float max = 2.25f;
                float min = 1.0f;
                RGB *= multiplier;
                if (RGB.X > max)
                {
                    multiplier = 0.2f;
                }
                if (RGB.X < min)
                {
                    multiplier = 0.6f;
                }
                Lighting.AddLight(NPC.position, RGB.X, RGB.Y, RGB.Z);
                Vector2 vector; float speed; float turnResistance = 10f; bool toNPC = false;
                Vector2 WraithPos = cachedNPC.Center;
                NPC.spriteDirection = -NPC.direction;

                speed = NPC.Distance(WraithPos) / 4;

                Vector2 moveTo = cachedNPC.Center + new Vector2(0, 20);
                Vector2 move = moveTo - NPC.Center;
                float magnitude = Magnitude(move);
                if (magnitude > speed)
                {
                    move *= speed / magnitude;
                }

                move = (NPC.velocity * turnResistance + move) / (turnResistance + 0.5f);
                magnitude = Magnitude(move);
                if (magnitude > speed)
                {
                    move *= speed / magnitude;
                }

                NPC.velocity = move / 1f;
                NPC.TargetClosest(true);




            }
        }
   
        #region Movement towards main body



        #endregion

    }





}
