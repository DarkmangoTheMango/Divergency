
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Divergency.Common;
using Divergency.Content.Projectiles;
using Divergency.Common.Helpers;
using Terraria.Utilities;
using Mono.Cecil;


namespace Divergency.Content.Bosses
{
    public class Wraith : ModNPC
    {
        //main body, responsible for body attacks
        //npc ai 0 is the normal timer
        private int frame = 0;
        private int frameTimer = 0;
        public float State = 0;
        public byte phase;
        private int framerate;

        private byte _hitShake;
        private bool spawned;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 45;
            NPCID.Sets.TrailingMode[NPC.type] = 3;

            Main.npcFrameCount[NPC.type] = 25; // make sure to set this for your modNPCs.
      
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 5000;
            NPC.damage = 30;
            NPC.defense = 10;
            NPC.knockBackResist = 0f;
            NPC.width = 122;
            NPC.height = 144;
            // NPC.dontTakeDamage = true;
            NPC.friendly = false;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            
            //NPC.dontTakeDamageFromHostiles = true;
            NPC.behindTiles = false;

            Music = MusicLoader.GetMusicSlot("Divergency/Assets/Sounds/Music/DuelOfTheRoots");

        }
        private enum Phase
        {
            Float,
            HandDash,

        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/Bosses/Wraith").Value;

            Vector2 position = NPC.Center - screenPos - new Vector2(0f, NPC.gfxOffY - 2f);
            Color color = Color.White;

            SpriteEffects spriteEffects = NPC.spriteDirection > 0 ?   SpriteEffects.None : SpriteEffects.FlipVertically;
            if (_hitShake > 0)
            {
                position += new Vector2(Main.rand.Next(-_hitShake, _hitShake), Main.rand.Next(-_hitShake, _hitShake));
                _hitShake--;
            }
                spriteBatch.Draw(texture, position, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects, 1f);
            return false;
        }
        public override void FindFrame(int blabla)
        {
            int frameHeight = 180;
            NPC.frame.Width = 180;
            NPC.frame.Height = 180;

            //idle anime
            if (State == (float)Phase.Float)
            {
                framerate = 10;
                NPC.frameCounter++;

                if (NPC.frameCounter >= framerate)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;

                    if  (NPC.frame.Y >= frameHeight * 4)
                    {
                        NPC.frame.Y = 0 * frameHeight;
                    }
                }

            }

        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            byte shake = (byte)MathHelper.Clamp(hit.Damage / 8, 4, 10);
            if (shake > _hitShake)
            {
                _hitShake = shake;
            }
        }
        public override void AI()
        {

            NPC.TargetClosest();
            if (!spawned)
            {
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WraithCore>(), 0, NPC.whoAmI);
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WraithHand>(), 0, NPC.whoAmI);
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WraithBody>(), 0, NPC.whoAmI);
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<BodyOrb>(), 0, NPC.whoAmI);
              
                    spawned = true;
            }
            else
            {
                switch (State)
                {
                    case (float)Phase.Float:
                        Float();
                        break;


                }
            }
          
            if (NPC.life <= NPC.lifeMax / 2)
            {
                phase = 2;
            }

        }
        private void Float()
        {
            NPC.oldRot[0] = NPC.velocity.ToRotation();
            for (int i = NPCID.Sets.TrailCacheLength[Type] - 1; i > 0; i--)
            {
                NPC.oldRot[i] = NPC.oldRot[i - 1];
            }
            NPC.ai[0]++;
            Player player = Main.player[NPC.target];
            NPC.direction = NPC.spriteDirection = (NPC.velocity.X >= 0f) ? 1 : -1;


            NPC.rotation = NPC.velocity.ToRotation();       
            

            if (phase == 2)
            {
                NPC.Move(player.Center, 4f);
            }
            else
            {
              
                    NPC.Move(player.Center, 5f);


            }
            if (NPC.ai[0] == 480)
            {
                WeightedRandom<Phase> phase = new WeightedRandom<Phase>();
                //phase.Add(Phase.HandDash, 1f);
                NPC.ai[0] = 0;
            }

        }
    }
}
