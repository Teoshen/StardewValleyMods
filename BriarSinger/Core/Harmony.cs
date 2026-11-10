using BriarSinger.Framework;
using BriarSinger.Profession;
using BriarSinger.Spells;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
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
            // stops the normal defense block from happening
            harmony.Patch(
                original: AccessTools.Method(typeof(MeleeWeapon), "beginSpecialMove"),
                prefix: new HarmonyMethod(typeof(Harmony), nameof(beginSpecialMove_Prefix))
            );
            //adds the new xp method for the harpsword
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation),"onMonsterKilled"),
                postfix: new HarmonyMethod(typeof(MonsterKilledPatch), nameof(MonsterKilledPatch.Postfix))
                );
            //attempt to remove the red bar from the harpsword
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(MeleeWeapon), nameof(MeleeWeapon.drawInMenu)),
                prefix: new HarmonyMethod(typeof(Harmony), nameof(DrawInMenu_Prefix)),
                postfix: new HarmonyMethod(typeof(Harmony), nameof(DrawInMenu_Postfix)));
            //Yes I know I can do patchall, this makes it easier to exclude one for testing, I'll switch it when it's done
        }

        //stops the regular block and plays the PowerChord instead
        public static bool beginSpecialMove_Prefix(MeleeWeapon __instance, Farmer who)
        {
            if (__instance.ItemId != HarpSword.WeaponId)
                return true;

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
        [HarmonyPatch(typeof(GameLocation), "onMonsterKilled")]
        public static class MonsterKilledPatch
        {
            public static void Postfix(Farmer who, Monster monster)
            {
                if (who?.CurrentTool is not MeleeWeapon weapon) return;
                if (weapon.ItemId != "Teoshen.BriarSinger_HarpSword") return;

                int xp = Math.Max(1, monster.ExperienceGained / 2);
                BriarSingerSkill.AddEXP(who, xp);
            }
        }  
        
        //Stops the red bar from appearing on the harpsword defense cooldown
        public static void DrawInMenu_Prefix(MeleeWeapon __instance, out int __state)
        {
            __state = MeleeWeapon.defenseCooldown;
            if (__instance.ItemId == HarpSword.WeaponId) //TODO if this naming works then change the MonsterKilledPatch
                MeleeWeapon.defenseCooldown = 0;
        }
        public static void DrawInMenu_Postfix(int __state)
        {
            MeleeWeapon.defenseCooldown = __state; //TODO if this works, add power chord to the cooldowntoolbar
        }
    }
}
