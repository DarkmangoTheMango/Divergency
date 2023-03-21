using Divergency.Common.Helpers;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Divergency.Common.Systems3D
{
    public class Sword3DProjectile : ModProjectile
    {
        public int baseItem;

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.ownerHitCheck = true;
        }

        public override void AI()
        {
        }

        public override void Kill(int timeLeft)
        {
            base.Kill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            I3D ItemInfo = ModContent.GetModItem(baseItem) as I3D;

            int frame = ItemInfo.SwordFrames.lookupArray.Count - Projectile.timeLeft;
            int keyframe = ItemInfo.SwordFrames.lookupArray[frame];

            SwordAnimation SA = ItemInfo.SwordFrames.realArray[keyframe];

            Vector3 StartRotation = keyframe == 0 ? ItemInfo.StartRotation : ItemInfo.SwordFrames.realArray[keyframe - 1].TargetRotation;
            Vector3 EndRotation = SA.TargetRotation;

            Vector2 StartOffset = keyframe == 0 ? ItemInfo.StartOffset : ItemInfo.SwordFrames.realArray[keyframe - 1].Offset;
            Vector2 EndOffset = SA.Offset;

            Player player = Main.player[Projectile.owner];

            Handler3D.Activate();

            Vector3 rotation = SA.Interpolation3D.Lerp(StartRotation, EndRotation, frame - SA.LastKeyframe, SA.ThisKeyframe - SA.LastKeyframe);
            Vector2 offset = SA.Interpolation2D.Lerp(StartOffset, EndOffset, frame - SA.LastKeyframe, SA.ThisKeyframe - SA.LastKeyframe);

            Console.WriteLine(frame + ": " + StartRotation + " | " + EndRotation + " | " + (frame - SA.LastKeyframe) + " | " + (SA.ThisKeyframe - SA.LastKeyframe));

            //rotation.X *= -player.direction;

            Handler3D.SetValues(player, ItemInfo.ConstOffset, offset, rotation);

            Texture2D texture = ModContent.Request<Texture2D>(ItemInfo.SwordTexture).Value;
            if (texture != null)
            {
                Main.spriteBatch.Draw(texture, new Vector2(0, 0), new Rectangle(0, 0, texture.Width, texture.Height), Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), 1, SpriteEffects.None, 1f);
            }

            Handler3D.DeActivate();

            return false;
        }
    }
}
