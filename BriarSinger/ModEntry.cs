using System;
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
using SpaceCore.Events;
using HarmonyLib;

using BriarSinger.Spells.Components;
using BriarSinger.Framework;
using StardewValley.Tools;
using System.Xml.Serialization;
using BriarSinger.Spells;
using static BriarSinger.Spells.SpellValidation;



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
        private static bool isActionButtonDown;
        internal static IMonitor ModMonitor;

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

            //harmony things
            Harmony harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(MeleeWeapon), "beginSpecialMove"),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(PerformSpecialAttack_Prefix))
           );


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


        //Harmony things, look here first if stuff is broke.

        public static bool PerformSpecialAttack_Prefix(MeleeWeapon __instance, Farmer who)
        {
            try
            {
                if (__instance.Name == "HarpSword")
                {
                    return false; 
                }
                return true;

            }
            catch (Exception e)
            {
                ModMonitor.Log("$Failed to suppress weapon special attack.");
            }
            return true;
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
            helper.Events.GameLoop.UpdateTicking += this.GameLoopUpdateTicking;
            helper.Events.Display.RenderedHud += this.OnRenderedHud;
        }



        ///<summary>Event called when the game launches to load content packs.</summary>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API if it's installed
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
                configMenu.AddKeybind(
                    mod: this.ModManifest,
                    name: () => "Spell",
                    tooltip: () => "The button to cast the currently active spell",
                    getValue: () => this.Config.CastSpellButton,
                    setValue: value => this.Config.CastSpellButton = value
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

            /*
            //Add HarpSword serializer
            if (!helper.TryGetAPI("spacechase0.SpaceCore", "1.23.2", out ICompleteSpaceCoreAPI? SCapi))
            {

            }
            */
        }

        // This is the thing that lets the code know if you're shooting the bow.
        public static bool IsActionButtonDown { get; private set; }


        /// <summary>
        /// Event for when the mod requests assets to be loaded.
        /// </summary>

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
        
            // Spell Textures
            Bolt.ProjectileTexture = helper.ModContent.Load<Texture2D>("assets/MyProjectile.png");

            if (e.Name.IsEquivalentTo("BriarSinger/MyProjectile2"))
            {
                e.LoadFromModFile<Texture2D>("assets/spellcomponents/starsnew hotprojectile.png", AssetLoadPriority.Medium);
            }
            if (e.Name.IsEquivalentTo("BriarSinger/MyProjectile3"))
            {
                e.LoadFromModFile<Texture2D>("assets/spellcomponents/starshottail.png", AssetLoadPriority.Medium);
            }            //don't judge me I promise I'll fix this when I have the tilesheet made

            // Adds the Harpsword weapon to the game.
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Weapons"))
            {
                e.Edit(asset => HarpSword.EditWeaponData(asset.AsDictionary<string, WeaponData>().Data));                                
            }
            // Adds the Harp object to the game. 
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                e.Edit(asset => Harp.EditObjectData(asset.AsDictionary<string, ObjectData>().Data));               
            }

            //Add the harpsword weapon icon to the weapon sprite sheet.
            if (e.NameWithoutLocale.IsEquivalentTo("BriarSinger/weapons"))
            {
                HarpSword.LoadTexture(e);
            }
            //Add the harp object to the objects sprite sheet.
            if (e.NameWithoutLocale.IsEquivalentTo("BriarSinger/Objects"))
            {
                Harp.LoadTexture(e);
            }

        }

        ///<summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        public void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            var caster = Game1.player;

            if (Game1.activeClickableMenu != null)
                return;

            if (Context.IsPlayerFree)
            {
                if (e.Button == this.Config.CastBoltButton)
                {
                    SpellValidation.TryCastSpell(caster, Spellbook.BoltSpell);
                }
                if (e.Button == this.Config.CastSpellButton)
                {
                    Spellbook.ActiveSpell.TryCast(caster);
                }
            }
                     
            
            ///<summary>Add a section here to do the spell swap mechanic. Should be shift-right click while holding the HarpSword.</summary>

            if (e.Button.IsActionButton() && caster.CurrentTool?.Name == "HarpSword")
            {
                ModEntry.isActionButtonDown = true;
                Game1.playSound("slingshot");
                ChargeTime.SetChargeTime(0f);
                ChargeTime.WhileCharging();
            }

            ModEntry.FixMana(Game1.player); //change this after everything is done to be the OnDayStarted event that sets mana to full
        }

        public void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (e.Button.IsActionButton())
            {
                ModEntry.isActionButtonDown = false;
                ChargeTime.ReleaseCharge(Game1.player);
            }
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
        private void GameLoopUpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            var caster = Game1.player;

            if (caster.CurrentTool?.Name == "HarpSword" && ModEntry.isActionButtonDown)
            {
              ChargeTime.TickUpdate(caster);
            }
            CooldownHelper.Tick(Game1.currentGameTime);

        }

        private void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            if (Game1.player.CurrentTool?.Name == "HarpSword" && ModEntry.isActionButtonDown)
            {
                ChargeTime.DrawReticle(e.SpriteBatch);
            }
        }


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