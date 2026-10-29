using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;

using StardewValley.Constants;
using StardewValley.GameData.LocationContexts;
using StardewValley.Extensions;


using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using SpaceShared.APIs;
using System.Runtime.CompilerServices;



namespace HotSpringRegeneration
{
    public class ModEntry : Mod
    {
        private ModConfig Config;
        private double staminaAccum;
        private double healthAccum;
        private double stamCounter;
        private double healthCounter;

        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();

            Helper.Events.Player.Warped += Player_Warped;
            Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        }

        private void GameLoop_GameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (configMenu is null)
                return;
            {
                configMenu.Register(
                    mod: this.ModManifest,
                    reset: () => this.Config = new ModConfig(),
                    save: () => this.Helper.WriteConfig(this.Config)
                    );
             
                configMenu.AddNumberOption(
             mod: this.ModManifest,
             name: () => "Stamina Regeneration Multiplier",
             getValue: () => this.Config.StamRegenMult,
             setValue: value => this.Config.StamRegenMult = value,
             min: 10,
             max: 200,
             interval: 10

   );
                configMenu.AddNumberOption(
             mod: this.ModManifest,
             name: () => "Health Regeneration Multiplier",
             getValue: () => this.Config.HealthRegenMult,
             setValue: value => this.Config.HealthRegenMult = value,
             min: 10,
             max: 200,
             interval: 10

   );

            }
        }

        private void Player_Warped(object? sender, WarpedEventArgs e)
        {
            if (Game1.currentLocation.NameOrUniqueName != "FarmCave")
            {
                Helper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked;
                return;
            }
            else if ((Game1.currentLocation.NameOrUniqueName == "FarmCave"))
            {
                Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;

            }
        }

        private void GameLoop_UpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            Farmer player = Game1.player;
            var location = Game1.currentLocation;
                         

            if (player == null || Game1.paused)
            {
                return;
            }

            if (!Game1.eventUp && (Game1.activeClickableMenu == null || Game1.IsMultiplayer) && !Game1.paused && Game1.game1.IsActive)
            {
                if (player.isMoving() || player.swimming.Value == false) return;
                {
                    double currentTime = Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
                    ChangeStaminaRegen(player, location, currentTime);
                    ChangeHealthRegen(player, location, currentTime);
                }
            }
        }

       

        private void ChangeStaminaRegen(Farmer player, GameLocation location, double currentTime)
        {
            
            if (Game1.currentLocation.NameOrUniqueName != "FarmCave" || Config.StamRegenMult == 100)
            {
                return;
            }

            else if (Game1.currentLocation.NameOrUniqueName == "FarmCave")
            {
                if (player.stamina < 20)
                {
                    return;
                }
                double stamAccumRate = (Config.StamRegenMult - 100) / 10.0;
                double stamMS = 1000.0 / Math.Abs(stamAccumRate);
           
                stamCounter -= currentTime;
                if (stamCounter <= 0 )
                {
                    stamCounter += stamMS;

                    if (stamAccumRate > 0 && player.stamina < player.MaxStamina)
                            player.stamina++;
                    else if (stamAccumRate < 0 && player.stamina > 5)
                            player.stamina--;
                } 
            }
        }



        private void ChangeHealthRegen(Farmer player, GameLocation location, double currentTime)
        {

            if (Game1.currentLocation.NameOrUniqueName != "FarmCave" || Config.HealthRegenMult == 100)
            {
                return;
            }

            else if (Game1.currentLocation.NameOrUniqueName == "FarmCave")
            {
                if (player.health < 20 || player.health >= player.maxHealth)
                {
                    return;
                }

                double healthAccumRate = (Config.HealthRegenMult - 100) / 10.0;
                double healthMS = 1000.0 / Math.Abs(healthAccumRate);

                healthCounter -= currentTime;

                if (healthCounter <= 0)
                {

                    healthCounter += healthMS;

                    if (healthAccumRate > 0 && player.health < player.maxHealth)
                        player.health++;
                    if (healthAccumRate < 0 && player.health > 5)
                        player.health--;
                }
            }
        }
    }
}
