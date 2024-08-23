using Divergency.Content.Dusts;
using Divergency.Content.Projectiles.Hostile;
using Divergency.Content.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Divergency.Common;
using Divergency.Content.Items.Weapons.LivingCore;
using Divergency.Content.Projectiles;
using Divergency.Common.Helpers;
using Divergency.Common.Players;

namespace Divergency.Content.NPCs.LivingGrove
{
    public class CoreVacuum : ModNPC
    {
        enum State
        {
           initialize,
           dashing,
           sucking

        }
        State state = State.initialize;

        public int dashcooldown { get; private set; }
        public int initialDamage { get; private set; }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 150;
            NPC.damage = 50;
            NPC.defense = 15;
            NPC.knockBackResist = 0.7f;

            NPC.noTileCollide = false;

            NPC.scale = 1f;
            NPC.Size = new Vector2(52f, 32f);

            NPC.HitSound = SoundID.DD2_WitherBeastHurt;
            NPC.DeathSound = SoundID.DD2_WitherBeastDeath;
            NPC.value = Item.sellPrice(0, 0, 0, 0);

            NPC.aiStyle = -1;
            NPC.noGravity = false;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("He just took 3 gas station dick pills")
            });
        }

        public override void AI()
        {
            Player target = Main.player[NPC.target];
            NPC.TargetClosest(true);

            NPC.velocity /= 1.02f;
            if (state == State.initialize)
            {
                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0), ModContent.ProjectileType<CoreVacuumTrail>(), 0, 0);
                state = State.dashing;
            }
            if (state == State.dashing)
            {
                NPC.ai[0]++;
                
                if (NPC.ai[0] < 60)
                {
                    NPC.velocity.Y -= 0.1f;
                }

                if (NPC.ai[0] == 60)
                {
                    NPC.velocity += NPC.Center.DirectionTo(target.Center) * 15;
                    NPC.velocity.Y -= 10;

                }
                if (NPC.ai[0] == 120)
                {
                    NPC.velocity += NPC.Center.DirectionTo(target.Center) * 15;
                    NPC.velocity.Y -= 10;

                }
                if (NPC.ai[0] == 180)
                {
                    NPC.velocity += NPC.Center.DirectionTo(target.Center) * 15;
                    NPC.velocity.Y -= 10;

                }
                //dash up
                if (NPC.ai[0] == 240)
                {
                    NPC.velocity.Y -= 5;
                    NPC.noGravity = true;
                }
                if (NPC.ai[0] == 300)
                {
                    state = State.sucking;
                    NPC.ai[0] = 0;
                }
            }

            //succ

            if (state == State.sucking)
            {
                NPC.ai[0]++;
                Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);

                Dust dust = Dust.NewDustPerfect(NPC.Center + (velocity * 1000f), ModContent.DustType<Glow>(), velocity * -20f, 0, Color.LimeGreen, 0.7f);
                Dust dust2 = Dust.NewDustPerfect(target.Center + (velocity * Main.rand.NextFloat(-10,10)), ModContent.DustType<GlowLine>(), velocity * target.DirectionTo(NPC.Center) * 20, 0, Color.LimeGreen, 0.1f);

                target.velocity += target.DirectionTo(NPC.Center) * 0.2f;
                target.velocity.Y -= 0.3f;
                target.gravity = 0;
                if (NPC.ai[0] == 35)
                {

                    DivergencyDraw.SpawnRingReverse(NPC.Center, Color.LimeGreen);
                    NPC.ai[0] = 0;

                }
            }
        }



        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Divergency/Content/NPCs/LivingGrove/CoreElemental").Value;

            Vector2 position = NPC.Center - screenPos - new Vector2(0f, NPC.gfxOffY - 2f);
            Color color = Color.White;

            SpriteEffects spriteEffects = NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

           // spriteBatch.Draw(texture, position, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects, 1f);
            return true;
        }
    }

    public class CoreVacuumTrail : ModProjectile
    {
        public override string Texture => "Divergency/Assets/Textures/Empty";
        public override void SetStaticDefaults()
        {

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public Projectile cachedProjectile { get; private set; }
        public bool spawned { get; private set; }
        public NPC cachedNPC { get; private set; }

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;

            Projectile.width = Projectile.height = 0;
            Projectile.damage = 0;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
        }
        public override void AI()
        {
            if (!spawned)
            {
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC taggedNPC = Main.npc[k];

                    if (taggedNPC.active && taggedNPC.ModNPC is CoreVacuum)
                    {
                        cachedNPC = taggedNPC;
                    }
                }

                spawned = true;
            }

            if (!cachedNPC.active)
            {
                Projectile.active = false;
            }
            if (spawned)
            {
                Projectile.Center = cachedNPC.Center;
            }
        }
        public Trail trail;
        public Trail trail2;

        public bool Particlespawned { get; private set; }

        public override bool PreDraw(ref Color lightColor)
        {



            Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Stretched").Value;

            if (trail == null)
            {
                trail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(12f), (p) => Projectile.GetAlpha(new Color(30, 220, 30, 100)));
                trail.drawOffset = Projectile.Size / 2f;
            }
            if (trail2 == null)
            {
                trail2 = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(5f), (p) => Projectile.GetAlpha(new Color(255, 255, 255, 100)));
                trail2.drawOffset = Projectile.Size / 2f;
            }

            trail.Draw(Projectile.oldPos);
            trail2.Draw(Projectile.oldPos);

            return false;
        }

    }
}
    
    
