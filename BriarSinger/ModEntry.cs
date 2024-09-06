﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using SpaceCore;
using SpaceShared.APIs;
using SpaceShared.ConsoleCommands;
using StardewValley.Monsters;
using StardewValley.GameData.Weapons;
using StardewValley.GameData.Objects;

using BriarSinger.Spells.Components;
using BriarSinger.Framework;
using SpaceCore.Events;


namespace BriarSinger
{
    public class ModEntry : Mod
    {
        public static Mod ModInstance;
        public static IModHelper helper;
        private ModConfig Config;

        //Calling APIs and related content
        public static IManaBarApi ManaBarApi;
        public static ContentPatcher.IContentPatcherAPI ContentPatcherApi;
        public static string BriarSingerContentPatcherId = "Teoshen.CP.BriarSinger";
        public static readonly string HARPSWORD_WEAPON_ID = "HarpSword";
        public static readonly string HARP_OBJECT_ID = "HarpObject";
        private bool isActionButtonDown;

        /// <summary>
        /// Initalize all the classes in ModEntry
        /// </summary>
        /// <param name="helper"></param>
        public override void Entry(IModHelper helper)
        {
            ModEntry.helper = helper;

            LoadAssets();
            SetUpEvents();
            this.Config = this.Helper.ReadConfig<ModConfig>();
            ModInstance = this;

            //Console Command
            //helper.ConsoleCommands.Add("player_giveharpsword", "...", (cmd, args) => Game1.player.addItemByMenuIfNecessary(new HarpSword()));

        }

        //Initialize mana bar
        private static Texture2D ManaBg;
        private static Texture2D ManaFg;



        //Defines the color of the mana bar and loads it.
        private static void LoadAssets()
        {
            ModEntry.ManaBg = helper.ModContent.Load<Texture2D>("assets/farmer/manabg.png");
            Color manaCol = new Color(120, 48, 255);
            ModEntry.ManaFg = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            ModEntry.ManaFg.SetData(new[] { manaCol });
        }


        //Sets up events for the mod.
        private void SetUpEvents()
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            helper.Events.GameLoop.OneSecondUpdateTicked += this.OneSecondUpdateTicked;
            helper.Events.Input.ButtonReleased += this.OnButtonReleased;
            SpaceEvents.BeforeGiftGiven += this.GivingGift;
          //  helper.Events.GameLoop.UpdateTicking += this.GameLoopUpdateTicking;
        }

