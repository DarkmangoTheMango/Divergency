using Humanizer;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Divergency.Common.Helpers.SwordAnimator
{
    public class TimedFunction
    {
        private bool RunEveryFrame;

        private float Frame = -1;
        private float Percent = -1;

        private Action<Projectile> FunctionToRun;

        public void TryRun(Projectile proj, float curTime, float lastTime, float TotalTime)
        {
            float TargetTime = Frame != -1 ? Frame : (int)MathF.Floor(Percent * TotalTime);

            if (RunEveryFrame)
            {
                if (TargetTime <= curTime)
                    FunctionToRun(proj);
            }
            else
            {
                //Console.WriteLine(TargetTime + " | " + curTime + " | " + TargetTime + " | " + lastTime + ": " + (TargetTime <= curTime && TargetTime >= lastTime));
                if (TargetTime <= curTime && TargetTime >= lastTime)
                    FunctionToRun(proj);
            }
        }

        public TimedFunction(Action<Projectile> FunctionToRun, int Frame = 0, bool RunEveryFrame = false)
        {
            this.RunEveryFrame = RunEveryFrame;
            this.Frame = (float)Frame;

            this.FunctionToRun = FunctionToRun;
        }
        public TimedFunction(Action<Projectile> FunctionToRun, float Percent = 0, bool RunEveryFrame = false)
        {
            this.RunEveryFrame = RunEveryFrame;
            this.Percent = Percent;

            this.FunctionToRun = FunctionToRun;
        }
    }
    public class SwordAnimator : ModSystem
    {
        public static int Swing(Player player, int itemID, int damage, float knockback)
        {
            int projectileID = Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, new Vector2(player.direction, 0), ModContent.ProjectileType<SwordProjectile>(), damage, knockback, player.whoAmI);
            Projectile projectile = Main.projectile[projectileID];

            ISwordSwing SS = (ModContent.GetModItem(itemID) as ISwordSwing);

            projectile.timeLeft = 2;

            projectile.extraUpdates = SS.Updates - 1;
            projectile.localNPCHitCooldown = SS.NPCHitCooldown * SS.Updates; // maby change..?

            SwordProjectile projSword = projectile.ModProjectile as SwordProjectile;
            projSword.baseItem = itemID;
            projSword.AttackSpeed = player.GetTotalAttackSpeed(DamageClass.Melee); // idk how speed is calculated...
            // above might just work a-ok

            projSword.Trails = new Trail[SS.SwordTrails.Length];

            int longestTrail = 0;
            for (int trailIDX = 0; trailIDX < SS.SwordTrails.Length; trailIDX++)
            {
                projSword.Trails[trailIDX] = SS.SwordTrails[trailIDX].MakeTrailFunc(projectile, SS.SwordTrails[trailIDX].Texture);
                longestTrail = SS.SwordTrails[trailIDX].TrailLength;
            }

            if (longestTrail > 0)
            {
                projSword.trailPositions = new Vector2[longestTrail];
                projSword.trailRotations = new float[longestTrail];
            }

            projectile.netUpdate = true;

            return projectileID;
        }

        public static int Swing(NPC npc, int itemID, int damage, float knockback, float attackspeed = 1)
        {
            int projectileID = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(npc.direction, 0), ModContent.ProjectileType<SwordProjectile>(), damage, knockback);
            Projectile projectile = Main.projectile[projectileID];

            ISwordSwing SS = (ModContent.GetModItem(itemID) as ISwordSwing);

            projectile.timeLeft = 2;

            projectile.extraUpdates = SS.Updates - 1;
            projectile.localNPCHitCooldown = SS.NPCHitCooldown * SS.Updates; // maby change..?
            projectile.hostile = true;
            projectile.friendly = false;

            SwordProjectile projSword = projectile.ModProjectile as SwordProjectile;
            projSword.baseItem = itemID;
            projSword.AttackSpeed = attackspeed;
            projSword.NPCOwned = npc.whoAmI;

            projSword.Trails = new Trail[SS.SwordTrails.Length];

            int longestTrail = 0;
            for (int trailIDX = 0; trailIDX < SS.SwordTrails.Length; trailIDX++)
            {
                projSword.Trails[trailIDX] = SS.SwordTrails[trailIDX].MakeTrailFunc(projectile, SS.SwordTrails[trailIDX].Texture);
                longestTrail = SS.SwordTrails[trailIDX].TrailLength;
            }

            if (longestTrail > 0)
            {
                projSword.trailPositions = new Vector2[longestTrail];
                projSword.trailRotations = new float[longestTrail];
            }

            projectile.netUpdate = true;

            return projectileID;
        }
    }

    public class Keyframes
    {
        public float TotalTime;
        public SwordAnimation[] keyframeArray;

        public Keyframes(SwordAnimation[] incommingArray)
        {
            if (incommingArray.Length > 0)
                incommingArray[0].Duration = incommingArray[0].SDelay = incommingArray[0].EDelay = 0;

            foreach (SwordAnimation SA in incommingArray)
            {
                TotalTime += SA.Duration + SA.EDelay + SA.SDelay;
            }

            keyframeArray = incommingArray;

            /*
            int curIDX = 0;

            for (int SAIDX = 1; SAIDX < realArray.Length; SAIDX++)
            {
                SwordAnimation SA = realArray[SAIDX];
                SA.LastKeyframe = curIDX;

                for (int i = 0; i < (SA.Duration); i++)
                {
                    lookupArray.Add(SAIDX);
                    curIDX++;
                }

                SA.ThisKeyframe = curIDX - 1; // MIGHT BE WRONG

                for (int i = 0; i < (SA.Delay); i++)
                {
                    lookupArray.Add(SAIDX);
                    curIDX++;
                }
            }
            */
        }

        public struct FrameInfo
        {
            public int frameID;
            public float framePassed;

            public FrameInfo(int frameID, float framePassed)
            {
                this.frameID = frameID; this.framePassed = framePassed;
            }
        }

        public FrameInfo NextFrame(float curTime)
        {
            float collectiveTime = 0;

            if (curTime < 0)
                return new FrameInfo(0, 0);

            for (int i = 0; i < keyframeArray.Length; i++)
            {
                SwordAnimation SA = keyframeArray[i];

                float lastCollectiveTime = collectiveTime;

                collectiveTime += SA.Duration + SA.SDelay + SA.EDelay;

                if (collectiveTime > curTime)
                {
                    return new FrameInfo(i, curTime - lastCollectiveTime);
                }
            }

            return new FrameInfo(-1, 0);
        }
    }

    public class SwordAnimation
    {
        public static float linear(float time, float maxTime)
        {
            return time / maxTime;
        }
        public static float one(float time, float maxTime)
        {
            return 1f;
        }
        public static void chargeS(Projectile proj) { }
        public static void chargeE(Projectile proj, float charge, float maxCharge) { }

        public float TargetRotation;
        public Func<float, float, float> RotationIn; // In -> interpolator
        public Func<float, float, float> RotationMul; // Mul -> multiplier

        public Vector2 GlobalOffset;
        public Func<float, float, float> GlobalOffsetIn;
        public Func<float, float, float> GlobalOffsetMul;

        public Vector2 LocalOffset;
        public Func<float, float, float> LocalOffsetIn;
        public Func<float, float, float> LocalOffsetMul;

        public Vector2 Scale;
        public Func<float, float, float> ScaleIn;
        public Func<float, float, float> ScaleMul;

        public TimedFunction[] FrameFunctions;

        public float Duration; // The duration the animation takes
        public float SDelay; // Dealy till animation starts...
        public float EDelay; // Dealy till animation ends...

        public Action<Projectile> ChargeStart;
        public Action<Projectile, float, float> ChargeEnd;
        public bool ChargeAutoRelease;
        public float MaxCharge;

        // public bool CheckMouseClick; something to be considered for future, ig

        public bool HoldToContinue;
        public bool Flipped;

        public SwordAnimation(
            float TargetRotation, float Duration, float SDelay = 0, float EDelay = 0, bool Flipped = false,
            Vector2 GlobalOffset = default, Vector2 LocalOffset = default, Vector2 Scale = default,
            bool HoldToContinue = false, bool ChargeAutoRelease = false, float MaxCharge = 0,
            Action<Projectile> ChargeStart = default, Action<Projectile, float, float> ChargeEnd = default,

            Func<float, float, float> SharedIn = null, Func<float, float, float> RotationIn = null,
            Func<float, float, float> GlobalOffsetIn = null, Func<float, float, float> LocalOffsetIn = null,
            Func<float, float, float> ScaleIn = null,

            Func<float, float, float> SharedMul = null, Func<float, float, float> RotationMul = null,
            Func<float, float, float> GlobalOffsetMul = null, Func<float, float, float> LocalOffsetMul = null,
            Func<float, float, float> ScaleMul = null,
            
            TimedFunction[] FrameFunctions = null)
        {
            this.TargetRotation = TargetRotation;
            this.GlobalOffset = GlobalOffset;
            this.LocalOffset = LocalOffset;
            this.Scale = Scale == default ? Vector2.One : Scale;

            this.HoldToContinue = MaxCharge == 0 ? HoldToContinue : false;
            this.Flipped = Flipped;

            this.FrameFunctions = FrameFunctions == null ? new TimedFunction[] {} : FrameFunctions;

            this.MaxCharge = MaxCharge;
            this.ChargeAutoRelease = ChargeAutoRelease;
            this.ChargeStart = ChargeStart == null ? chargeS : ChargeStart;
            this.ChargeEnd = ChargeEnd == null ? chargeE : ChargeEnd;

            // In
            this.RotationIn = this.GlobalOffsetIn = this.LocalOffsetIn = this.ScaleIn = (SharedIn == null ? linear : SharedIn);

            if (RotationIn != null)
                this.RotationIn = RotationIn;

            if (GlobalOffsetIn != null)
                this.GlobalOffsetIn = GlobalOffsetIn;

            if (LocalOffsetIn != null)
                this.LocalOffsetIn = LocalOffsetIn;

            if (ScaleIn != null)
                this.ScaleIn = ScaleIn;

            // Mul
            this.RotationMul = this.GlobalOffsetMul = this.LocalOffsetMul = this.ScaleMul = (SharedMul == null ? one : SharedMul);

            if (RotationMul != null)
                this.RotationMul = RotationMul;

            if (GlobalOffsetMul != null)
                this.GlobalOffsetMul = GlobalOffsetMul;

            if (LocalOffsetMul != null)
                this.LocalOffsetMul = LocalOffsetMul;

            if (ScaleMul != null)
                this.ScaleMul = ScaleMul;

            this.Duration = Duration;
            this.SDelay = SDelay;
            this.EDelay = EDelay;
        }
    }
    /*
    public enum SwordTrailBase
    {
        Handle,
        Center,
        Tip,
    }
    */
    public class SwordTrail
    {
        static Trail BaseFunction(Projectile proj, Texture2D Texture)
        {
            return new Trail(Texture, Trail.DefaultPass, (p) => new Vector2(200f), (p) => proj.GetAlpha(new Color(255, 255, 255, 255)));
        }

        public Texture2D Texture; // "Divergency/Assets/Textures/Trails/Stretched"
        //public SwordTrailBase TrailBase; this might be worthless...
        //public Vector2 TrailOffset; ^^
        public Func<Projectile, Texture2D, Trail> MakeTrailFunc;
        public int TrailLength;

        public SwordTrail(string Texture, int TrailLenght, Func<Projectile, Texture2D, Trail> MakeTrailFunction = default)
        {
            this.Texture = ModContent.Request<Texture2D>(Texture).Value;
            //this.TrailBase = TrailBase;
            //this.TrailOffset = Offset;
            this.MakeTrailFunc = MakeTrailFunc == null ? BaseFunction : MakeTrailFunc;
            this.TrailLength = TrailLenght;
        }

        /*
        Texture2D texture = ModContent.Request<Texture2D>("Divergency/Assets/Textures/Trails/Stretched").Value;

        if (trail == null)
        {
            trail = new Trail(texture, Trail.DefaultPass, (p) => new Vector2(20f), (p) => Projectile.GetAlpha(new Color(255, 108, 23, 100)));
            trail.drawOffset = Projectile.Size / 2f;

            whiteTrail = new Trail(texture, Trail.DefaultPass, (p) => new Vector2(10f), (p) => Projectile.GetAlpha(new Color(255, 247, 179, 100)));
            whiteTrail.drawOffset = Projectile.Size / 2f;
        }
        */
    }

    public interface ISwordSwing
    {
        public float Width { get; }
        public string SwordTexture { get; }
        public Keyframes SwordFrames { get; }
        public float Height { get { return -1; } } // height based off of texture
        public Vector2 Pivot { get { return default; } }
        public Func<Projectile, bool> PreDraw { get { return null; } }
        public Func<Projectile, Vector2, bool> OnHitTile { get { return null; } }
        public Action<Projectile, NPC, int, float, bool> OnHitNPC { get { return null; } }
        public TimedFunction[] SwingFunctions { get { return new TimedFunction[] { }; } }
        public int Updates { get { return 1; } }
        public int NPCHitCooldown { get { return 10; } }
        public SwordTrail[] SwordTrails { get { return new SwordTrail[] {}; } }
        public Action<Projectile, Trail[], Vector2[], float[]> DrawTrails { get { return null; } }
    }


    public class SwordProjectile : ModProjectile
    {
        public override string Texture => "Divergency/Common/Helpers/SwordAnimator";

        float lerp(float a, float b, float f)
        {
            return a * (1.0f - f) + (b * f);
        }

        public int baseItem; // getting synced (i hope...)
        public float AttackSpeed = 0; // getting synced (i hope...)
        public float FramesPassed = 0; // getting synced (i hope...) (maby dosent need to?, probably does... (if so, it needs to be every frame...)

        public int NPCOwned = -1; // getting synced (i hope...)
        
        public float Charge = 0; // also needs net sync...

        // pass values from update into draw, i assume update runs on all clients
        //private float Rotation;
        //private Vector2 Position;
        private Vector2 Scale;

        public Trail[] Trails; // probably dosent need to be synced, needs to sync creation though...
        public Vector2[] trailPositions;
        public float[] trailRotations;

        //private bool Flipped;

        // private int freeze ?

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = false;
            Projectile.ownerHitCheck = true;
        }

        public override void AI()
        {
            ISwordSwing SwingInfo = ModContent.GetModItem(baseItem) as ISwordSwing;
            
            Projectile.timeLeft = 2;

            float curTime = FramesPassed * AttackSpeed;

            Keyframes.FrameInfo frameInfo = SwingInfo.SwordFrames.NextFrame(curTime);
            int keyframe = frameInfo.frameID;

            if (keyframe == -1)
            {
                Projectile.timeLeft = 0; // kill
                return;
            }

            int direction = Projectile.velocity.X > 0 ? 1 : -1;

            SwordAnimation SA = SwingInfo.SwordFrames.keyframeArray[keyframe];
            SwordAnimation LastSA = SwingInfo.SwordFrames.keyframeArray[Math.Max(keyframe - 1, 0)];

            float lastTime = curTime - AttackSpeed / SwingInfo.Updates;

            bool Charging = false;
            if (NPCOwned == -1)
            {
                int lastTimeKeyframe = SwingInfo.SwordFrames.NextFrame(lastTime).frameID;

                // TODO: add something to check all frames in between as well...
                if (lastTimeKeyframe != keyframe)
                { // changed anim
                    if (SA.HoldToContinue)
                    {
                        if (!Main.mouseLeft)
                        {
                            Projectile.timeLeft = 0; // kill
                            return;
                        }
                    }

                    if (SA.MaxCharge != 0) // can charge
                    {
                        if (Main.mouseLeft && !(SA.ChargeAutoRelease && (Charge == SA.MaxCharge)))
                        {
                            if (Charge == 0) {
                                SA.ChargeStart(Projectile);
                            }

                            Charge = MathF.Min(Charge + 1f / SwingInfo.Updates, SA.MaxCharge);
                            Charging = true;
                        }
                        else
                        {
                            SA.ChargeEnd(Projectile, Charge, SA.MaxCharge);
                        }
                    }
                }
            }

            if (!Charging)
            {
                FramesPassed += 1f / SwingInfo.Updates;
            }

            foreach (TimedFunction TF in SwingInfo.SwingFunctions)
            {
                TF.TryRun(Projectile, curTime, lastTime, SwingInfo.SwordFrames.TotalTime);
            }

            float CurFrame = frameInfo.framePassed;
            float MaxFrame = SA.Duration;

            bool inAnimation = CurFrame >= SA.SDelay && CurFrame <= SA.Duration;

            CurFrame = MathF.Min(MathF.Max(CurFrame - SA.SDelay, 0), SA.Duration);

            float LastFrame = CurFrame - AttackSpeed / SwingInfo.Updates;

            if (inAnimation)
            {
                foreach (TimedFunction TF in SA.FrameFunctions)
                {
                    TF.TryRun(Projectile, CurFrame, LastFrame, MaxFrame);
                }
            }

            //Console.WriteLine(CurFrame + " | " + MaxFrame);

            float Rotation = lerp(LastSA.TargetRotation, SA.TargetRotation, SA.RotationIn(CurFrame, MaxFrame)) * direction;
            Rotation *= SA.RotationMul(CurFrame, MaxFrame);

            Scale = Vector2.Lerp(LastSA.Scale, SA.Scale, SA.ScaleIn(CurFrame, MaxFrame));
            Scale *= SA.ScaleMul(CurFrame, MaxFrame);

            Vector2 localOffset = Vector2.Lerp(LastSA.LocalOffset, SA.LocalOffset, SA.LocalOffsetIn(CurFrame, MaxFrame));
            localOffset *= SA.LocalOffsetMul(CurFrame, MaxFrame);

            Vector2 globalOffset = Vector2.Lerp(LastSA.GlobalOffset, SA.GlobalOffset, SA.GlobalOffsetIn(CurFrame, MaxFrame));
            globalOffset *= SA.GlobalOffsetMul(CurFrame, MaxFrame);

            Vector2 pivot = (SwingInfo.Pivot + localOffset);
            pivot.X *= Scale.X;
            pivot.Y *= Scale.Y;

            Projectile.rotation = Rotation;

            Projectile.spriteDirection = SA.Flipped ? 1 : -1;

            Vector2 Position;
            if (NPCOwned != -1)
            {
                NPC npc = Main.npc[NPCOwned];

                Position = npc.Center - pivot.RotatedBy(Rotation) + globalOffset;

                npc.direction = direction;
            }
            else
            {
                Player player = Main.player[Projectile.owner];

                Position = player.Center - pivot.RotatedBy(Rotation) + globalOffset;

                player.heldProj = Projectile.whoAmI;

                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Rotation - direction * MathF.PI);
                player.ChangeDir(direction);
            }

            Projectile.Center = Position;

            int len = trailPositions.Length;
            trailPositions = trailPositions.Prepend(Position).Take(len).ToArray(); // heres where i might add more things for stuff...
            trailRotations = trailRotations.Prepend(Rotation+MathF.PI/2f).Take(len).ToArray();
        }

        public override bool PreKill(int timeLeft)
        {
            return base.PreKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int direction = Projectile.velocity.X > 0 ? 1 : -1;
            bool flipped = Projectile.spriteDirection == 1 ? true : false;

            ISwordSwing SwingInfo = ModContent.GetModItem(baseItem) as ISwordSwing;

            if (SwingInfo.DrawTrails != null)
            {
                SwingInfo.DrawTrails(Projectile, Trails, trailPositions, trailRotations);
            }
            else
            {
                for (int trailIDX = 0; trailIDX < SwingInfo.SwordTrails.Length; trailIDX++)
                {
                    SwordTrail ST = SwingInfo.SwordTrails[trailIDX];
                    Trails[trailIDX].Draw(trailPositions.Take(ST.TrailLength).ToArray(), trailRotations.Take(ST.TrailLength).ToArray());
                }
            }

            Texture2D texture = ModContent.Request<Texture2D>(SwingInfo.SwordTexture).Value;

            SpriteEffects spriteEffects = direction * (flipped ? -1 : 1) == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            float rotation = (Projectile.rotation - (direction == 1 ? (MathF.PI / 4) : (MathF.PI / 4*3)));

            rotation += (flipped ? (direction == 1 ? MathF.PI * 1.5f : MathF.PI/2f) : 0);

            if (texture != null)
            {
                if (SwingInfo.PreDraw != null)
                    if (!SwingInfo.PreDraw(Projectile)) // values to pass in
                        return false;

                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), lightColor, rotation, new Vector2(texture.Width / 2, texture.Height / 2), Scale, spriteEffects, 1f);
            }

            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            ISwordSwing SwingInfo = ModContent.GetModItem(baseItem) as ISwordSwing;

            if (SwingInfo.OnHitTile != null)
                return SwingInfo.OnHitTile(Projectile, oldVelocity);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
       
            ISwordSwing SwingInfo = ModContent.GetModItem(baseItem) as ISwordSwing;

            if (SwingInfo.OnHitNPC != null)
                SwingInfo.OnHitNPC(Projectile, target, hit.Damage, hit.Knockback, hit.Crit);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            ISwordSwing SwingInfo = ModContent.GetModItem(baseItem) as ISwordSwing;
            Texture2D texture = ModContent.Request<Texture2D>(SwingInfo.SwordTexture).Value;
            float collisionPoint = 0f;

            if (texture != null)
            {
                float halfHeight = MathF.Sqrt(texture.Height * texture.Height * 2) / 2;
                if (SwingInfo.Height != -1)
                    halfHeight = SwingInfo.Height;

                float SWidth = SwingInfo.Width * Scale.X;
                float SHegith = halfHeight * Scale.Y;

                float rotation = (Projectile.rotation - MathF.PI/2);

                if (Collision.CheckAABBvLineCollision(
                    targetHitbox.TopLeft(),
                    targetHitbox.Size(),
                    Projectile.Center - rotation.ToRotationVector2() * SHegith,
                    Projectile.Center + rotation.ToRotationVector2() * SHegith,
                    SWidth,
                    ref collisionPoint))
                { return true; }
            }

            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        { // i dont think this works...
            /*
            writer.Write(NPCOwned);
            writer.Write(baseItem);
            writer.Write(AttackSpeed);
            writer.Write(FramesPassed);
            */

            /*
            Console.WriteLine(Main.netMode + " -> asd");

            Console.WriteLine(IsNPC);
            Console.WriteLine(baseItem);
            Console.WriteLine(AttackSpeed);
            Console.WriteLine(FramesPassed);
            */
    }

    public override void ReceiveExtraAI(BinaryReader reader)
        {
            /*
            NPCOwned = reader.ReadInt32();
            baseItem = reader.ReadInt32();
            AttackSpeed = reader.ReadSingle();
            FramesPassed = reader.ReadSingle();
            */

            /*
            Console.WriteLine(Main.netMode + " -> tst");

            Console.WriteLine(IsNPC);
            Console.WriteLine(baseItem);
            Console.WriteLine(AttackSpeed);
            Console.WriteLine(FramesPassed);
            */
        }
    }
}