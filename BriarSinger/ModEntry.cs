global using Vector2 = Microsoft.Xna.Framework.Vector2;
using BriarSinger.Framework;
using BriarSinger.Spells;
using BriarSinger.Spells.Components;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore;
using SpaceCore.Events;
using SpaceShared.APIs;
using SpaceShared.ConsoleCommands;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Weapons;
using StardewValley.Monsters;
using StardewValley.Tools;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Serialization;
using static BriarSinger.Spells.SpellValidation;




namespace BriarSinger
{
    public class ModEntry : Mod
    {
        public static Mod ModInstance;
        public static IModHelper helper;
        internal ModConfig Config { get; set; }
        private Events EventHandlers;

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
            this.Config = this.Helper.ReadConfig<ModConfig>();
            this.EventHandlers = new Events(this);
            SetUpEvents();

            //Harmony things, look here first if stuff is broke.
            Harmony harmony = new Harmony(ModManifest.UniqueID);

            /*  harmony.Patch(
                  original: AccessTools.Method(typeof(MeleeWeapon), "beginSpecialMove"),
                  prefix: new HarmonyMethod(typeof(ModEntry), nameof(PerformSpecialAttack_Prefix))
              );
*/


         //this forces the game to create the Harpsword as a subclass instead of a generic melee weapon so that attacks will use the custom logic
            var createMethod = typeof(ItemRegistry)
     .GetMethods(BindingFlags.Public | BindingFlags.Static)
     .First(m => m.Name == "Create"
              && !m.IsGenericMethodDefinition
              && m.GetParameters().Select(p => p.ParameterType)
                  .SequenceEqual(new[] { typeof(string), typeof(int), typeof(int), typeof(bool) }));

            harmony.Patch(
                original: createMethod,
                postfix: new HarmonyMethod(typeof(ItemRegistry_Create_HarpSwordPatch), nameof(ItemRegistry_Create_HarpSwordPatch.Postfix))
            );
        }
        
        //This should be intercepting the right click on the Harpsword and telling it to not.
        public static bool beginSpecialMove_Prefix(MeleeWeapon __instance, Farmer who)
        {
            try
            {
                if (__instance.Name == "HarpSword")
                {
                    return false;
                }
                return true;

            }
            catch { return false; }
        }

        public static class ItemRegistry_Create_HarpSwordPatch
        {
            public static void Postfix(string itemId, int amount, int quality, bool allowNull, ref Item __result)
            {
                if (__result is MeleeWeapon weapon
                    && weapon is not HarpSword
                    && weapon.ItemId == "Teoshen.BriarSinger_HarpSword")
                {
                    __result = new HarpSword(itemId)
                    {
                        Stack = weapon.Stack,
                        Quality = weapon.Quality
                    };
                }
            }
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
            helper.Events.Input.ButtonPressed += this.EventHandlers.OnButtonPressed;
            helper.Events.GameLoop.GameLaunched += this.EventHandlers.OnGameLaunched;
            helper.Events.Content.AssetRequested += this.EventHandlers.OnAssetRequested;
            helper.Events.GameLoop.OneSecondUpdateTicked += this.EventHandlers.OneSecondUpdateTicked;
            helper.Events.Input.ButtonReleased += this.EventHandlers.OnButtonReleased;
            SpaceEvents.BeforeGiftGiven += this.EventHandlers.GivingGift;
            helper.Events.GameLoop.UpdateTicking += this.EventHandlers.GameLoopUpdateTicking;
            helper.Events.Display.RenderedHud += this.EventHandlers.OnRenderedHud;
        }


        // This is the thing that lets the code know if you're shooting the bow.
        public static bool IsActionButtonDown { get; internal set; }

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