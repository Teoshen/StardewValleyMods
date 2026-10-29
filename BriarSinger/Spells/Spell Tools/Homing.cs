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
using System.Numerics;


* namespace BriarSinger.Spells
{
    internal static class Homing
    {
        // Rotates currentVelocity towards targetPosition by the turnRate in radians per second, preserving speed.
        // Returns currentVelocity unchanged if speed is 0 or target is on the projectile.

        public static Vector2 SteerToward(Vector2 currentVelocity, Vector2 position, Vector2 targetPosition, float turnRate, float deltaSeconds)
        {
            float speed = currentVelocity.Length();
            if (speed <= 0f) return currentVelocity;

            Vector2 toTarget = targetPosition - position;
            if (toTarget == Vector2.Zero) return currentVelocity;
            toTarget.Normalize();

            Vector2 currentDir = currentVelocity / speed;
            float currentAngle = (float)Math.Atan2(currentDir.Y, currentDir.X);
            float targetAngle = (float)Math.Atan2(toTarget.Y, toTarget.X);

            float angleDiff = MathHelper.WrapAngle(targetAngle - currentAngle);
            float maxTurn = turnRate * deltaSeconds;
            float appliedTurn = MathHelper.Clamp(angleDiff, -maxTurn, maxTurn);

            float newAngle = currentAngle + appliedTurn;
            return new Vector2((float)Math.Cos(newAngle), (float)Math.Sin(newAngle)) * speed;
        }
    }
    
}
