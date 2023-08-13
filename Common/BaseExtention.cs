using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;


namespace Divergency.Common
{
	
	public static class BaseExtension
    {

        public static float InRadians(this float degrees) => MathHelper.ToRadians(degrees);

        /// <summary>Shorthand for converting radians of rotation into a degrees equivalent.</summary>
        public static float InDegrees(this float radians) => MathHelper.ToDegrees(radians);

        public static Rectangle AnimationFrame(this Texture2D texture, ref int frame, ref int frameTick, int frameTime, int frameCount, bool frameTickIncrease, int overrideHeight = 0)
        {
            if (frameTick >= frameTime)
            {
                frameTick = -1;
                frame = frame == frameCount - 1 ? 0 : frame + 1;
            }
            if (frameTickIncrease)
                frameTick++;
            return new Rectangle(0, overrideHeight != 0 ? overrideHeight * frame : (texture.Height / frameCount) * frame, texture.Width, texture.Height / frameCount);
        }
        // use:
        // public Rectangle FrameData;
        // public int FrameTick;
        //
        // Texture tex;
        // int frameTime = 6; // how long each frame lasts
        // tex.Animate(ref FrameData, ref FrameTick, frameTime);
        public static Rectangle Animate(this Texture2D texture, ref Rectangle frameData, ref int frameTick, int frameTime, int horizontalFrames = 1, int verticalFrames = 1, int sizeOffsetX = 0, int sizeOffsetY = 0)
        {
            frameData.Width = horizontalFrames;
            frameData.Height = verticalFrames;

            if (frameTick >= frameTime)
            {
                frameTick = 0;
                frameData.X = frameData.X == frameData.Width - 1 ? 0 : frameData.X + 1;
                frameData.Y = frameData.Y == frameData.Height - 1 ? 0 : frameData.Y + 1;
            }
            frameTick++;

            return new Rectangle(sizeOffsetX != 0 ? sizeOffsetX * frameData.X : (texture.Width / frameData.Width) * frameData.X,   // Horizontal frame position
                                 sizeOffsetY != 0 ? sizeOffsetY * frameData.Y : (texture.Height / frameData.Height) * frameData.Y, // Vertical frame position
                                 texture.Width / frameData.Width,                   // Horizontal frame width
                                 texture.Height / frameData.Height);                // Vertical frame width
        }


        public static float Magnitude(Vector2 mag)
        {
            return (float)Math.Sqrt(mag.X * mag.X + mag.Y * mag.Y);
        }
        public static void Move(this NPC npc, Vector2 pos, float speed, float divider)
    {
        Vector2 vel = npc.DirectionTo(pos) * speed;
        npc.velocity = (npc.velocity * divider + vel) / (divider + 1);
    }

    public static void Move(this NPC npc, Vector2 vector, float speed, float turnResistance = 10f,
        bool toPlayer = false)
    {
        Player player = Main.player[npc.target];
        Vector2 moveTo = toPlayer ? player.Center + vector : vector;
        Vector2 move = moveTo - npc.Center;
        float magnitude = Magnitude(move);
        if (magnitude > speed)
        {
            move *= speed / magnitude;
        }

        move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
        magnitude = Magnitude(move);
        if (magnitude > speed)
        {
            move *= speed / magnitude;
        }

        npc.velocity = move;
    }

        public static void MoveAbove(this NPC npc, Vector2 vector, float speed, float turnResistance = 10f,
        bool toPlayer = false)
        {
            Player player = Main.player[npc.target];
            Vector2 moveTo = player.Top + new Vector2(0, -650);
            Vector2 move = moveTo - (npc.Bottom - new Vector2(0, 500));
            float magnitude = Magnitude(move);
            if (magnitude > speed)
            {
                move *= speed / magnitude;
            }

            move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
            magnitude = Magnitude(move);
            if (magnitude > speed)
            {
                move *= speed / magnitude;
            }

            npc.velocity = move;
        }

        public static void Move(this Projectile projectile, Vector2 vector, float speed, float turnResistance = 10f, bool toPlayer = false)
    {
        Player player = Main.player[projectile.owner];
        Vector2 moveTo = toPlayer ? player.Center + vector : vector;
        Vector2 move = moveTo - projectile.Center;
        float magnitude = Magnitude(move);
        if (magnitude > speed)
        {
            move *= speed / magnitude;
        }

        move = (projectile.velocity * turnResistance + move) / (turnResistance + 1f);
        magnitude = Magnitude(move);
        if (magnitude > speed)
        {
            move *= speed / magnitude;
        }

        projectile.velocity = move;
    }


            
        public static int AttackTimer;
        public static bool mouseleftwasreleased;
        public static float pos { get; set; }
        public static byte CurrentAttack { get; set; }
   
          

            


    }

    
	
}
