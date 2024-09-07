using System;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Divergency.Content.NPCs.LivingGrove;



namespace Divergency.Content.Bosses
{
    public class WraithHand : ModNPC
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

            Main.npcFrameCount[NPC.type] = 4; // make sure to set this for your modNPCs.
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0);

        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 6000;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.knockBackResist = 0f;
            NPC.width = 90;
            NPC.height = 110;
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


        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
      

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("hands down")
            });
        }
        public NPC cachedNPC;
        private bool spawned;

        public override void AI()

        {
            if (!spawned)
            {
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC taggedNPC = Main.npc[k];

                    if (taggedNPC.active && taggedNPC.ModNPC is WraithBody)
                    {
                        cachedNPC = taggedNPC;
                    }
                }

                spawned = true;
            }
            if (!cachedNPC.active)
            {
                NPC.active = false;
            }
            if (spawned)
            {



                Vector2 vector; float speed; float turnResistance = 10f; bool toNPC = false;
                Vector2 WraithPos = cachedNPC.Center;
                NPC.spriteDirection = -NPC.direction;

                speed = NPC.Distance(WraithPos) / 4;

                Vector2 moveTo = cachedNPC.Center + new Vector2(-50 * cachedNPC.direction, 0);
                Vector2 move = moveTo - NPC.Center;
                float magnitude = Magnitude(move);
                if (magnitude > speed)
                {
                    move *= speed / magnitude;
                }

                move = (NPC.velocity * turnResistance + move) / (turnResistance + 2f);
                magnitude = Magnitude(move);
                if (magnitude > speed)
                {
                    move *= speed / magnitude;
                }

                NPC.velocity = move;
                NPC.TargetClosest(true);




            }

        }

        public override void FindFrame(int frameHeight)
        {
            frametimer++;
            if (frametimer == 60)
            {
                NPC.frameCounter++;

                frametimer = 0;
                NPC.frame.Y += frameHeight;

            }
            NPC.frameCounter = 0;
            if (NPC.frame.Y >= frameHeight * 4)
                NPC.frame.Y = 0;

        }

    }
}