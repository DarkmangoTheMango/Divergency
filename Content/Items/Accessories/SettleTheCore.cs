using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Divergency.Content.Particles;
using ParticleLibrary;
using Microsoft.Xna.Framework;


namespace Divergency.Content.Items.Accessories
{
    public class SettleTheCore : ModItem
	{
		public override void SetStaticDefaults()
		{
			//.setdefault("Rapidity Glove");
			////.setdefault("Doubles your ranged weapons fire rate and enables Auto-Shoot, however ranged damage is decreased by 50% \n'Fire quicker than your own shadow!'");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(10);	
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
			

		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
		
			 player.GetDamage(DamageClass.Magic) *= 1.1f; // Increase ALL player damage by 100%
			 player.GetCritChance(DamageClass.Melee) *= 1.1f;
			 player.statDefense -= 10;
			 player.GetModPlayer<SettleTheCorePlayer>().Revenge = true;

            

		}




	}

	public class SettleTheCorePlayer : ModPlayer
	{
		public bool Revenge;
		public int RevengeCooldown;
		public int RevengePeriod;
        public override void ResetEffects() => Revenge = false;

        public override void OnHitAnything(float x, float y, Entity victim)
        {
			if (victim.Distance(Player.Center) <= 100 && RevengePeriod > 0)
			{
				Player.Heal(50);
				RevengeCooldown = 600;
				RevengePeriod = 0;
			}
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (Revenge && RevengeCooldown == 0)
			{
				RevengePeriod = 60;

			}
			
        }
        public override void PreUpdate()
        {
			if (Revenge && RevengePeriod > 0)
			{
                Vector2 spawnPosition = Player.Center;

                ParticleManager.NewParticle<FlareLineParticle>(spawnPosition + new Vector2(Main.rand.NextFloat(-20,20),0), new Vector2(Player.velocity.X, -3), new Color(0.50f, 2.05f, 0.5f, 0), 0.9f, Main.rand.NextFloat(0.8f, 1.1f));
            }
			if (RevengeCooldown > 0)
			{
                RevengeCooldown--; 
            }
            if (RevengePeriod > 0)
            {
                RevengePeriod--;
            }

        }

    }
	

}