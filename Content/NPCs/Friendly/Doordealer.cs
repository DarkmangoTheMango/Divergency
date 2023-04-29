
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Divergency.Content.NPCs.Friendly
{
    public class DoordealerFirstEncounter : ModNPC
    {
        public ref float AITimer => ref NPC.ai[1];

        public override void SetStaticDefaults()
        {
            //.setdefault("Mystery Man");
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = false;
            NPC.width = 24;
            NPC.height = 48;
            NPC.lifeMax = 999;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 0;
            NPC.scale = 1.3f;
            NPC.townNPC = true;
            NPC.homeless = true;
        }
        public override bool CanChat()
        {
            return true;
        }
        public override bool CanGoToStatue(bool toKingStatue) => true;
        public override bool UsesPartyHat() => false; // FOR NOW
        public override bool CanTownNPCSpawn(int numTownNPCs    ) => false;
        public override void AI()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.homeless = false;
                NPC.homeTileX = -1;
                NPC.homeTileY = -1;
                NPC.netUpdate = true;
            }
            NPC.direction = 1;

        }
        private static int ChatOptions = 0;
        public override void SetChatButtons(ref string button, ref string button2)
        {
           // button2 = "Next";

            switch (ChatOptions)
            {
                case 0:
                    button = "Please leave my property immediately.";
                    break;
                case 1:
                    button = "Doors?";
                    break;
                case 2:
                    button = "Not interested.";
                    break;
                case 3:
                    button = "What are you doing on my property again?";
                    break;
                case 4:
                    button = "I'll call the Police";
                    break;
                case 5:
                    button = "Don't talk like that";
                    break;
                case 6:
                    button = "I'd rather stay far away from your shop";
                    break;
                case 7:
                    button = "FINE whatever. Can you leave now?";
                    break;
       
            }
        }
        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            Player player = Main.LocalPlayer;
            if (firstButton)
            {
                Main.npcChatText = Chat();
                ChatOptions++;

            }
        }
        public static string Chat()
        {
            switch (ChatOptions)
            {
                case 0:
                    return "Hey, nice door you got there bud!";
                case 1:
                    return "You know I only wanted to greet my new neighbors and maybe show them some of my glorious doors!";
                case 2:
                    return "Aww cmon don't be shy! I can even give you a discount ticket. Here!";
                case 3:
                    return "I have no clue!";
                case 4:
                    return "Okay okay slow it down bud! I'm actually here to give you some insider information about a hidden treasure located in the Living Grove... Twop secwet if ywou know what I mwean...";
                case 5:
                    return "Look bud, head to the totally normal giant infallibe tree that suddenly grew out of nowhere last week. God I hate mondays. On the way you should find my shop and I'll tell you the information you need!";
                case 6:
                    return "Come onnn it's gonna be fun and if you bring me the treasure I'll reward you with my most valuable creation in return!";
                case 7:
                    return "Sure thing my favorite neighbor! We see us on the other side!!!";

            }
            return "...";
        }
        public override string GetChat()
        {
            Player player = Main.player[Main.myPlayer];
            WeightedRandom<string> chat = new(Main.rand);
            chat.Add("Wassuppppp!");

            return chat;
        }
    }

    //public class DoordealerSpawn : GlobalNPC
    //{

       // public override void AI(NPC npc)
        //{
            
         //   if (!npc.homeless && npc.townNPC)
          //  {
           //     NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.homeTileX, (int)npc.homeTileY, ModContent.NPCType<DoordealerFirstEncounter>());
            //}
       // }
    //}
 


}
