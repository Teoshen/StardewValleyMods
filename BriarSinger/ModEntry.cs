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
using System.Numerics;



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
        //This should be intercepting the right click on the Harpsword and telling it to not.

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


        // This is the thing that lets the code know if you're shooting the bow.
        public static bool IsActionButtonDown { get; private set; }

        //Checks to see if the current cursor tile is an interactable so it knows to not activate the bow part if you're going through a door or ladder
        private static bool IsInteractableTile(GameLocation location, Vector2 tile)
        {
            int tx = (int)tile.X; int ty = (int)tile.Y;

            bool hasAction = location.isActionableTile(tx, ty, Game1.player);
            bool hasTouchAction =
                !string.IsNullOrEmpty(location.doesTileHaveProperty(tx, ty, "TouchAction", "Back")) ||
                !string.IsNullOrEmpty(location.doesTileHaveProperty(tx, ty, "TouchAction", "Buildings"));

            return hasAction || hasTouchAction;
        }

        //Add some mana to make sure the mana bar will show up. Change this after mana is complete so that it refills your mana bar at the start of the day.
        public static void FixMana(Farmer player)
        {
            if (player.getMaxMana() <= 0)
            {
                player.setMaxMana(100);
                player.addMana(75);
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