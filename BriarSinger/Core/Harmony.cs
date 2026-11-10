using BriarSinger.Framework;
using BriarSinger.Spells;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static BriarSinger.Core.ModEntry;

namespace BriarSinger.Core
{
    internal class Harmony
    {
        // Harmony things, look here first if stuff is broken.
        HarmonyLib.Harmony harmony = new HarmonyLib.Harmony(ModEntry.UniqueID);

        //Item registry interception
        public Harmony()
        {
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

            // stops the normal defense block from happening
            harmony.Patch(
                original: AccessTools.Method(typeof(MeleeWeapon), "beginSpecialMove"),
                prefix: new HarmonyMethod(typeof(Harmony), nameof(beginSpecialMove_Prefix))
            );
        }

        //this forces the game to create the Harpsword as a subclass instead of a generic melee weapon so that attacks will use the custom logic
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

        //stops the regular block and plays the PowerChord instead
        public static bool beginSpecialMove_Prefix(MeleeWeapon __instance)
        {
            if (__instance is not HarpSword)
                return true;

            Farmer who = __instance.getLastFarmerToUse();
            if (who is not null &&
                Context.IsWorldReady &&
                Context.IsPlayerFree &&
                Game1.activeClickableMenu == null)
            {
                SpellValidation.TryCastSpell(who, Spellbook.PowerChordSpell);
            }

            // Prevent the normal defense block.
            return false;
        }

        //Adds profession XP when you kill an enemy with the HarpSword
        [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.onMonsterKilled))]
        public static class MonsterKilledPatch
        {
            public static void Postfix(Farmer who, Monster monster)
            {
                if (who?.CurrentTool is not MeleeWeapon weapon) return;
                if (weapon.ItemId != "Teoshen.BriarSinger_HarpSword") return;

                int xp = Math.Max(1, monster.ExperienceGained / 2);
                AddEXP(who, xp);
            }
        } //TODO fun thing to test, don't know if I named everything correctly

        /*  //This should be intercepting the right click on the Harpsword and telling it to not.
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
        */
    }
}
