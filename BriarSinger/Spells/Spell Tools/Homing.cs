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


namespace BriarSinger.Spells
{
    internal static class Homing
    {
        // Rotates currentVelocity towards targetPosition by the turnRate in radians per second, keeping speed constant.
        // Returns currentVelocity unchanged if speed is 0 or target is on the projectile.

        public static Vector2 SteerToward(Vector2 currentVelocity, Vector2 position, Vector2 targetPosition, float turnRate, float deltaSeconds)
        {
            float speed = currentVelocity.Length();
            if (speed <= 0f) return currentVelocity; //don't divide by zero

            Vector2 toTarget = targetPosition - position;

            if (toTarget == Vector2.Zero) return currentVelocity;
            toTarget.Normalize(); //if projectile is on the target, normalize turns the length to 1 so it doesn't return NaN

            Vector2 currentDir = currentVelocity / speed; //make a length 1 arrow showing direction
            float currentAngle = (float)Math.Atan2(currentDir.Y, currentDir.X); //convert current direction into an angle in radians from x axis
            float targetAngle = (float)Math.Atan2(toTarget.Y, toTarget.X); //find the direction you want to go

            float angleDiff = MathHelper.WrapAngle(targetAngle - currentAngle); //subtract angles, and wrap to the shortest path
            float maxTurn = turnRate * deltaSeconds; //how much it is allowed to turn per frame. If the turnrate is 6f (344 degrees), it will turn 0.1 radians (5.7 degrees) per frame
            float appliedTurn = MathHelper.Clamp(angleDiff, -maxTurn, maxTurn); //clamp keeps the target angle from overshooting the target

            float newAngle = currentAngle + appliedTurn; 
            return new Vector2((float)Math.Cos(newAngle), (float)Math.Sin(newAngle)) * speed; //sin and cos turn the angle back into length 1 arrows, multiply by the saved speed
        }
    }    
}
