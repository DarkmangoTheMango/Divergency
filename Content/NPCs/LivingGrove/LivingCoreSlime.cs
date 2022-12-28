using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.NPCs.LivingGrove
{
    [AutoloadBossHead]

    public class LivingCoreSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;
            
			NPCID.Sets.TrailCacheLength[NPC.type] = 10;
			NPCID.Sets.TrailingMode[NPC.type] = 0;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Velocity = 0f };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 200;
            NPC.damage = 40;
            NPC.defense = 25;
            NPC.knockBackResist = 0.1f;

            NPC.noTileCollide = false;

            NPC.scale = 1f;
            NPC.Size = new Vector2(32f);

            NPC.HitSound = SoundID.DD2_WitherBeastHurt;
            NPC.DeathSound = SoundID.DD2_WitherBeastDeath;
            NPC.value = Item.sellPrice(0, 0, 0, 0);

            NPC.aiStyle = NPCAIStyleID.Slime;
            NPC.noGravity = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            int frameHeight = texture.Height / Main.projFrames[NPC.type];

            Rectangle sourceRectangle = new Rectangle(0, NPC.frame.Y * frameHeight, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            for (int i = 1; i < NPC.oldPos.Length; i++)
            {
                Vector2 position = NPC.oldPos[i] - Main.screenPosition + new Vector2(0f, NPC.gfxOffY);
                Color color = Color.White * ((NPCID.Sets.TrailCacheLength[NPC.type] - i) / (float)NPCID.Sets.TrailCacheLength[NPC.type]);

                spriteBatch.Draw(texture, position, sourceRectangle, color, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            }

            return true;
        }
    }

    public class LivingCoreSlimeSpawner : ModItem
    {
        public override bool AltFunctionUse(Player player) => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Core Slime");
            Tooltip.SetDefault("Summons a Guardian Cluster");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 16;
            Item.scale = 1f;

            Item.useTime = Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = new SoundStyle("Divergency/Assets/Sounds/Items/SwingStyleNotYippee");
            Item.autoReuse = true;
            Item.useTurn = false;

            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.rare = ItemRarityID.Blue;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC npc = Main.npc[k];
                    if (npc.type == ModContent.NPCType<Coreling>()) { npc.active = false; }
                }
            }
            else { if (player.whoAmI == Main.myPlayer && Main.netMode != NetmodeID.Server) { NPC.NewNPC(Terraria.Entity.GetSource_NaturalSpawn(), (int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, ModContent.NPCType<LivingCoreSlime>()); } }

            return true;
        }
    }
}