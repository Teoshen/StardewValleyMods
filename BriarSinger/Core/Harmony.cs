using BriarSinger.Framework;
using BriarSinger.Profession;
using BriarSinger.Spells;
using HarmonyLib;
using Microsoft.Xna.Framework;
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
using static BriarSinger.Spells.SpellValidation;

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
                original: AccessTools.Method(typeof(GameLocation), "onMonsterKilled"),
                postfix: new HarmonyMethod(typeof(MonsterKilledPatch), nameof(MonsterKilledPatch.Postfix))
                );
            //attempt to remove the red bar from the harpsword
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(MeleeWeapon), nameof(MeleeWeapon.drawInMenu)),
                prefix: new HarmonyMethod(typeof(Harmony), nameof(DrawInMenu_Prefix)),
                postfix: new HarmonyMethod(typeof(Harmony), nameof(DrawInMenu_Postfix)));
            //stop the player from moving while casting
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(Farmer), "MovePosition"),
                prefix: new HarmonyMethod(typeof(Harmony), nameof(MovePosition_Prefix))
            );
            //stop the player from attacking while casting
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.pressUseToolButton)),
                prefix: new HarmonyMethod(typeof(Harmony), nameof(PressUseToolButton_Prefix))
            );
            //stop the animation for the special attack
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(MeleeWeapon), "animateSpecialMove"),
                prefix: new HarmonyMethod(typeof(Harmony), nameof(AnimateSpecialMove_Prefix))
                );
            //Yes I know I can do patchall, this makes it easier to exclude one for testing, I'll switch it when it's done
        }

        //stops the regular block and plays the PowerChord instead
        public static bool beginSpecialMove_Prefix(MeleeWeapon __instance, Farmer who)
        {
            if (__instance.ItemId != HarpSword.WeaponId)
                return true;

            if (ModEntry.helper.Input.IsDown(SButton.LeftShift))
                return false;

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

        //Harpsword gets PowerChord's cooldown isntead of vanilla block cooldown
        private const float VanillaDefenseBarMax = 1500f;
        public static void DrawInMenu_Prefix(MeleeWeapon __instance, out int __state)
        {
            __state = MeleeWeapon.defenseCooldown;
            if (__instance.ItemId != HarpSword.WeaponId)
                return;
            else
            {
                var spell = Spellbook.PowerChordSpell;
                float remaining = CooldownHelper.GetRemainingSeconds(spell.Id);
                float fraction = MathHelper.Clamp(remaining / spell.CooldownSeconds, 0f, 1f);

                MeleeWeapon.defenseCooldown = (int)(fraction * VanillaDefenseBarMax);
                //TODO add a part to suppress the regular sword action while the thing is on cooldown, just a flat return on the defense weapon part
                //TODO add the tictic sound when the cooldown is over
            }
        }
        public static void DrawInMenu_Postfix(int __state)
        {
            MeleeWeapon.defenseCooldown = __state; //put the real value back
        }

        //stop the harp from swinging around while casting
        public static bool MovePosition_Prefix(Farmer __instance) => !SpellCastAnim.IsCasting(__instance);

        //stop the player from using tools while casting
        public static bool PressUseToolButton_Prefix(ref bool __result)
        {
            if (!SpellCastAnim.IsCasting(Game1.player))
                return true;
            __result = false;
            return false;
        }

        //stop the right click from doing stuff while harpsword is on cd
        public static bool AnimateSpecialMove_Prefix(MeleeWeapon __instance, Farmer who)
        {
            if (__instance.ItemId != HarpSword.WeaponId)
                return true;

            bool blocked = CooldownHelper.IsOnCooldown(Spellbook.PowerChordSpell.Id)
                || SpellCastAnim.IsCasting(who);
            return !blocked; //false means skip
        }
    }
} 
       
