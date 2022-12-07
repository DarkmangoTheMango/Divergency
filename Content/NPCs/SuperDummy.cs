using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Divergency.Content.NPCs
{
    public class SuperDummy : ModNPC
    {
		public override void SetDefaults()
		{
			NPC.damage = 0;
			NPC.lifeMax = int.MaxValue - 1;
			NPC.defense = 0;
			NPC.knockBackResist = 0f;

			NPC.width = 28;
			NPC.height = 44;
			NPC.scale = 1f;

			NPC.HitSound = SoundID.NPCHit15;

			NPC.value = 0;

			NPC.aiStyle = -1;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] { BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
				new FlavorTextBestiaryInfoElement("A target dummy infused with the essence of pure life, he'll be your best friend forever!... as long as you stay on his good side...") });
		}

		public override void AI() { NPC.life = NPC.lifeMax + 1; }
    }

	public class SuperDummySpawner : ModItem
    {
		public override bool AltFunctionUse(Player player) => true;

        public override void SetStaticDefaults()
        {
			DisplayName.SetDefault("Super Dummy");
			Tooltip.SetDefault("Summons a Super Dummy");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

        public override void SetDefaults()
		{
			Item.width = Item.height = 16;
			Item.scale = 1f;

			Item.useTime = Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
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

					if (npc.type == ModContent.NPCType<SuperDummy>())
					{
						npc.active = false;
					}
				}
			}
			else
			{
				if (player.whoAmI == Main.myPlayer && Main.netMode != NetmodeID.Server)
				{
					NPC.NewNPC(Terraria.Entity.GetSource_NaturalSpawn(), (int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, ModContent.NPCType<SuperDummy>());
				}
			}

			return true;
		}
    }
}
