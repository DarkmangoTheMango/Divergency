using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Divergency.Content.Tiles.LivingGrove.CorePuzzle;
using Terraria.ID;
using static System.Net.Mime.MediaTypeNames;

namespace Divergency.Assets.Particles
{
    public class ResetParticle : Particle
    {
        public override string Texture => "Divergency/Assets/Textures/ParticleTextures/Shockwave";

        public override void SetDefaults()
        {
            width = 1;
            height = 1;
            timeLeft = 40;
        }

        public override void AI()
        {



            rotation = velocity.ToRotation();

            // Allows custom scaling upon spawn
            Scale *= ai[0] != 0f ? ai[0] : 1.3f;
            if (Scale <= 0f)
                active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPos, Color lightColor)
        {
            Texture2D tex3 = ModContent.Request<Texture2D>(Texture).Value;

            float alpha = timeLeft <= 20 ? 1f - 1f / 20f * (20 - timeLeft) : 1f;
            if (alpha < 0f) alpha = 0f;
            Color color = Color.Multiply(new(0.50f, 2.05f, 0.5f, 0), alpha);
            Color color2 = Color.Multiply(new(0.50f, 2.05f, 0.5f, 0), alpha);
            spriteBatch.Draw(tex3, position - Main.screenPosition, new Rectangle(0, 0, tex3.Width, tex3.Height), color, rotation, new Vector2(tex3.Width / 2f, tex3.Height / 2f), 0.17f * Scale, SpriteEffects.None, 0f);
            return false;
        }
    }
    public class LivingCoreInsertParticle : Particle
    {
        private int frameCount;
        private int frameTick;
        public override string Texture => "Divergency/Assets/Textures/ParticleTextures/LivingCore";
        public override void SetDefaults()
        {
            width = 34;
            height = 34;
            Scale = 3f;
            timeLeft = 99999999;
        }
        public override void AI()
        {
            Player player = Main.LocalPlayer;
            {
                Vector2 pos = player.Center + new Vector2(0, -30);
                position = pos;
                if (player.GetModPlayer<CorePuzzle>().LivingCoreAmount == 0)
                {
                    active = false;
                }




            }
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;


            float alpha = timeLeft <= 20 ? 1f - 1f / 20f * (20 - timeLeft) : 1f;
            if (alpha < 0f) alpha = 0f;
            Color color = Color.Multiply(new(0.50f, 2.05f, 0.5f, 0), alpha);
            //spriteBatch.Draw(tex2, position - Main.screenPosition, new Rectangle(0, 0, tex2.Width, tex2.Height), color, ai[1].InRadians().AngleLerp((ai[1] * 180f).InRadians(), (120f - timeLeft) / 120f), new Vector2(tex2.Width / 2f, tex2.Height / 2f), 0.05f * Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, position - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), color, 0, new Vector2(texture.Width / 2f, texture.Height / 2f), 0.8f * Scale, SpriteEffects.None, 0f);
            //spriteBatch.Draw(tex, position - Main.screenPosition, tex.AnimationFrame(ref frameCount, ref frameTick, 4, 7, true), color, 0f, new Vector2(width / 2, height / 2), 0.5f, SpriteEffects.None, 0f);
            return false;
        }
    }
    public class LivingCoreInsertParticle2 : Particle
    {
        private int frameCount;
        private int frameTick;
        public override string Texture => "Divergency/Assets/Textures/ParticleTextures/LivingCore";

        public override void SetDefaults()
        {
            width = 34;
            height = 34;
            Scale = 3f;
            timeLeft = 99999999;
        }

        public override void AI()
        {
            Player player = Main.LocalPlayer;


            Vector2 pos = player.Center + new Vector2(30, -30);
            position = pos;

            if (player.GetModPlayer<CorePuzzle>().LivingCoreAmount < 2)
            {
                active = false;
            }




        }


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;


            float alpha = timeLeft <= 20 ? 1f - 1f / 20f * (20 - timeLeft) : 1f;
            if (alpha < 0f) alpha = 0f;
            Color color = Color.Multiply(new(0.50f, 2.05f, 0.5f, 0), alpha);
            //spriteBatch.Draw(tex2, position - Main.screenPosition, new Rectangle(0, 0, tex2.Width, tex2.Height), color, ai[1].InRadians().AngleLerp((ai[1] * 180f).InRadians(), (120f - timeLeft) / 120f), new Vector2(tex2.Width / 2f, tex2.Height / 2f), 0.05f * Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, position - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), color, 0, new Vector2(texture.Width / 2f, texture.Height / 2f), 0.8f * Scale, SpriteEffects.None, 0f);
            //spriteBatch.Draw(tex, position - Main.screenPosition, tex.AnimationFrame(ref frameCount, ref frameTick, 4, 7, true), color, 0f, new Vector2(width / 2, height / 2), 0.5f, SpriteEffects.None, 0f);
            return false;
        }
    }
    public class LivingCoreInsertParticle3 : Particle
    {
        private int frameCount;
        private int frameTick;
        public override string Texture => "Divergency/Assets/Textures/ParticleTextures/LivingCore";

        public override void SetDefaults()
        {
            width = 34;
            height = 34;
            Scale = 3f;
            timeLeft = 99999999;
        }

        public override void AI()
        {
            Player player = Main.LocalPlayer;



            Vector2 pos = player.Center + new Vector2(-30, -30);
            position = pos;


            if (player.GetModPlayer<CorePuzzle>().LivingCoreAmount < 3)
            {
                active = false;
            }




        }


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;


            float alpha = timeLeft <= 20 ? 1f - 1f / 20f * (20 - timeLeft) : 1f;
            if (alpha < 0f) alpha = 0f;
            Color color = Color.Multiply(new(0.50f, 2.05f, 0.5f, 0), alpha);
            //spriteBatch.Draw(tex2, position - Main.screenPosition, new Rectangle(0, 0, tex2.Width, tex2.Height), color, ai[1].InRadians().AngleLerp((ai[1] * 180f).InRadians(), (120f - timeLeft) / 120f), new Vector2(tex2.Width / 2f, tex2.Height / 2f), 0.05f * Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, position - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), color, 0, new Vector2(texture.Width / 2f, texture.Height / 2f), 0.8f * Scale, SpriteEffects.None, 0f);
            //spriteBatch.Draw(tex, position - Main.screenPosition, tex.AnimationFrame(ref frameCount, ref frameTick, 4, 7, true), color, 0f, new Vector2(width / 2, height / 2), 0.5f, SpriteEffects.None, 0f);
            return false;
        }
    }
}