        ///<summary>Event called when the game launches to load content packs.</summary>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
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
                configMenu.AddKeybind(
                      mod: this.ModManifest,
                      name: () => "Bolt",
                      tooltip: () => "The button to cast Bolt",
                      getValue: () => this.Config.CastBoltButton,
                      setValue: value => this.Config.CastBoltButton = value
                  );
            }

            //hook Mana Bar
            {
                var manaBar = this.Helper.ModRegistry.GetApi<IManaBarApi>("spacechase0.ManaBar");

                if (manaBar == null)
                {
                    this.Monitor.Log("No mana bar API", LogLevel.Error);
                    return;
                }

                ModEntry.ManaBarApi = manaBar;

            }

            //hook Content Patcher
            {
                var api = this.Helper.ModRegistry.GetApi<ContentPatcher.IContentPatcherAPI>("Pathoschild.ContentPatcher");
                ModEntry.ContentPatcherApi = api;
            }
        }

  
        /// <summary>
        /// Event for when the mod requests assets to be loaded.
        /// </summary>
        
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            //Currently for the projectile info, only for Bolt, will try to make it dynamic as I add more spells.
            if (e.Name.IsEquivalentTo("BriarSinger/MyProjectile"))
            {
                e.LoadFromModFile<Texture2D>("assets/spellcomponents/boltprojectile.png", AssetLoadPriority.Medium);
            }
            if (e.Name.IsEquivalentTo("BriarSinger/MyProjectile2"))
            {
                e.LoadFromModFile<Texture2D>("assets/spellcomponents/starshotprojectile.png", AssetLoadPriority.Medium);
            }
            if (e.Name.IsEquivalentTo("BriarSinger/MyProjectile3"))
            {
                e.LoadFromModFile<Texture2D>("assets/spellcomponents/starshottail.png", AssetLoadPriority.Medium);
            }            //don't judge me I promise I'll fix this when I have the tilesheet made

            // Adds the Harpsword weapon to the game.
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Weapons"))
             {
                  e.Edit(asset =>
                     {
                    IAssetDataForDictionary<string, WeaponData> editor = asset.AsDictionary<string, WeaponData>();

                    editor.Data[HARPSWORD_WEAPON_ID] = new WeaponData
                             {
                        Name = "HarpSword",
                        DisplayName = "HarpSword",
                        Description = "Your harp, modified with forest magic to be a weapon.",
                        Type = 3,
                        SpriteIndex = 0,
                        Texture = "BriarSinger/weapons",
                        MinDamage = 15,
                        MaxDamage = 25,
                        Speed = 4,
                        CanBeLostOnDeath = false,
                        AreaOfEffect = 2,
                        CritChance = 0.04f,
                        CritMultiplier = 3.5f,
                        Precision = 10
                             };
                    });
             } 
          
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                e.Edit(asset =>
                {
                    IAssetDataForDictionary<string, ObjectData> editor = asset.AsDictionary<string, ObjectData>();
                    editor.Data[HARP_OBJECT_ID] = new ObjectData
                    {
                        Name = "Harp",
                        DisplayName = "Mini Harp",
                        Description = "It's your old mini harp. In good condition, but could use some tuning.",
                        SpriteIndex = 0,
                        Texture = "BriarSinger/Objects",
                        Price = 200,
                        Type = "Objects",
                    };
                });
            }
            //Add the harpsword weapon icon to the weapon sprite sheet.
            if (e.NameWithoutLocale.IsEquivalentTo("BriarSinger/weapons"))
            {
                e.LoadFromModFile<Texture2D>("assets/farmer/harpsword.png", AssetLoadPriority.Medium);
            }
            if (e.NameWithoutLocale.IsEquivalentTo("BriarSinger/Objects"))
            {
                e.LoadFromModFile<Texture2D>("assets/farmer/harpobject.png", AssetLoadPriority.Medium);
            }
            
        }

        ///<summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        public void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            var caster = Game1.player;

            if (Game1.activeClickableMenu != null)
                return;

            if (Context.IsPlayerFree)
                if (e.Button == this.Config.CastBoltButton)
                    {
                     CastBolt(caster);
                    }
             if (caster.CurrentTool?.Name == "HarpSword" && e.Button.IsActionButton())
                 {
                //GameLocation location, int x, int y, int power, Farmer who
              //  HarpSword.DoFunction(Game1.currentLocation, 0,0,0, caster);
                Game1.playSound("slingshot");
                  CastStarShot(caster);
                 } // add an option that if the MeleeWeapon.defenseCooldown >0 it casts normally but if the defenseCooldown is at 0, it costs no mana or does more damage or something.
         
            
            if (e.Button.IsActionButton())
            {
                this.isActionButtonDown = true;
            }
             
            ModEntry.FixMana(Game1.player); //change this after everything is done to be the OnDayStarted event that sets mana to full
        }

        public void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (e.Button.IsActionButton())
                this.isActionButtonDown = false;
            //put stuff here for if you're holding the harpsword and release button, it shoots a projectile
        }

        // Event run each second
        public void OneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            { 
            ManaRegen(Game1.player);
            }
        }

        //Event to stop the player from giving away their harp object
        public void GivingGift(object sender, EventArgsBeforeReceiveObject e)
        {
            if (e.Gift.Name == "Harp")
            {
                e.Cancel = true;
                //I know that there's base game function to do this but I already wrote it before I figured it out and it works.
            }
        }

        /// <summary>
        /// Ok this one is a long shot, but if the player is holding the harpsword (which we might make a slingshot later) and the action button is down and blah blah it will enter the slingshot minigame.
        /// </summary>
     /*   private void GameLoopUpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            var caster = Game1.player;

            if (caster.CurrentTool?.Name == "HarpSword" && this.isActionButtonDown == true)
            {
                UseHarpBow(caster);
            }
        }
     */




        //Bolt spell information
        private void CastBolt(Farmer caster)
        {
            Monster closestMonster = MonsterHelper.GetClosestMonsterToCursor();
            int damage = 25; //replace this with the scaling factor when the profession levels are added
            if (closestMonster == null)
                {
                return;
            }
              Game1.currentLocation.projectiles.Add(new Bolt(damage, 0, 10, closestMonster.getStandingPosition() + new Vector2(-16, -256), 4, true, caster.currentLocation, caster));
        }

        //Starshot spell information
        private void CastStarShot(Farmer caster)
        {
            int damage = 25; //replace this with better randomized math later
            var target = ModEntry.ModInstance.Helper.Input.GetCursorPosition();
            Vector2 velocity = target.AbsolutePixels - Game1.player.getStandingPosition();
            velocity.Normalize();
            velocity *= 15;

            Game1.currentLocation.projectiles.Add(new StarShot(damage, velocity, caster.getStandingPosition() + new Vector2(0, -64), 6, caster.currentLocation, caster));
        }
     

        //UseHarpBow function information
     /*   private void UseHarpBow(Farmer caster)
        {
           int damage = 10;
            Vector2 velocity1 = TranslateVector(new Vector2(0, 10), caster.FacingDirection);
            Vector2 startPos1 = TranslateVector(new Vector2(0, 96), caster.FacingDirection);
            
            Game1.currentLocation.projectiles.Add(new HarpSword.PerformFire(damage, velocity1.X, velocity1.Y, caster.getStandingPosition() + new Vector2(0, -64) + startPos1, 6, caster.currentLocation, caster));
          
        }
     */

        //Math to figure out if the game needs to change your direction based on where you cast your spell.
      /* public static Vector2 TranslateVector(Vector2 vector, int facingDirection)
        {
            float outx = vector.X;
            float outy = vector.Y;
            switch (facingDirection)
            {
                case 2:
                    break;
                case 3:
                    outx = -vector.Y;
                    outy = vector.X;
                    break;
                case 0:
                    outx = -vector.X;
                    outy = -vector.Y;
                    break;
                case 1:
                    outx = vector.Y;
                    outy = -vector.X;
                    break;
            }
            return new Vector2(outx, outy);
        }
      */

        //Add some mana to make sure the mana bar will show up. Change this after mana is complete so that it refills your mana bar at the start of the day.
        public static void FixMana(Farmer player)
        {
            if (player.getMaxMana() <= 0)
            {
                player.setMaxMana(100);
                player.addMana(25);
            }
        }
        public static void ManaRegen(Farmer player)
        {
            if (player.GetCurrentMana() < 100)
            {
            player.addMana(1); //replace with the formula for mana additions after the profession levels are added
            }
        }

    }
    
}