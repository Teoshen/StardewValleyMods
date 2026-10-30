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
            public bool TryResolve(Farmer caster, int range, out List<Monster> target)
            {
                Vector2 center = Game1.currentCursorTile * 64f;
                target = Game1.currentLocation.characters
                    .OfType<Monster>()
                    .Where(m => Vector2.Distance(m.Position, center) <= range)
                    .ToList();

                return target.Count > 0;
            }
        }

        public class GroundTileTargeting : ITargetingStrategy<Vector2>
        {
            public bool TryResolve(Farmer caster, int range, out Vector2 target)
            {
                target = Game1.currentCursorTile;
                return Game1.currentLocation.isTileLocationOpen(
                    new xTile.Dimensions.Location((int)target.X, (int)target.Y));
            }
        }

        public class SingleMonsterTargeting : ITargetingStrategy<Monster>
        {
            public bool TryResolve(Farmer caster, int range, out Monster target)
            {
                target = Utility.findClosestMonsterWithinRange(Game1.currentLocation, Game1.currentCursorTile * 64f, range);
                return target != null;
            }
        }

        public class CursorDirection : ITargetingStrategy<Vector2>
        {
            public bool TryResolve(Farmer caster, int range, out Vector2 target)
            {
                Vector2 casterCenter = caster.Position + new Vector2(32f, 32f);
                Vector2 cursorWorldPos = Game1.currentCursorTile * 64f + new Vector2(32f, 32f);

                Vector2 direction = cursorWorldPos - casterCenter;

                //zero-length vector
                if (direction == Vector2.Zero)
                    direction = new Vector2(0f, -1f);

                direction.Normalize();
                target = direction;
                return true;
            }
        }

        public class PlayerCenteredArea : ITargetingStrategy<List<Monster>>
        {
            public bool TryResolve(Farmer caster, int range, out List<Monster> target)
            {
                //omnidirectional circle around the caster
                Vector2 center = caster.Position;

                target = Game1.currentLocation.characters
                    .OfType<Monster>()
                    .Where(m => Vector2.Distance(m.Position, center) <= range)
                    .ToList();

                return target.Count > 0;
            }
        }

        public class FacingArcArea : ITargetingStrategy<List<Monster>>
        {
            private const float ArcHalfAngle = MathHelper.PiOver4; //45 degree each side, 90 degree cone

            //Circle around caster, in cone on their facing direction
            public bool TryResolve(Farmer caster, int range, out List<Monster> target)
            {
                Vector2 center = caster.Position;
                Vector2 facing = FacingDirectionToVector(caster.FacingDirection);

                target = Game1.currentLocation.characters
                    .OfType<Monster>() 
                    .Where(m =>
                    {
                    Vector2 toMonster = m.Position - center;
                    if (toMonster == Vector2.Zero)
                        return true; //standing on top of player
                    if (toMonster.Length() > range)
                        return false;

                    toMonster.Normalize();
                    float dot = Vector2.Dot(facing, toMonster);
                    float angle = (float)Math.Acos(MathHelper.Clamp(dot, -1f, 1f));
                    return angle <= ArcHalfAngle;
                })
                .ToList();

                return target.Count > 0;
            }

            private static Vector2 FacingDirectionToVector(int facingDirection) => facingDirection switch
            {
                0 => new Vector2(0f, -1f), // up
                1 => new Vector2(1f, 0f),  // right
                2 => new Vector2(0f, 1f),  // down
                3 => new Vector2(-1f, 0f), // left
                _ => new Vector2(0f, 1f),
            };
        }
        
    }
}
