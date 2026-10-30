using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriarSinger.Framework
{
    public static class SwingCharge
    {
        private const float ChargeThresholdMs = 100f; //below this is a regular attack
        private const float MaxChargeMs = 300f; //full charge

        private static float heldMs;
        private static bool isPressed;
        private static bool isCharging;

        public static void StartPress()
        {
            isPressed = true;
            isCharging = false;
            heldMs = 0;
        }

        public static void Tick(GameTime gameTime)
        {
            if (!isPressed) return;

            heldMs += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (!isCharging && heldMs >= ChargeThresholdMs)
            {
                isCharging = true;
                Game1.player.UsingTool = true; //stop player from moving
                Game1.playSound("miniharp_note");
            }
        }

        public static void Release(Farmer who)
        {
            if (!isPressed) return;
            isPressed = false;

            if (isCharging)
            {
                float chargeRatio = Math.Min(heldMs / MaxChargeMs, 1f);
                HarpSword.PerformPowerChord(who, chargeRatio);
            }
            else
            {
                HarpSword.PerformDirectionalSwing(who);
            }
            isCharging = false;
            who.UsingTool = false; //lets the player move again
        }

        public static void DrawChargeBar(SpriteBatch b)
        {
            if (!isCharging) return;
            float pct = Math.Min(heldMs / MaxChargeMs, 1f); //draw the charging bar above the player using the watering can thing
        }
    }
}
