using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace Divergency.Content.Events.LivingCore.InfiniteRoomParts
{
    public class ActivePattern
    {
        public Unit unit;
        public Vector2 position;

        // public List<List<ActivePattern>> participatingPatternGroups = new List<List<ActivePattern>>();

        public ActivePattern(string name, Vector2 pos)
        {
            position = pos;

            foreach (Unit u in Unit.units)
            {
                if (u.fullName == name)
                {
                    unit = u;
                    return;
                }
            }

            unit = new Unit("", 99999, 9);
        }
        public ActivePattern(Unit u, Vector2 pos) { position = pos; unit = u; }

        public ActivePattern Clone()
        {
            ActivePattern clone = new ActivePattern(unit, position);

            /*
            List<List<ActivePattern>> nllap = new List<List<ActivePattern>>();
            foreach (List<ActivePattern> lap in participatingPatternGroups)
            {
                List<ActivePattern> nlap = [.. lap];

                nllap.Add(nlap);
            }

            clone.participatingPatternGroups = nllap;
            */

            return clone;
        }
    }

    public abstract class Pattern
    {
        public static List<Pattern> patterns = new List<Pattern>
        {
            new FixedPosPattern(), new MirrorPattern(), new CenterPosPattern(), new FixedFarPosPattern(),
        };
        public static List<Pattern> killPatterns = new List<Pattern>
        {
            new Kill.BlockersSidePattern(), new Kill.OverAndUnderPattern(), new Kill.CorennectorLimitPattern(),
            new Kill.NeedsDamageDealerPattern(), new Kill.SeerCapPattern(), new Kill.PersonalSpacePatter(),
        };

        public virtual float costMultiplier => 1;

        public virtual List<List<ActivePattern>> Possibilities(List<ActivePattern> activePatterns)
        {
            return new List<List<ActivePattern>>();
        }
        public virtual bool DoKill(List<ActivePattern> activePatterns) { return true; }
    }
}
