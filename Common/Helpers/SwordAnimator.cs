using Humanizer;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using ReLogic.Content;
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
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;

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
        public static int Swing<T>(Player player, int damage, float knockback) where T : SwordSwing, new()
        {
            SwordSwing SS = new T();

            int projectileID = Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, new Vector2(player.direction, 0), ModContent.ProjectileType<SwordProjectile>(), damage, knockback, player.whoAmI);
            Projectile projectile = Main.projectile[projectileID];

            projectile.timeLeft = 2;

            projectile.extraUpdates = SS.Updates - 1;
            projectile.localNPCHitCooldown = SS.NPCHitCooldown * SS.Updates; // maby change..?

            SwordProjectile projSword = projectile.ModProjectile as SwordProjectile;
            projSword.SwingInfo = SS;
            projSword.AttackSpeed = player.GetTotalAttackSpeed(DamageClass.Melee); // idk how speed is calculated...
            // above might just work a-ok

            projectile.netUpdate = true;

            return projectileID;
        }

        public static int Swing<T>(NPC npc, int damage, float knockback, float attackspeed = 1) where T : SwordSwing, new()
        {
            SwordSwing SS = new T();

            int projectileID = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(npc.direction, 0), ModContent.ProjectileType<SwordProjectile>(), damage, knockback);
            Projectile projectile = Main.projectile[projectileID];

            projectile.timeLeft = 2;

            projectile.extraUpdates = SS.Updates - 1;
            projectile.localNPCHitCooldown = SS.NPCHitCooldown * SS.Updates; // maby change..?
            projectile.hostile = true;
            projectile.friendly = false;

            SwordProjectile projSword = projectile.ModProjectile as SwordProjectile;
            projSword.SwingInfo = SS;
            projSword.AttackSpeed = attackspeed;
            projSword.NPCOwned = npc.whoAmI;

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

    public enum TrailType
    {
        Raw,
        Sqrt,
    }
    
    public class SwordTrail
    {

        public Texture2D Texture; // 1*x where x is sword height

        public float trailMultiplier;
        public float trailLimit;

        public float height;

        public TrailType TT;

        public static Effect effect = ModContent.Request<Effect>("Divergency/Content/Effects/SwordTrailShader/Renderer", AssetRequestMode.ImmediateLoad).Value;

        public SwordTrail(string Texture, float height, TrailType trailType = TrailType.Raw, float trailMultiplier = 80, float trailLimit = 1)
        {
            this.Texture = ModContent.Request<Texture2D>(Texture).Value;

            this.trailMultiplier = trailMultiplier;
            this.trailLimit = trailLimit;

            this.TT = trailType;

            this.height = height;
        }

        public void DrawSwordTrail(Vector2 worldPos, float scale, int dir, float rotation, float lastRot) // float speed and max speed to scale TrailLength width
        {
            int _height = (int)(height * scale);
            float len = MathF.Min(((rotation - lastRot) * trailMultiplier) * dir, trailLimit);

            effect.Parameters.GetParameterBySemantic("rot").SetValue(rotation - MathF.PI/ 2f);
            effect.Parameters.GetParameterBySemantic("len").SetValue(len);
            effect.Parameters.GetParameterBySemantic("mlen").SetValue(trailLimit);
            effect.Parameters.GetParameterBySemantic("dir").SetValue(dir);

            effect.Parameters.GetParameterBySemantic("type").SetValue((int)TT+1);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, effect, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(Texture, new Rectangle((int)worldPos.X - _height, (int)worldPos.Y - _height, _height * 2, _height * 2), Color.White);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
    public class SwordGlowColor // Color animation
    {
        public bool loop;
        public List<Color> colors;
        public List<int> colorTimers;
        public float loopTime;

        public SwordGlowColor(List<Color> colors, List<int> colorTimers)
        {
            this.colors = colors;
            this.colorTimers = colorTimers;
            this.loop = false;
            this.loopTime = 0f;
        }
        public SwordGlowColor(List<Color> colors, float loopTime)
        {
            this.colors = colors;
            this.loopTime = loopTime;
            this.loop = true;
            this.colorTimers = null;
        }

        public Color GetColor(float time)
        {
            if (colors.Count == 1)
                return colors[1];
            else if (colors.Count == 0)
                return Color.White;

            if (loop)
            {
                float colorCalculation = (time / loopTime) % colors.Count;
                int color = (int)Math.Ceiling(colorCalculation) - 1;
                float transition = colorCalculation % 1f;

                int otherColor = (color - 1) % colors.Count;
                if (otherColor < 0) otherColor += colors.Count;

                Console.WriteLine(colors[otherColor] + ", " + colors[color] + ", " + transition + " => " + Color.Lerp(colors[otherColor], colors[color], transition));

                return Color.Lerp(colors[otherColor], colors[color], transition);
            }
            else
            {
                int curTime = 0;

                for (int i = 0; i < colorTimers.Count; i++)
                {
                    if (curTime + colorTimers[i] > time)
                    {
                        float progress = time - curTime;
                        float percentProgress = progress / colorTimers[i];

                        int otherColor = i - 1;
                        if (otherColor == -1) otherColor = 0;

                        return Color.Lerp(colors[otherColor], colors[i], percentProgress);
                    }

                    curTime = curTime + colorTimers[i];
                }

                return colors[colors.Count-1];
            }
        }
    }

    public class SwordGlow
    {
        public string Path;
        public SwordGlowColor Color;
        public float Scale;
        public bool InFront;

        public SwordGlow(SwordGlowColor color, float scale = 1f, bool inFront = true, string path = "")
        {
            Path = path;
            Color = color;
            Scale = scale;
            InFront = inFront;
        }

        //texture, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), lightColor, rotation, new Vector2(texture.Width / 2, texture.Height / 2), Scale, spriteEffects, 1f)
        public void Draw(string tex, Vector2 position, Rectangle rec, float rotation, Vector2 offset, Vector2 scale, SpriteEffects spriteEffects, float time)
        {
            string path = Path;
            if (path == "") { path = tex + "Glow"; }

            Texture2D texture = ModContent.Request<Texture2D>(path).Value;

            Console.WriteLine(Color.GetColor(time));

            Main.spriteBatch.Draw(texture, position, rec, Color.GetColor(time), rotation, offset, Scale*scale, spriteEffects, 1f);
        }
    }

    public abstract class SwordSwing
    {
        public abstract float Width { get; }
        public abstract string SwordTexture { get; }
        public abstract Keyframes SwordFrames { get; }
        public virtual float Height { get { return -1; } } // height based off of texture
        public virtual Vector2 Pivot { get { return default; } }
        public abstract float BuildInRotation { get; }
        public virtual Func<Projectile, bool> PreDraw { get { return null; } }
        public virtual Func<Projectile, Vector2, bool> OnHitTile { get { return null; } }
        public virtual Action<Projectile, NPC, int, float, bool> OnHitNPC { get { return null; } }
        public virtual TimedFunction[] SwingFunctions { get { return new TimedFunction[] { }; } }
        public virtual int Updates { get { return 1; } }
        public virtual int NPCHitCooldown { get { return 10; } }
        public virtual SwordTrail SwordTrail { get { return null; } }
        public virtual Action<Projectile, Trail[], Vector2[], float[]> DrawTrails { get { return null; } }
        public virtual SwordGlow[] Glows { get { return new SwordGlow[] { }; } }
        public virtual bool SlantingSword { get { return true; } }
    }


    public class SwordProjectile : ModProjectile
    {
        public override string Texture => "Divergency/Common/Helpers/SwordAnimator";

        float lerp(float a, float b, float f)
        {
            return a * (1.0f - f) + (b * f);
        }

        public SwordSwing SwingInfo; // getting synced (i hope...)
        public float AttackSpeed = 0; // getting synced (i hope...)
        public float FramesPassed = 0; // getting synced (i hope...) (maby dosent need to?, probably does... (if so, it needs to be every frame...)

        public int NPCOwned = -1; // getting synced (i hope...)
        
        public float Charge = 0; // also needs net sync...

        // pass values from update into draw, i assume update runs on all clients
        //private float Rotation;
        //private Vector2 Position;
        private Vector2 TrailPosition;
        private Vector2 Scale;

        //private bool Flipped;

        // private int freeze ?

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            base.SetStaticDefaults();
        }

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

                TrailPosition = npc.Center + localOffset.RotatedBy(Rotation) + globalOffset;
                Position = npc.Center - pivot.RotatedBy(Rotation) + globalOffset;

                npc.direction = direction;
            }
            else
            {
                Player player = Main.player[Projectile.owner];

                Console.WriteLine(pivot.RotatedBy(Rotation));
                TrailPosition = player.Center + localOffset.RotatedBy(Rotation) + globalOffset;
                Position = player.Center - pivot.RotatedBy(Rotation) + globalOffset;

                player.heldProj = Projectile.whoAmI;    

                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Rotation - direction * MathF.PI);
                player.ChangeDir(direction);
            }

            Projectile.Center = Position;
        }

        public override bool PreKill(int timeLeft)
        {
            return base.PreKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float curTime = FramesPassed * AttackSpeed;
            int direction = Projectile.velocity.X > 0 ? 1 : -1;
            bool flipped = Projectile.spriteDirection == 1 ? true : false;

            Keyframes.FrameInfo frameInfo = SwingInfo.SwordFrames.NextFrame(curTime);
            int keyframe = frameInfo.frameID;

            if (keyframe == -1)
                return false;

            SwordAnimation SA = SwingInfo.SwordFrames.keyframeArray[keyframe];
            SwordAnimation LastSA = SwingInfo.SwordFrames.keyframeArray[Math.Max(keyframe - 1, 0)];

            SwordTrail ST = SwingInfo.SwordTrail;

            List<SwordGlow> frontGlow = new List<SwordGlow>();
            List<SwordGlow> backGlow = new List<SwordGlow>();

            foreach (SwordGlow glow in SwingInfo.Glows)
            {
                if (glow.InFront)
                    frontGlow.Add(glow);
                else
                    backGlow.Add(glow);
            }

            Texture2D texture = ModContent.Request<Texture2D>(SwingInfo.SwordTexture).Value;

            Player player = Main.player[Projectile.owner];

            if (ST != null)
            {
                float h = texture.Height * Scale.Y;
                float w = texture.Width * Scale.X;

                ST.DrawSwordTrail(TrailPosition - Main.screenPosition, Scale.Y, (SA.TargetRotation > LastSA.TargetRotation ? 1 : -1) * direction, Projectile.oldRot[0], Projectile.oldRot[1]);
            }

            SpriteEffects spriteEffects;
            float rotation;
            
            if (SwingInfo.SlantingSword)
            {
                spriteEffects = direction * (flipped ? -1 : 1) == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

                rotation = (Projectile.rotation - (direction == 1 ? (MathF.PI / 4) : (MathF.PI / 4 * 3)));

                rotation += (flipped ? (direction == 1 ? MathF.PI * 1.5f : MathF.PI / 2f) : 0);

            }
            else
            {
                spriteEffects = SpriteEffects.None;

                rotation = Projectile.rotation;

            }

            if (texture != null)
            {
                if (SwingInfo.PreDraw != null)
                    if (!SwingInfo.PreDraw(Projectile)) // values to pass in
                        return false;
                float addRotation = SwingInfo.BuildInRotation *    -direction;

                foreach (SwordGlow glow in backGlow)
                {
                    glow.Draw(SwingInfo.SwordTexture, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), rotation - addRotation, new Vector2(texture.Width / 2, texture.Height / 2), Scale, spriteEffects, curTime);
                }

                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), lightColor, rotation - addRotation, new Vector2(texture.Width / 2, texture.Height / 2), Scale, spriteEffects, 1f);

                foreach (SwordGlow glow in frontGlow)
                {
                    glow.Draw(SwingInfo.SwordTexture, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), rotation - addRotation, new Vector2(texture.Width / 2, texture.Height / 2), Scale, spriteEffects, curTime);
                }
            }

            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (SwingInfo.OnHitTile != null)
                return SwingInfo.OnHitTile(Projectile, oldVelocity);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (SwingInfo.OnHitNPC != null)
                SwingInfo.OnHitNPC(Projectile, target, hit.Damage, hit.Knockback, hit.Crit);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
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