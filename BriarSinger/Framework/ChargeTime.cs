using BriarSinger;
using BriarSinger.Spells;
using BriarSinger.Spells.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;


namespace BriarSinger.Framework
{
    public class ChargeTime
    {

        private static bool chargeComplete;
        private static double pullStartTime;
        private static bool canPlaySound = true;

        internal static float GetChargeTime()
        {
            var caster = Game1.player;


            if (caster.CurrentTool?.Name == "HarpSword" && ModEntry.IsActionButtonDown)
            {
                if (chargeComplete == true)
                {

                    return 1f;
                }
                var requiredChargeTime = 750;
                var chargeStartTime = pullStartTime;

                //does math to make the decimal between 0 and 1 for how long it takes to charge.
                return Utility.Clamp((float)((Game1.currentGameTime.TotalGameTime.TotalMilliseconds - chargeStartTime) / (double)requiredChargeTime), 0f, 1f);

            }
            return 0f;
        }

        public static void TickUpdate(Farmer who)
        {

            if (who.CurrentTool?.Name != "HarpSword")
            {
                return;
            }
            if (who.IsLocalPlayer)
            {
                var currentChargeTime = GetChargeTime();

                //after the charge finishes, play a sound and a sparkle so the player knows it's ready to release
                if (currentChargeTime >= 1f)
                {
                    chargeComplete = true;
                    if (canPlaySound == true)
                    {
                        Game1.currentLocation.playSound("select");
                        SpawnTwinkle(who);
                        canPlaySound = false;
                    }
                }
                else if (currentChargeTime < 1f)
                {
                    chargeComplete = false;
                }
            }
        }

        public static void ReleaseCharge(Farmer caster)
        {
            if (ModEntry.IsActionButtonDown == false)
            {
                if (chargeComplete)
                {
                    SpellValidation.TryCastSpell(caster, Spellbook.StarShotSpell);
                }
                ResetCharge();

            }
        }

        //Makes the player hold still while charging
        public static void WhileCharging()
        {
            if (ModEntry.IsActionButtonDown)
            {
                chargeComplete = false;
                Game1.player.UsingTool = true;
            }
        }

        private static void ResetCharge()
        {
            chargeComplete = false;
            Game1.player.CanMove = true;
            canPlaySound = true;
        }


        public static void SetChargeTime(float percentage)
        {
            var caster = Game1.player;


            if (caster.CurrentTool?.Name == "HarpSword")
            {
                var currentMilliseconds = Game1.currentGameTime.TotalGameTime.TotalMilliseconds;
                var requiredChargingTime = 750;

                percentage = Utility.Clamp(percentage, 0f, 1f);
                pullStartTime = Math.Abs(currentMilliseconds - (percentage * requiredChargingTime));
            }
        }
        public static void DrawReticle(SpriteBatch b)
        {
            SpriteBatch spriteBatch = Game1.spriteBatch;
            
            Point mousePos = Game1.getMousePosition();
            Vector2 mousePosVector = new Vector2(mousePos.X, mousePos.Y);
            b.Draw(Game1.mouseCursors,
                mousePosVector,
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 43),
                Color.White,
                0f,
                new Vector2(32f, 32f),
                1f,
                SpriteEffects.None,
                0.999999f);
        }
        

        private static void SpawnTwinkle(Farmer who)
        {
            Game1.Multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(352, 
                who.Position + new Vector2(0, -96), 
                Color.White));
        }
    }
}
