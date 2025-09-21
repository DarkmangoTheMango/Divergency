global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using Terraria;
global using Terraria.DataStructures;
global using Terraria.ID;
global using Terraria.ModLoader;
using ReLogic.Content;
using System.Reflection;
using Terraria.WorldBuilding;

namespace Divergency
{
    public partial class Divergency : Mod
    {
        public static Effect BeamShader, Lens, Test1, Test2, LavaRT, Galaxy, CrystalShine, TrailShader, RTAlpha;

        public class TemporaryFix : PreJITFilter
        {
            public override bool ShouldJIT(MemberInfo member) => false;
        }

        public static Divergency Instance { get; set; }

        public Divergency()
        {
            Instance = this;
            PreJITFilter = new TemporaryFix();
        }

        public override void Unload()
        {
            if (!Main.dedServ)
            {
                Instance = null;
            }
        }

        internal static Asset<Effect> Supernova;

        public override void Load()
        {
            BeamShader = ModContent.Request<Effect>("Divergency/Common/Helpers/Beam", (AssetRequestMode)1).Value;
            Supernova = ModContent.Request<Effect>("Divergency/Content/Effects/Supernova");
            TrailShader = ModContent.Request<Effect>("Divergency/Content/Effects/Trailshader", (AssetRequestMode)1).Value;
        }
    }

    static class DivergencyUtils
    {
        public static Vector2 findGroundUnder(this Vector2 position)
        {
            Vector2 returned = position;
            while (!WorldUtils.Find(returned.ToTileCoordinates(), Searches.Chain(new Searches.Down(1), new GenCondition[]
                {
                new Conditions.IsSolid()
                }), out _))
            {
                returned.Y++;
            }

            return returned;
        }
    }
}