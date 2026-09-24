using System;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
﻿using Microsoft.Xna.Framework;
using Divergency.Content.NPCs.LivingGrove;
using Microsoft.Xna.Framework.Graphics;



namespace Divergency.Content.Bosses
{
    public class WraithBody : ModNPC
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
            NPC.width = 50;
            NPC.height = 50;
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

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/Bosses/WraithBody").Value;

            Vector2 position = NPC.Center - screenPos - new Vector2(0f, NPC.gfxOffY - 2f);
            Color color = Color.White;

            SpriteEffects spriteEffects = NPC.spriteDirection > 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
           
            spriteBatch.Draw(texture, position, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects, 1f);
            return false;
        }
        public override void AI()

        {

            if (!spawned)
            {
                bool found = false;

                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC taggedNPC = Main.npc[k];

                    if (taggedNPC.active && taggedNPC.ModNPC is Wraith)
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

                NPC.ai[3] = cachedNPC.ai[3];
             
                if (NPC.ai[4] == 2)
                {
                    cachedNPC.ai[4] = 2;
                    NPC.ai[4] = 0;
                }
                NPC.rotation = cachedNPC.oldRot[cachedNPC.oldRot.Length - 1];

                Vector2 vector; float speed; float turnResistance = 10f; bool toNPC = false;
                Vector2 WraithPos = cachedNPC.Center;
                NPC.spriteDirection = -NPC.direction;

                speed = NPC.Distance(WraithPos) / 4;

                Vector2 moveTo = cachedNPC.Center + new Vector2(-30 * cachedNPC.direction, 65 - cachedNPC.rotation);
                Vector2 move = moveTo - NPC.Center;
                float magnitude = Magnitude(move);
                if (magnitude > speed)
                {
                    move *= speed / magnitude;
                }

                move = (NPC.velocity * turnResistance + move) / (turnResistance + 0.1f);
                magnitude = Magnitude(move);
                if (magnitude > speed)
                {
                    move *= speed / magnitude;
                }

                NPC.velocity = move / 1.35f;
                NPC.TargetClosest(true);




            }
        }
   
        #region Movement towards main body



        #endregion

    }





}
