using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using Divergency.Content.Events.LivingCore;
using System;
using Divergency.Common.Helpers;
using System.Diagnostics;
using Terraria.DataStructures;
using ReLogic.Content;
using ParticleLibrary;
using Terraria.Graphics.Renderers;
using System.Reflection.Metadata;

namespace Divergency.Common.Systems3D
{
    internal static class Handler3D
    {
        public static Matrix View = Matrix.CreateTranslation(0, 0, -600);
        public static Effect Shader3D;

        private static Matrix projection;
        private static Matrix model;

        public static void Activate()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, Shader3D, Main.GameViewMatrix.TransformationMatrix);
        }

        public static void DeActivate()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin();
        }
        public static Matrix Rotate(float angle, Vector3 axis)
        {
            Matrix res = Matrix.Identity;
            float cA = MathF.Cos(angle);
            float sA = MathF.Sin(angle);
            float C = 1.0f - cA;

            res.M11 = cA + axis.X * axis.X * C;
            res.M12 = axis.X * axis.Y * C - axis.Z * sA;
            res.M13 = axis.X * axis.Z * C + axis.Y * sA;
            res.M21 = axis.Y * axis.X * C + axis.Z * sA;
            res.M22 = cA + axis.Y * axis.Y * C;
            res.M23 = axis.Y * axis.Z * C - axis.X * sA;
            res.M31 = axis.Z * axis.X * C - axis.Y * sA;
            res.M32 = axis.Z * axis.Y * C + axis.X * sA;
            res.M33 = cA + axis.Z * axis.Z * C;

            return res;
        }
        public static Vector3 MultVec3(Matrix m, Vector3 v, float last_bit)
        {
            Vector3 result;

            result.X = m.M11 * v.X +
                       m.M21 * v.Y +
                       m.M31 * v.Z +
                       m.M41 * last_bit;

            result.Y = m.M12 * v.X +
                       m.M22 * v.Y +
                       m.M32 * v.Z +
                       m.M42 * last_bit;

            result.Z = m.M13 * v.X +
                       m.M23 * v.Y +
                       m.M33 * v.Z +
                       m.M43 * last_bit;

            float w = m.M14 * v.X +
                      m.M24 * v.Y +
                      m.M34 * v.Z +
                      m.M44 * last_bit;

            if (last_bit != 0)
            {
                result /= w;
            }

            return result;
        }

        public static void SetValues(Player player, Vector2 OriginOffset, Vector2 Offset, Vector3 Rotations)
        {
            projection = Matrix.CreatePerspectiveFieldOfView(80 * MathF.PI / 180, (float)Main.screenWidth / Main.screenHeight, 0.1f, 1000f);

            model = Matrix.CreateScale(1);

            model *= Matrix.CreateRotationZ(-MathF.PI / 4);

            model *= Matrix.CreateScale(-1, -1, 1);

            model *= Matrix.CreateTranslation(OriginOffset.X, OriginOffset.Y, 0);

            model *= Matrix.CreateTranslation(Offset.X, Offset.Y, 0);

            model *= Matrix.CreateRotationY(Rotations.Y);
            model *= Matrix.CreateRotationZ(Rotations.X);
            //model *= Matrix.CreateRotationY(Rotations.Z);

            Vector3 p1 = MultVec3(model * View, new Vector3(0, 0, 0), 1);
            Vector3 p2 = MultVec3(model * View, new Vector3(1, 0, 0), 1);
            Vector3 p3 = MultVec3(model * View, new Vector3(0, 1, 0), 1);

            Vector3 norm = Vector3.Normalize(Vector3.Cross(p2 - p1, p3 - p1));

            model *= Rotate(-Rotations.Z, norm);

            model *= Matrix.CreateScale(-player.direction, 1, 1);

            Vector2 target = player.Center - Main.screenPosition - Main.ScreenSize.ToVector2() / 2 - player.velocity;

            model *= Matrix.CreateTranslation(target.X, target.Y, 0);

            // idk if this needs to be called every frame time you change stuff...
            /*
            Shader3D.Parameters["Model"].SetValue(model);
            Shader3D.Parameters["View"].SetValue(View);
            Shader3D.Parameters["Projection"].SetValue(projection);
            */

            Shader3D.Parameters["Model"].SetValue(model);
            Shader3D.Parameters["View"].SetValue(View);
            Shader3D.Parameters["Projection"].SetValue(projection);
        }

        public static int Swing(Player player, EntitySource_ItemUse_WithAmmo source, int itemID, int damage, float knockback)
        {
            int projectileID = Projectile.NewProjectile(source, player.position, new Vector2(0, 0), ModContent.ProjectileType<Sword3DProjectile>(), damage, knockback, player.whoAmI);
            Projectile projectile = Main.projectile[projectileID];

            projectile.timeLeft = (ModContent.GetModItem(itemID) as I3D).SwordFrames.lookupArray.Count;

            Sword3DProjectile proj3D = projectile.ModProjectile as Sword3DProjectile;
            proj3D.baseItem = itemID;

            player.heldProj = projectileID;

            return projectileID;
        }

        public static void Setup()
        {
            Shader3D = ModContent.Request<Effect>("Divergency/Common/Systems3D/S3D", AssetRequestMode.ImmediateLoad).Value;

            Shader3D.Parameters["Model"].SetValue(model);
            Shader3D.Parameters["View"].SetValue(View);
            Shader3D.Parameters["Projection"].SetValue(projection);
        }

        /*
        public Matrix projection;
        public Matrix model;

        public Vector3 scale3D = new Vector3();
        public Vector3 rotation3D = new Vector3();
        */
    }
    public class Setup3D : ModSystem
    {
        public override void OnModLoad()
        {
            Handler3D.Setup();
        }
    }
}
