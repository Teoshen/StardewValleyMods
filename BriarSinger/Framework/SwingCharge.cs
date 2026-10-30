using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.GameData.FloorsAndPaths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriarSinger.Framework
{
    public static class SwingCharge
    {

        public static bool IsCharging { get; private set; }
        public static float ChargeAmount { get; private set; }


        private const float ChargeThreshold = 100f; //below this is a regular attack
        private const float MaxCharge = 300f; //full charge
        private const float ChargeRatePerSecond = 300f;

        public static void StartCharging()
        {
            IsCharging = true;
            ChargeAmount = 0f;
        }

        public static void WhileCharging()
        {
            if (!IsCharging) return;
            ChargeAmount = Math.Min(MaxCharge, ChargeAmount + ChargeRatePerSecond * (float)Game1.currentGameTime.ElapsedGameTime.TotalSeconds);
            Game1.player.UsingTool = true;
        }

        public static void ReleaseCharge(Farmer who)
        {
            if (!IsCharging) return;
            IsCharging = false;
            Game1.player.UsingTool = false;

            if (ChargeAmount >= ChargeThreshold)
                HarpSword.PerformPowerChord(who, ChargeAmount);
            else
                HarpSword.PerformDirectionalSwing(who);

            ChargeAmount = 0f;

        }
    }
}





