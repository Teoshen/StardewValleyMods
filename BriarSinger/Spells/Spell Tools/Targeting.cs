using BriarSinger.Framework;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BriarSinger.Spells.SpellValidation;

namespace BriarSinger.Spells.Spell_Tools
{
    internal class Targeting
    {
            public class CircleAreaTargeting : ITargetingStrategy<List<Monster>>
        {
            public CastResult TryResolve(Farmer caster, int range, out List<Monster> target)
            {
                Vector2 center = Game1.currentCursorTile * 64f;
                target = Game1.currentLocation.characters
                    .OfType<Monster>()
                    .Where(m => Vector2.Distance(m.Position, center) <= range)
                    .ToList();

                return CastResult.Success;
            }
        }

        public class GroundTileTargeting : ITargetingStrategy<Vector2>
        {
            public CastResult TryResolve(Farmer caster, int range, out Vector2 target)
            {
                target = Game1.currentCursorTile;
                bool isOpen = Game1.currentLocation.isTileLocationOpen(new xTile.Dimensions.Location((int)target.X, (int)target.Y));
                return isOpen ? CastResult.Success : CastResult.NoValidTarget;
            }
        }

        public class SingleMonsterTargeting : ITargetingStrategy<Monster>
        {
            public CastResult TryResolve(Farmer caster, int range, out Monster target)
            {
                target = Utility.findClosestMonsterWithinRange(Game1.currentLocation, Game1.currentCursorTile * 64f, range);
                return target != null ? CastResult.Success : CastResult.NoValidTarget;
            }
        }

        public class CursorDirection : ITargetingStrategy<Vector2>
        {
            public CastResult TryResolve(Farmer caster, int range, out Vector2 target)
            {
                Vector2 casterCenter = caster.Position + new Vector2(32f, 32f);
                Vector2 cursorWorldPos = Game1.currentCursorTile * 64f + new Vector2(32f, 32f);

                Vector2 direction = cursorWorldPos - casterCenter;

                //zero-length vector
                if (direction == Vector2.Zero)
                    direction = new Vector2(0f, -1f);

                direction.Normalize();
                target = direction;
                return CastResult.Success;
            }
        }


    }
}
