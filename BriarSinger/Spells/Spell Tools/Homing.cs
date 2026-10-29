using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpaceCore;
using StardewValley.Projectiles;
using StardewValley.TerrainFeatures;
using StardewValley;
using StardewValley.Monsters;

/* 
 * namespace BriarSinger.Spells
{
    internal class Homing
    {
        public override bool update(GameTime time, GameLocation location)
        {
            if (homing)
            {
                if (homeTarget == null || homeTarget.Health <= 0 || homeTarget.currentLocation == null)
                {
                    disappear(location);
                    return true;
                }
                else
                {
                    Vector2 unit = new Vector2(homeTarget.GetBoundingBox().Center.X+ 32, hometarget.GetBoundingBox().Center.Y + 32) - position;
                    unit.Normalize();

                    xVelocity.Value = unit.X * velocity;
                    yVelocity.Value = unit.Y * velocity;
                }
            }
            return base.update(time, location);
        }
        //put code here that does math to tell homing projectiles which way to move to stay on target to the enemy.
        //Will be used for bolt and the entangling roots spell.


    }
}
*/