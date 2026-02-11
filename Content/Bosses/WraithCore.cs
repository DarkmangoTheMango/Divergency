using Divergency.Content.NPCs.LivingGrove;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;



namespace Divergency.Content.Bosses
{
    public class WraithCore : ModNPC
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

            Main.npcFrameCount[NPC.type] = 6; // make sure to set this for your modNPCs.
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
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC taggedNPC = Main.npc[k];

                    if (taggedNPC.active && taggedNPC.ModNPC is Wraith)
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

                NPC.rotation = cachedNPC.oldRot[cachedNPC.oldRot.Length - 1];



                Vector3 RGB = new Vector3(1.45f, 2.55f, 0.94f);
                float multiplier = 1;
                float max = 2.25f;
                float min = 1.0f;
                RGB *= multiplier;
                if (RGB.X > max)
                {
                    multiplier = 0.5f;
                }
                if (RGB.X < min)
                {
                    multiplier = 1.5f;
                }
                Lighting.AddLight(NPC.position, RGB.X, RGB.Y, RGB.Z);
                Vector2 vector; float speed; float turnResistance = 10f; bool toNPC = false;
                Vector2 WraithPos = cachedNPC.Center;
                NPC.spriteDirection = -NPC.direction;

                speed = NPC.Distance(WraithPos) / 4;

                Vector2 moveTo = cachedNPC.Center + new Vector2(-25 * cachedNPC.direction, 20);
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

                NPC.velocity = move;
                NPC.TargetClosest(true);




            }

        }

        public override void FindFrame(int frameHeight)
        {
            frametimer++;
            if (frametimer == 5)
            {
                NPC.frameCounter++;
                frametimer = 0;
                NPC.frame.Y += frameHeight;

            }
            NPC.frameCounter = 0;
            if (NPC.frame.Y >= frameHeight * 6)
                NPC.frame.Y = 0;

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float scaleModifier = 0.8f;

            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Noise/ElectricNoise2").Value;
            Effect shader = Divergency.WraithOrb.Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, shader, Main.GameViewMatrix.TransformationMatrix);

            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);

            Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition, texture.Bounds, Color.White, 0f, texture.Size() * 0.5f, NPC.scale * 0.1f * scaleModifier, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Glow_0").Value;

            float beatSpeed = 1f;
            float t = (Main.GlobalTimeWrappedHourly * beatSpeed) % 1f;

            float beat2 = MathF.Exp(-60f * MathF.Pow(t - 0.15f, 2f));
            float beat1 = MathF.Exp(-60f * MathF.Pow(t - 0.35f, 2f));

            float squashAmount = beat2 * 0.25f;
            float stretchAmount = beat1 * 0.25f;

            Vector2 pulseModifier = new(1f + squashAmount - stretchAmount, 1f - squashAmount + stretchAmount);

            Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition, texture.Bounds, new Color(191, 255, 119, 0), 0f, texture.Size() * 0.5f, NPC.scale * 0.5f * scaleModifier * pulseModifier, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition, texture.Bounds, new Color(96, 214, 72, 0), 0f, texture.Size() * 0.5f, NPC.scale * 0.5f * scaleModifier * pulseModifier, SpriteEffects.None, 0);

            texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Ring").Value;

            Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition, texture.Bounds, new Color(96, 214, 72, 0), 0f, texture.Size() * 0.5f, NPC.scale * 0.85f * scaleModifier, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition, texture.Bounds, new Color(191, 255, 119, 0), 0f, texture.Size() * 0.5f, NPC.scale * 0.8f * scaleModifier, SpriteEffects.None, 0);

            return false;
        }
    }
}