using Divergency.Content.Dusts;
using Divergency.Content.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.NPCs.LivingGrove
{

    public class Corennector : ModNPC
    {
        int startingFrame;

        int endingFrame;

        int framerate;

        int combatFrame = 6;

        float maxSpeed = 2f;

        float attackTimer;

        float attackCooldown = 30f;

        bool attacking;

        enum State
        {
            Connect,
            Idle,
            Death

        }

        State state = State.Connect;

        public bool TargetFound { get; private set; }
        public NPC cachedNPC { get; private set; }
        public NPC cachedNPC2 { get; private set; }
        public NPC cachedNPC3 { get; private set; }

        public int counter { get; private set; }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 100;
            NPC.damage = 30;
            NPC.defense = 5;
            NPC.knockBackResist = 0.2f;

            NPC.noTileCollide = true;

            NPC.scale = 0.5f;
            NPC.Size = new Vector2(44f, 54f);

            NPC.HitSound = SoundID.DD2_WitherBeastHurt;
            NPC.DeathSound = SoundID.DD2_WitherBeastDeath;
            NPC.value = Item.sellPrice(0, 0, 0, 0);

            NPC.aiStyle = -1;
            NPC.noGravity = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("The most annoying creature in the groves :kojima:")
            });
        }

        public override void AI()
        {
            Player target = Main.player[NPC.target];
            Vector2 VinePosTop = NPC.Top;
            Vector2 VinePosBottomRight = NPC.BottomRight;
            Vector2 VinePosBottomLeft = NPC.BottomRight;

            NPC.TargetClosest(true);

            NPC.spriteDirection = NPC.direction;

            Lighting.AddLight(NPC.Center, new Color(79, 214, 126).ToVector3());

            if (state == State.Connect)
            {
                 //START FINDING ENEMYS TO CONNECT
                if (counter == 0)
                {
                    for (int k = 0; k < Main.maxNPCs; k++)
                    {
                        NPC taggedNPC = Main.npc[k];

                        if (taggedNPC.active && taggedNPC.ModNPC is not Corennector && taggedNPC.Distance(NPC.Center) < 1000)
                        {
                            cachedNPC = taggedNPC;
                        }
                    } 

                    counter++;
                }
                if (counter == 1)
                {
                    for (int k = 0; k < Main.maxNPCs; k++)
                    {
                        NPC taggedNPC = Main.npc[k];

                        if (taggedNPC.active && taggedNPC.ModNPC is not Corennector && taggedNPC.Distance(NPC.Center) < 1000 && taggedNPC != cachedNPC)
                        {
                            cachedNPC2 = taggedNPC;
                        }
                    }
                    counter++;
                }
                if (counter == 2)
                {
                    for (int k = 0; k < Main.maxNPCs; k++)
                    {
                        NPC taggedNPC = Main.npc[k];

                        if (taggedNPC.active && taggedNPC.ModNPC is not Corennector && taggedNPC.Distance(NPC.Center) < 1000 && taggedNPC != cachedNPC2 && taggedNPC != cachedNPC)
                        {
                            cachedNPC3 = taggedNPC;
                        }
                    }
                    counter++;
                }
                if (counter == 3)
                {
                    // NPC.Move(cachedNPC.Center + new Vector2(0, 18), 30f, 20);
                    
                        Main.NewText(cachedNPC);
                    Main.NewText(cachedNPC2);
                    Main.NewText(cachedNPC3);
                    counter++;
                }
                //FOUND ENEMIES END

                //START CONNECTING
                if (counter == 4)
                {
                    NPC.ai[0]++;//general ai timer
                    if (NPC.ai[0] == 20)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/InvocationShot") with { Pitch = 1f }, NPC.Center);

                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), VinePosTop, NPC.DirectionTo(cachedNPC.Center) * 10, ModContent.ProjectileType<GuardianBeam>(), 0, 0);
                    }
                    if (NPC.ai[0] == 40)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/InvocationShot") with { Pitch = 1f }, NPC.Center);

                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), VinePosBottomRight, NPC.DirectionTo(cachedNPC2.Center) * 10, ModContent.ProjectileType<GuardianBeam>(), 0, 0);
                    }
                    if (NPC.ai[0] == 60)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Divergency/Assets/Sounds/Items/InvocationShot") with { Pitch = 1f }, NPC.Center);

                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), VinePosBottomLeft, NPC.DirectionTo(cachedNPC3.Center) * 10, ModContent.ProjectileType<GuardianBeam>(), 0, 0);
                    }
                }
            }
         

        }


        public override void OnKill()
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<LivingShard>(), Main.rand.NextVector2Circular(1f, 1f) * 2f, 0, default, 1f);
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<CradleWoodFurniture>(), Main.rand.NextVector2Circular(1f, 1f) * 2f, 0, default, 1f);
            }

            // if (Main.netMode != NetmodeID.Server) { Gore.NewGore(NPC.GetSource_Death(), NPC.position, new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-1f, -3f)), Mod.Find<ModGore>("GuardianCorpse").Type, 1f); }
        }






        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/NPCs/LivingGrove/Coreling").Value;

            Vector2 position = NPC.Center - screenPos - new Vector2(0f, NPC.gfxOffY - 2f);
            Color color = Color.White;

            SpriteEffects spriteEffects = NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // spriteBatch.Draw(texture, position, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects, 1f);
            return true;
        }
    }

}