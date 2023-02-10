using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Divergency.Content.Dusts;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Localization;
using Divergency.Assets.Particles;
using Divergency.Common.Helpers;

namespace Divergency.Content.Tiles.LivingGrove.CorePuzzle
{
    public class LivingCorePodestTileUp : ModTile
    {
        public override string Texture => "Divergency/Content/Tiles/LivingGrove/CorePuzzle/LivingCorePodestUp";

        private Vector2 zero = Vector2.Zero;

        public override void SetStaticDefaults()
        {

            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            Main.tileLighted[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.Width = 2; // unless it's already 2 tiles wide
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(0, 2);

            TileObjectData.addTile(Type);
            Main.tileBouncy[Type] = true;

            AddMapEntry(new Color(120, 85, 60), Language.GetText("Podest"));
            DustType = 7;

        }
        public override bool RightClick(int i, int j)
        {
            Vector2 speed = new Vector2(0f, -10f);

            int left = i - Main.tile[i, j].TileFrameX / 18;
            int top = j - Main.tile[i, j].TileFrameY / 18;


            Vector2 pos = new Vector2(left * 16f + 8f, top * 16f + 8f);
            Player player = Main.LocalPlayer;
            if (!Main.tileLighted[Type])
            {
                if (player.GetModPlayer<CorePuzzle>().LivingCoreAmount != 0)
                {
                    Projectile.NewProjectile(null, pos, speed, ModContent.ProjectileType<PodestProjectile>(), 0, 0);
                    player.GetModPlayer<CorePuzzle>().LivingCoreAmount--;
                    Main.tileLighted[Type] = true;
                    SoundEngine.PlaySound(SoundID.NPCDeath44 with { Volume = 1f, Pitch = Main.rand.NextFloat(0.5f, 2f), MaxInstances = 400 });



                }
                else
                {

                }
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.Unlock, -1, -1, null, player.whoAmI, 1f, left, top);
            }
            return true;
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {

            if (Main.tileLighted[Type])
            {


                r = 1.45f;
                g = 2.55f;
                b = 0.94f;
            }
            else if (!Main.tileLighted[Type])
            {
                r = 0f;
                g = 0f;
                b = 0f;
            }

        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            //Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<Items.Placeable.Furniture.MinionBossTrophy>());
        }
        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = 2;
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/CorePuzzle/LivingCorePodestUp").Value;
            Texture2D tex2 = ModContent.Request<Texture2D>("Divergency/Content/Tiles/LivingGrove/CorePuzzle/LivingCorePodestUpCharged").Value;
            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {
                if (!Main.tileLighted[Type])
                {
                    spriteBatch.Draw(tex, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero + new Vector2(0, 13), Color.White);
                }
                else if (Main.tileLighted[Type])
                {
                    spriteBatch.Draw(tex2, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero + new Vector2(0, 9), Color.White);

                }
            }

            return false;

        }

    }
    internal class LivingCorePodestUp : ModItem
    {
        public override string Texture => "Divergency/Content/Tiles/LivingGrove/CorePuzzle/LivingCorePodestUpCharged";


        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.value = 1000;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.rare = ItemRarityID.White;
            Item.createTile = ModContent.TileType<LivingCorePodestTileUp>();

        }
    }

    public class PodestProjectile : ModProjectile
    {
        private Vector2 tilePos = Vector2.Zero;
        public static int DoubleCooldown;

        public override string Texture => "Divergency/Assets/Textures/Empty";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Core Blast");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

        }
        public override void SetDefaults()
        {
            Projectile.damage = 100;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.height = 20;
            Projectile.width = 10;
            Projectile.friendly = true;
            Projectile.scale = 1f;
            Projectile.timeLeft = 3000;

        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void OnSpawn(IEntitySource source)
        {

        }

        public override void AI()
        {

            Timer++;

                for (int i = 0; i < 2; i++)
                {

                    Vector2 dir = Main.rand.NextVector2Unit() * 0.1f;

                    ParticleManager.NewParticle(Projectile.Center, dir * 10, ParticleManager.NewInstance<StarParticle>(), new Color(0.50f, 2f, 0.5f, 0), 0.1f, Projectile.whoAmI);
                }

            if (Timer == 1)
            {
                ParticleManager.NewParticle(Projectile.Center, new Vector2(0, 0), ParticleManager.NewInstance<BloomParticleProjectile>(), new Color(0.50f, 2f, 0.5f, 0), 0.04f, Projectile.whoAmI, Layer: Particle.Layer.BeforeProjectiles);

            }
            if (DoubleCooldown >= 1)
            {
                DoubleCooldown--;
            }
            drawcooldown++;
            Vector3 RGB = new Vector3(1.45f, 2.55f, 0.94f);
            float multiplier = 0.4f;
            float max = 1f;
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
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }
            Vector2 pos = Projectile.position;
            for (int i = -5; i <= 5; i++)
            {
                bool success = TryFindNearPodest(pos + new Vector2(i * 16f, 0f), out Vector2 result);
                result = tilePos;


            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
            Projectile.spriteDirection = Projectile.direction;
            if (

                     Main.tile[(int)Projectile.position.X / 16, (int)Projectile.position.Y / 16].TileType == ModContent.TileType<CoreMirrorTileDown>()
                    || Main.tile[(int)Projectile.position.X / 16, (int)Projectile.position.Y / 16].TileType == ModContent.TileType<CoreMirrorTileUp>()
                    || Main.tile[(int)Projectile.position.X / 16, (int)Projectile.position.Y / 16].TileType == ModContent.TileType<CoreMirrorTileRight>()
                    || Main.tile[(int)Projectile.position.X / 16, (int)Projectile.position.Y / 16].TileType == ModContent.TileType<CoreMirrorTileLeft>()
                    || Main.tile[(int)Projectile.position.X / 16, (int)Projectile.position.Y / 16].TileType == ModContent.TileType<XORCoreTile>()
                    || Main.tile[(int)Projectile.position.X / 16, (int)Projectile.position.Y / 16].TileType == ModContent.TileType<ANDCoreTile>()
                    || Main.tile[(int)Projectile.position.X / 16, (int)Projectile.position.Y / 16].TileType == ModContent.TileType<CoreDoublerLeftUpTile>()
                    || Main.tile[(int)Projectile.position.X / 16, (int)Projectile.position.Y / 16].TileType == ModContent.TileType<CoreDoublerDownLeftTile>()
                    || Main.tile[(int)Projectile.position.X / 16, (int)Projectile.position.Y / 16].TileType == ModContent.TileType<CoreDoublerRightDownTile>()
                    || Main.tile[(int)Projectile.position.X / 16, (int)Projectile.position.Y / 16].TileType == ModContent.TileType<CoreDoublerUpRightTile>()
                    || Main.tile[(int)Projectile.position.X / 16, (int)Projectile.position.Y / 16].TileType == ModContent.TileType<CoreRootsTile>()
                    || Main.tile[(int)Projectile.position.X / 16, (int)Projectile.position.Y / 16].TileType == ModContent.TileType<CoreRootsTile1>()
                    || Main.tile[(int)Projectile.position.X / 16, (int)Projectile.position.Y / 16].TileType == ModContent.TileType<CoreRootsTile2>())
            {
                // Origin position, in tile format.
                int x = (int)(Projectile.position.X / 16);
                int y = (int)(Projectile.position.Y / 16);

                // Position being checked;
                SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFuryShot with { Volume = 0.7f, Pitch = Main.rand.NextFloat(0.8f, 2f), MaxInstances = 400 });


                int checkX = x;
                int checkY = y;
                Tile tile = Framing.GetTileSafely(checkX, checkY);



                // Checking up to a maximum of 30 tiles.
                for (int b = 0; b < 1; b++)
                {

                    if (Main.tile[checkX, checkY].TileType == ModContent.TileType<CoreMirrorTileDown>())

                    {
                        Projectile.velocity = new Vector2(0, 10);
                        if (drawcooldown > 3)
                        {
                            DivergencyDraw.SpawnRing(Projectile.Center, Color.LimeGreen, 0.08f, 0.8f, 1);
                            drawcooldown = 0;
                        }

                    }
                    if (tile.TileType == ModContent.TileType<CoreMirrorTileUp>())

                    {
                        Projectile.velocity = new Vector2(0, -10);
                        if (drawcooldown > 3)
                        {
                            DivergencyDraw.SpawnRing(Projectile.Center, Color.LimeGreen, 0.08f, 0.8f, 1);
                            drawcooldown = 0;
                        }
                    }
                    if (tile.TileType == ModContent.TileType<CoreMirrorTileRight>())

                    {
                        Projectile.velocity = new Vector2(10, 0);
                        if (drawcooldown > 3)
                        {
                            DivergencyDraw.SpawnRing(Projectile.Center, Color.LimeGreen, 0.08f, 0.8f, 1);
                            drawcooldown = 0;
                        }
                    }
                    if (Main.tile[checkX, checkY].TileType == ModContent.TileType<CoreMirrorTileLeft>())

                    {
                        Projectile.velocity = new Vector2(-10, 0);
                        if (drawcooldown > 3)
                        {
                            DivergencyDraw.SpawnRing(Projectile.Center, Color.LimeGreen, 0.08f, 0.8f, 1);
                            drawcooldown = 0;
                        }
                    }

                    if (Main.tile[checkX, checkY].TileType == ModContent.TileType<CoreRootsTile>() && !Main.tileLighted[ModContent.TileType<CoreRootsTile>()])
                    {
                        Projectile.Kill();
                        Main.tileLighted[ModContent.TileType<CoreRootsTile>()] = true;
                        DivergencyDraw.SpawnExplosion(Projectile.Center, Color.LimeGreen, DustID.GemEmerald, scale: 1.4f);
                        SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.7f, Pitch = Main.rand.NextFloat(0.8f, 2f), MaxInstances = 400 });


                    }
                    if (Main.tile[checkX, checkY].TileType == ModContent.TileType<CoreRootsTile1>() && !Main.tileLighted[ModContent.TileType<CoreRootsTile1>()])
                    {
                        Projectile.Kill();
                        Main.tileLighted[ModContent.TileType<CoreRootsTile1>()] = true;
                        DivergencyDraw.SpawnExplosion(Projectile.Center, Color.LimeGreen, DustID.GemEmerald, scale: 1.4f);
                        SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.7f, Pitch = Main.rand.NextFloat(0.8f, 2f), MaxInstances = 400 });

                    }
                    if (Main.tile[checkX, checkY].TileType == ModContent.TileType<CoreRootsTile2>() && !Main.tileLighted[ModContent.TileType<CoreRootsTile2>()])
                    {
                        Projectile.Kill();

                        Main.tileLighted[ModContent.TileType<CoreRootsTile2>()] = true;
                        DivergencyDraw.SpawnExplosion(Projectile.Center, Color.LimeGreen, DustID.GemEmerald, scale: 1.4f);
                        SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.7f, Pitch = Main.rand.NextFloat(0.8f, 2f), MaxInstances = 400 });

                    }

                    if (Main.tile[checkX, checkY].TileType == ModContent.TileType<XORCoreTile>() && Main.tileLighted[ModContent.TileType<XORCoreTile>()])
                    {
                        Main.tileBouncy[ModContent.TileType<XORCoreTile>()] = true;

                        Projectile.Kill();
                    }
                    if (Main.tile[checkX, checkY].TileType == ModContent.TileType<XORCoreTile>() && !Main.tileLighted[ModContent.TileType<XORCoreTile>()])
                    {
                        Main.tileLighted[ModContent.TileType<XORCoreTile>()] = true;

                        Projectile.Kill();
                    }

                    if (Main.tile[checkX, checkY].TileType == ModContent.TileType<ANDCoreTile>() && Main.tileLighted[ModContent.TileType<ANDCoreTile>()])
                    {
                        Main.tileBouncy[ModContent.TileType<ANDCoreTile>()] = true;

                        Projectile.Kill();
                    }
                    if (Main.tile[checkX, checkY].TileType == ModContent.TileType<ANDCoreTile>() && !Main.tileLighted[ModContent.TileType<ANDCoreTile>()])
                    {
                        Main.tileLighted[ModContent.TileType<ANDCoreTile>()] = true;

                        Projectile.Kill();
                    }
                    if (Main.tile[checkX, checkY].TileType == ModContent.TileType<CoreDoublerUpRightTile>())
                    {
                        Vector2 up = new Vector2(0f, -10f);
                        Vector2 right = new Vector2(10f, 0f);

                        if (!Main.tileLighted[ModContent.TileType<CoreDoublerUpRightTile>()])
                        {
                            Projectile.NewProjectile(null, Projectile.Center, up, ModContent.ProjectileType<PodestProjectile>(), 0, 0);
                            Projectile.NewProjectile(null, Projectile.Center, right, ModContent.ProjectileType<PodestProjectile>(), 0, 0);
                            Main.tileLighted[ModContent.TileType<CoreDoublerUpRightTile>()] = true;
                            Projectile.Kill();


                        }


                    }
                    if (Main.tile[checkX, checkY].TileType == ModContent.TileType<CoreDoublerLeftUpTile>())
                    {
                        Vector2 up = new Vector2(0f, -10f);
                        Vector2 left = new Vector2(-10f, 0f);

                        if (!Main.tileLighted[ModContent.TileType<CoreDoublerLeftUpTile>()])
                        {
                            Projectile.NewProjectile(null, Projectile.Center, up, ModContent.ProjectileType<PodestProjectile>(), 0, 0);
                            Projectile.NewProjectile(null, Projectile.Center, left, ModContent.ProjectileType<PodestProjectile>(), 0, 0);
                            Main.tileLighted[ModContent.TileType<CoreDoublerLeftUpTile>()] = true;
                            Projectile.Kill();

                        }




                    }
                    if (Main.tile[checkX, checkY].TileType == ModContent.TileType<CoreDoublerDownLeftTile>())
                    {
                        Vector2 down = new Vector2(0f, 10f);
                        Vector2 left = new Vector2(-10f, 0f);

                        if (!Main.tileLighted[ModContent.TileType<CoreDoublerDownLeftTile>()])
                        {
                            Projectile.NewProjectile(null, Projectile.Center, down, ModContent.ProjectileType<PodestProjectile>(), 0, 0);
                            Projectile.NewProjectile(null, Projectile.Center, left, ModContent.ProjectileType<PodestProjectile>(), 0, 0);
                            Main.tileLighted[ModContent.TileType<CoreDoublerDownLeftTile>()] = true;
                            Projectile.Kill();

                        }




                    }
                    if (Main.tile[checkX, checkY].TileType == ModContent.TileType<CoreDoublerRightDownTile>())
                    {
                        Vector2 down = new Vector2(0f, 10f);
                        Vector2 right = new Vector2(10f, 0f);


                        if (!Main.tileLighted[ModContent.TileType<CoreDoublerRightDownTile>()])
                        {
                            Projectile.NewProjectile(null, Projectile.Center, down, ModContent.ProjectileType<PodestProjectile>(), 0, 0);
                            Projectile.NewProjectile(null, Projectile.Center, right, ModContent.ProjectileType<PodestProjectile>(), 0, 0);
                            Main.tileLighted[ModContent.TileType<CoreDoublerRightDownTile>()] = true;
                            Projectile.Kill();


                        }




                    }
                }
            }

        }


        public bool TryFindNearPodest(Vector2 position, out Vector2 result)
        {
            if (Main.tile[(int)position.X / 16, (int)position.Y / 16].TileType == ModContent.TileType<LivingCorePodestTileUp>())
            {
                // Origin position, in tile format.
                int x = (int)(position.X / 16);
                int y = (int)(position.Y / 16);

                // Position being checked;

                int checkX = x;
                int checkY = y;

                // Checking up to a maximum of 30 tiles.
                for (int b = 0; b < 30; b++)
                {
                    // If this position is in the world, and if the tile is a Tree tile.
                    if (WorldGen.InWorld(checkX, y) && Main.tile[checkX, checkY].TileType == ModContent.TileType<LivingCorePodestTileUp>())
                    {
                        // Checking if the tile's frames are within the range of tile frames used for the invisible tree top tiles.

                        //Dust.QuickBox(new Vector2(checkX * 16, checkY * 16), new Vector2((checkX * 16) + 16, (checkY * 16) + 16), 10, Color.Yellow, null);
                        result = new Vector2(checkX * 16, checkY * 16);

                        // Otherwise, its a success, since it's still a tree tile. Just not the one we're looking for.
                        //Dust.QuickBox(new Vector2(checkX * 16, checkY * 16), new Vector2((checkX * 16) + 16, (checkY * 16) + 16), 10, Color.Green, null);
                        return true;

                    }
                    else
                    {
                        // If the tile isn't what we're looking for and since we're only iterating upwards, logically this means its useless to continue.
                        //Dust.QuickDustLine(new Vector2(checkX * 16, checkY * 16), new Vector2((checkX * 16) + 16, (checkY * 16) + 16), 5f, Color.Red);
                        //Dust.QuickDustLine(new Vector2(checkX * 16, (checkY * 16) + 16), new Vector2((checkX * 16) + 16, checkY * 16), 5f, Color.Red);
                        break;
                    }
                }
            }


            result = default;
            return false;
        }
        public Trail trail;
        public Trail trail2;
        private int drawcooldown;

        public override bool PreDraw(ref Color lightColor)
        {



            Texture2D trailTexture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/MotionTrail").Value;

            if (trail == null)
            {
                trail = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(8f), (p) => Projectile.GetAlpha(new Color(30, 220, 30, 100)));
                trail.drawOffset = Projectile.Size / 2f;
            }
            if (trail2 == null)
            {
                trail2 = new Trail(trailTexture, Trail.DefaultPass, (p) => new Vector2(4f), (p) => Projectile.GetAlpha(new Color(255, 255, 255, 100)));
                trail2.drawOffset = Projectile.Size / 2f;
            }

            trail.Draw(Projectile.oldPos);
            trail2.Draw(Projectile.oldPos);

            return false;
        }





    }
}