using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Divergency.Common.Helpers
{
    public class BeamPacket
    {
        public List<VertexPositionColorTexture> Vertices = new List<VertexPositionColorTexture>();
        public PrimitiveType Type = PrimitiveType.TriangleList;
        public Effect Effect = Divergency.BeamShader;
        public string Pass = "Basic";
        public int Count
        {
            get
            {
                int count = 0;
                switch (Type)
                {
                    case PrimitiveType.LineList:
                        count = Vertices.Count / 2;
                        break;
                    case PrimitiveType.LineStrip:
                        count = Vertices.Count - 1;
                        break;
                    case PrimitiveType.TriangleList:
                        count = Vertices.Count / 3;
                        break;
                    case PrimitiveType.TriangleStrip:
                        count = Vertices.Count - 2;
                        break;
                }
                return count;
            }
        }
        public void Add(Vector2 position, Color color, Vector2 TexCoord)
        {
            Vector2 pos = position - Main.screenPosition;
            Vector3 pos2 = new Vector3(pos.X, pos.Y, 0f);
            Vertices.Add(new VertexPositionColorTexture(pos2, color, TexCoord));
        }
        public void Add2(Vector2 pos, Color color, Vector2 TexCoord)
        {
            Vertices.Add(new VertexPositionColorTexture(new Vector3(pos.X, pos.Y, 0), color, TexCoord));
        }
        public void AddStrip(Vector2 pos1, Vector2 pos2, float size1, float size2, float progress1, float progress2, Color color1, Color color2)
        {
            Vector2 dir = (pos2 - pos1).SafeNormalize(Vector2.Zero).RotatedBy(Math.PI / 2);
            Vector2 offset1 = dir * size1;
            Vector2 offset2 = dir * size2;
            Add(pos1 + offset1, color1, new Vector2(progress1, 1));
            Add(pos1 - offset1, color1, new Vector2(progress1, 0));
            Add(pos2 + offset2, color2, new Vector2(progress2, 1));

            Add(pos2 - offset2, color2, new Vector2(progress2, 0));
            Add(pos2 + offset2, color2, new Vector2(progress2, 1));
            Add(pos1 - offset1, color1, new Vector2(progress1, 0));
        }
        public static void SetTexture(int index, Texture2D texture)
        {
            Main.graphics.GraphicsDevice.Textures[index] = texture;
            Main.graphics.GraphicsDevice.SamplerStates[index] = SamplerState.PointWrap;
        }

        public short[] GetIndices()
        {
            short[] indexes = new short[1];
            int count = Vertices.Count - 1;
            switch (Type)
            {
                case PrimitiveType.TriangleList:
                    int IPV = 3; // indexes per vertex
                    int length = count * IPV; // length of index array
                    if (indexes.Length < length)
                    {
                        Array.Resize(ref indexes, length);
                    }
                    for (short i = 0; i < count; i = (short)(i + 1))
                    {
                        short indexInArray = (short)(i * IPV);
                        int num = i * 2;
                        indexes[indexInArray] = (short)num; // resuming: connect first one
                        indexes[indexInArray + 1] = (short)(num + 1); // to second one
                        indexes[indexInArray + 2] = (short)(num + 2); // then to third one
                    }
                    break;
                case PrimitiveType.TriangleStrip:
                    int IPV1 = 2;
                    int length1 = count * IPV1;
                    if (indexes.Length < length1)
                    {
                        Array.Resize(ref indexes, length1);
                    }
                    for (short i = 0; i < count; i = (short)(i + 1))
                    {
                        short indexInArray = (short)(i * IPV1);
                        int num = i * 2;
                        indexes[indexInArray] = (short)num; // connect first one
                        indexes[indexInArray] = (short)(num + 1); // to second one
                    }
                    break;
                case PrimitiveType.LineList:
                    int IPV2 = 2;
                    int length2 = count * IPV2;
                    if (indexes.Length < length2)
                    {
                        Array.Resize(ref indexes, length2);
                    }
                    for (short i = 0; i < count; i = (short)(i + 1))
                    {
                        short indexInArray = (short)(i * IPV2);
                        int num = i * 2;
                        indexes[indexInArray] = (short)num; // connect first
                        indexes[indexInArray] = (short)(num + 1); // to second
                    }
                    break;
                case PrimitiveType.LineStrip:
                    int length3 = count;
                    for (short i = 0; i < count; i = (short)(i + 1))
                    {
                        short indexInArray = (short)(i);
                        int num = i * 2;
                        indexes[indexInArray] = (short)num;
                    }
                    break;
            }
            return indexes;
        }
        public void Send()
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            if (Count > 0)
            {
                VertexBuffer buffer = new VertexBuffer(device, typeof(VertexPositionColorTexture), Vertices.Count, BufferUsage.WriteOnly);
                IndexBuffer index = new IndexBuffer(device, typeof(short), GetIndices().Length, BufferUsage.WriteOnly);

                device.SetVertexBuffer(null);

                buffer.SetData(Vertices.ToArray());
                index.SetData(GetIndices());

                device.SetVertexBuffer(buffer);
                device.Indices = index;

                RasterizerState rasterizerState = new RasterizerState();
                rasterizerState.CullMode = CullMode.None;
                device.RasterizerState = rasterizerState;

                Effect.Parameters["WVP"].SetValue(BeamHelper.GetMatrix());
                Effect.CurrentTechnique.Passes[Pass].Apply();

                device.DrawPrimitives(Type, 0, Count);
            }
        }
    }
    public class MatrixCollection
    {
        public Matrix View;
        public Matrix Projection;
    }
    public static class BeamHelper
    {
        public static MatrixCollection GetMatrixes()
        {
            MatrixCollection matrixes = new MatrixCollection();
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            int width = device.Viewport.Width;
            int height = device.Viewport.Height;
            Vector2 zoom = Main.GameViewMatrix.Zoom;
            matrixes.View = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(width / 2, height / -2, 0) * Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(zoom.X, zoom.Y, 1f);
            matrixes.Projection = Matrix.CreateOrthographic(width, height, 0, 1000);
            return matrixes;
        }
        public static Matrix GetMatrix()
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            int width = device.Viewport.Width;
            int height = device.Viewport.Height;
            Vector2 zoom = Main.GameViewMatrix.Zoom;
            Matrix View = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(width / 2, height / -2, 0) * Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(zoom.X, zoom.Y, 1f);
            Matrix Projection = Matrix.CreateOrthographic(width, height, 0, 1000);
            return View * Projection;
        }
        public static Vector3 ToVector3(this Vector2 vec)
        {
            return new Vector3(vec.X, vec.Y, 0);
        }
        public static Vector2 GetRotation(IReadOnlyList<Vector2> oldPos, int index)
        {
            if (oldPos.Count == 1)
                return oldPos[0];

            if (index == 0)
            {
                return Vector2.Normalize(oldPos[1] - oldPos[0]).RotatedBy(MathHelper.Pi / 2);
            }

            return (index == oldPos.Count - 1
                ? Vector2.Normalize(oldPos[index] - oldPos[index - 1])
                : Vector2.Normalize(oldPos[index + 1] - oldPos[index - 1])).RotatedBy(MathHelper.Pi / 2);
        }
        public static void SetBasicEffectParameters(this Effect effect)
        {
            effect.Parameters["WVP"].SetValue(GetMatrix());
        }
    }
    public static class DivUtils
    {
        public static VertexPositionColorTexture AsVertex(Vector2 position, Color color, Vector2 texCoord)
        {
            return new VertexPositionColorTexture(new Vector3(position, 50), color, texCoord);
        }
        public static VertexPositionColorTexture AsVertex(Vector3 position, Color color, Vector2 texCoord)
        {
            return new VertexPositionColorTexture(position, color, texCoord);
        }
        private static int width;
        private static int height;
        private static Vector2 zoom;
        private static Matrix view;
        private static Matrix projection;
        private static bool CheckGraphicsChanged()
        {
            var device = Main.graphics.GraphicsDevice;
            bool changed = device.Viewport.Width != width
                           || device.Viewport.Height != height
                           || Main.GameViewMatrix.Zoom != zoom;

            if (!changed) return false;

            width = device.Viewport.Width;
            height = device.Viewport.Height;
            zoom = Main.GameViewMatrix.Zoom;

            return true;
        }

        public static void QuickDustLine(this Dust dust, Vector2 start, Vector2 end, float splits, Color color)
        {
            Dust.QuickDust(start, color).scale = 1f;
            Dust.QuickDust(end, color).scale = 1f;
            float num = 1f / splits;
            for (float amount = 0.0f; (double)amount < 1.0; amount += num)
                Dust.QuickDustSmall(Vector2.Lerp(start, end, amount), color).scale = 1f;
        }
        public static void QuickDustLine(this Dust dust, Vector2 start, Vector2 end, float splits, Color color1, Color color2)
        {
            Dust.QuickDust(start, color1).scale = 1f;
            Dust.QuickDust(end, color2).scale = 1f;
            float num = 1f / splits;
            for (float amount = 0.0f; (double)amount < 1.0; amount += num)
            {
                Color color = Color.Lerp(color1, color2, amount);
                Dust.QuickDustSmall(Vector2.Lerp(start, end, amount), color).scale = 1f;
            }
        }
        public static float CircleDividedEqually(float i, float max)
        {
            return 2f * (float)Math.PI / max * i;
        }
        public static Matrix GetMatrix()
        {
            if (CheckGraphicsChanged())
            {
                var device = Main.graphics.GraphicsDevice;
                int width = device.Viewport.Width;
                int height = device.Viewport.Height;
                Vector2 zoom = Main.GameViewMatrix.Zoom;
                view =
                    Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up)
                    * Matrix.CreateTranslation(width / 2, height / -2, 0)
                    * Matrix.CreateRotationZ(MathHelper.Pi)
                    * Matrix.CreateScale(zoom.X, zoom.Y, 1f);
                projection = Matrix.CreateOrthographic(width, height, 0, 1000);
            }

            return view * projection;
        }

        public static Vector2 FromAToB(Vector2 a, Vector2 b, bool normalize = true, bool reverse = false)
        {
            Vector2 baseVel = b - a;
            if (normalize)
                baseVel.Normalize();
            if (reverse)
            {
                Vector2 baseVelReverse = a - b;
                if (normalize)
                    baseVelReverse.Normalize();
                return baseVelReverse;
            }
            return baseVel;
        }


        public static Texture2D GetExtraTexture(string tex, bool altMethod = false)
        {
            if (altMethod)
                return GetTextureAlt("Assets/Textures/" + tex);
            return GetTexture("Assets/Textures/" + tex);
        }
        public static Texture2D GetTexture(string path)
        {
            return ModContent.Request<Texture2D>("Divergency/" + path).Value;
        }
        public static Texture2D GetThisTexture(Item obj)
        {
            return TextureAssets.Item[obj.type].Value;
        }
        public static Texture2D GetThisTexture(NPC obj)
        {
            return TextureAssets.Npc[obj.type].Value;
        }
        public static Texture2D GetThisTexture(Projectile obj)
        {
            return TextureAssets.Projectile[obj.type].Value;
        }
        public static Texture2D GetTextureAlt(string path)
        {
            return Divergency.Instance.Assets.Request<Texture2D>(path).Value;
        }
        public static Vector4 ColorToVector4(Color color)
        {
            return new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }
        public static Vector4 ColorToVector4(Vector4 color)
        {
            return new Vector4(color.X / 255f, color.Y / 255f, color.Z / 255f, color.W / 255f);
        }
        public static Vector4 ColorToVector4(Vector3 color)
        {
            return new Vector4(color.X / 255f, color.Y / 255f, color.Z / 255f, 1);
        }
        //public static Player[] activePlayers = new Player[Main.maxPlayers];
        //public static Player GetRandomPlayer()
        //{
        //   return Main.player[Main.rand.Next(activePlayers.Length)];
        //}
        public static SpriteSortMode previousSortMode;
        public static BlendState previousBlendState;
        public static SamplerState previousSamplerState;
        public static DepthStencilState previousDepthStencilState;
        public static RasterizerState previousRasterizerState;
        public static Effect previousEffect;
        public static Matrix previousMatrix;

        public static void SaveCurrent(this SpriteBatch spriteBatch)
        {
            previousSortMode = SpriteSortMode.Deferred;
            previousBlendState = (BlendState)spriteBatch.GetType().GetField("blendState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            previousSamplerState = (SamplerState)spriteBatch.GetType().GetField("samplerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            previousDepthStencilState = (DepthStencilState)spriteBatch.GetType().GetField("depthStencilState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            previousRasterizerState = (RasterizerState)spriteBatch.GetType().GetField("rasterizerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            previousEffect = (Effect)spriteBatch.GetType().GetField("customEffect", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            previousMatrix = (Matrix)spriteBatch.GetType().GetField("transformMatrix", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
        }

        public static void ApplySaved(this SpriteBatch spriteBatch)
        {
            if ((bool)spriteBatch.GetType().GetField("beginCalled", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch))
            {
                spriteBatch.End();
            }
            SpriteSortMode sortMode = previousSortMode;
            BlendState blendState = previousBlendState;
            SamplerState samplerState = previousSamplerState;
            DepthStencilState depthStencilState = previousDepthStencilState;
            RasterizerState rasterizerState = previousRasterizerState;
            Effect effect = previousEffect;
            spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, previousMatrix);
        }
        public static void Reload(this SpriteBatch spriteBatch, SpriteSortMode sortMode = SpriteSortMode.Deferred)
        {
            if ((bool)spriteBatch.GetType().GetField("beginCalled", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch))
            {
                spriteBatch.End();
            }
            BlendState blendState = (BlendState)spriteBatch.GetType().GetField("blendState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            SamplerState samplerState = (SamplerState)spriteBatch.GetType().GetField("samplerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            DepthStencilState depthStencilState = (DepthStencilState)spriteBatch.GetType().GetField("depthStencilState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            RasterizerState rasterizerState = (RasterizerState)spriteBatch.GetType().GetField("rasterizerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            Effect effect = (Effect)spriteBatch.GetType().GetField("customEffect", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            Matrix matrix = (Matrix)spriteBatch.GetType().GetField("transformMatrix", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, matrix);
        }
        public static void Reload(this SpriteBatch spriteBatch, BlendState blendState = default)
        {
            if ((bool)spriteBatch.GetType().GetField("beginCalled", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch))
            {
                spriteBatch.End();
            }
            SpriteSortMode sortMode = SpriteSortMode.Deferred;
            SamplerState samplerState = (SamplerState)spriteBatch.GetType().GetField("samplerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            DepthStencilState depthStencilState = (DepthStencilState)spriteBatch.GetType().GetField("depthStencilState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            RasterizerState rasterizerState = (RasterizerState)spriteBatch.GetType().GetField("rasterizerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            Effect effect = (Effect)spriteBatch.GetType().GetField("customEffect", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            Matrix matrix = (Matrix)spriteBatch.GetType().GetField("transformMatrix", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, matrix);
        }
        public static void Reload(this SpriteBatch spriteBatch, Effect effect = null)
        {
            if ((bool)spriteBatch.GetType().GetField("beginCalled", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch))
            {
                spriteBatch.End();
            }
            SpriteSortMode sortMode = SpriteSortMode.Deferred;
            BlendState blendState = (BlendState)spriteBatch.GetType().GetField("blendState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            SamplerState samplerState = (SamplerState)spriteBatch.GetType().GetField("samplerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            DepthStencilState depthStencilState = (DepthStencilState)spriteBatch.GetType().GetField("depthStencilState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            RasterizerState rasterizerState = (RasterizerState)spriteBatch.GetType().GetField("rasterizerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            Matrix matrix = (Matrix)spriteBatch.GetType().GetField("transformMatrix", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, matrix);
        }
    }
}