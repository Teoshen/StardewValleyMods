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


namespace BriarSinger.Core
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
        internal static IMonitor ModMonitor;

        /// <summary>
        /// Initalize all the classes in ModEntry
        /// </summary>
        /// <param name="helper"></param>
        public override void Entry(IModHelper helper)
        {
            ModEntry.helper = helper;

            LoadAssets();
            Config = Helper.ReadConfig<ModConfig>();
            EventHandlers = new Events(this);
            SetUpEvents();

            //Harmony things, look here first if stuff is broke.
            Harmony harmony = new Harmony(ModManifest.UniqueID);

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
            ManaBg = helper.ModContent.Load<Texture2D>("assets/farmer/manabg.png");
            Color manaCol = new Color(110, 45, 191);
            ManaFg = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            ManaFg.SetData(new[] { manaCol });
        }

        //Sets up events for the mod.
        private void SetUpEvents()
        {
            helper.Events.Input.ButtonPressed += EventHandlers.OnButtonPressed;
            helper.Events.GameLoop.GameLaunched += EventHandlers.OnGameLaunched;
            helper.Events.Content.AssetRequested += EventHandlers.OnAssetRequested;
            helper.Events.GameLoop.OneSecondUpdateTicked += EventHandlers.OneSecondUpdateTicked;
            helper.Events.Input.ButtonReleased += EventHandlers.OnButtonReleased;
            SpaceEvents.BeforeGiftGiven += EventHandlers.GivingGift;
            helper.Events.GameLoop.UpdateTicking += EventHandlers.GameLoopUpdateTicking;
            helper.Events.Display.RenderedHud += EventHandlers.OnRenderedHud;
        }

        //Add some mana to make sure the mana bar will show up. Change this after mana is complete so that it refills your mana bar at the start of the day.
        public static void FixMana(Farmer player)
        {
            if (player.GetMaxMana() <= 0)
            {
                player.SetMaxMana(100);
                player.AddMana(75);
            }
        }

        //Per second event to add mana based on skill level
        public static void ManaRegen(Farmer player)
        {
            if (player.GetCurrentMana() < 100)
            {
                player.AddMana(1); //replace with the formula for mana additions after the profession levels are added
            }
        }

        //Skill information
        public const string SkillID = "Teoshen.BriarSinger";

        public static int GetLevel(Farmer who)
        {
            Farmer player = Game1.GetPlayer(who.UniqueMultiplayerID);
            return Skills.GetSkillLevel(player, ModEntry.SkillID);
        }
    }
}