using BriarSinger.Framework;
using BriarSinger.Spells.Spell_Tools;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BriarSinger.Spells
{
    public static class SpellValidation
    {
        public enum CastResult
        {
            Success, 
            OutOfMana, 
            OnCooldown, 
            NoValidTarget
        }

        public interface ITargetingStrategy<T>
        {
            CastResult TryResolve(Farmer caster, int range, out T target);
        }

        public delegate CastResult SpellCheck<T>(Farmer caster, SpellData<T> spell, out Monster target);

        public static CastResult CheckMana<T>(Farmer caster, SpellData<T> spell) =>
           ManaHelper.GetMana(caster) >= spell.ManaCost ? CastResult.Success : CastResult.OutOfMana;

        public static CastResult CheckCooldown<T>(Farmer caster, SpellData<T> spell) =>
           CooldownHelper.IsOnCooldown(caster, spell.Id) ? CastResult.OnCooldown : CastResult.Success;


        public static bool TryCastSpell<T>(Farmer caster, SpellData<T> spell)
        {
            if (SpellValidation.CheckCooldown(caster, spell) != CastResult.Success)
            {
                Game1.showRedMessage($"{spell.Id} is on cooldown.");
                return false;
            }
            if (SpellValidation.CheckMana(caster, spell) != CastResult.Success)
            {
                Game1.showRedMessage("Not enough mana.");
                return false;
            }
            if (spell.Targeting.TryResolve(caster, spell.Range, out T target) != CastResult.Success)
            {
                Game1.showRedMessage("Invalid target.");
                return false;
            }

            //makes the player turn in the direction of the spell being cast
            Vector2 toCursor = (Game1.currentCursorTile * 64f) - caster.Position;
            caster.faceDirection(VectorToFacingDirection(toCursor));

            //spends the mana cost, starts the cooldown, and casts the spell
            ManaHelper.SpendMana(caster, spell.ManaCost);
            CooldownHelper.StartCooldown(caster, spell.Id, spell.CooldownSeconds);
            spell.Effect(caster, target);
            return true;
        }

        public static class ManaHelper
        {
            public static int GetMana(Farmer caster) => caster.GetCurrentMana();
            public static bool HasEnoughMana(Farmer caster, int cost) => caster.GetCurrentMana() >= cost;
            public static void SpendMana(Farmer caster, int cost) => caster.addMana(-cost);
        }

        public static class CooldownHelper
        {
            private static readonly Dictionary<string, float> cooldowns = new();

            public static void Tick(GameTime gameTime)
            {
                var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                var keys = cooldowns.Keys.ToList();

                foreach (var key in keys)
                {
                    cooldowns[key] -= elapsed;
                    if (cooldowns[key] <= 0f)
                        cooldowns.Remove(key);
                }
            }
            public static bool IsOnCooldown(Farmer caster, string spellId) => cooldowns.ContainsKey(MakeKey(caster, spellId));

            public static void StartCooldown(Farmer caster, string spellId, float seconds) => cooldowns[MakeKey(caster, spellId)] = seconds;

            private static string MakeKey(Farmer caster, string spellId) => $"{caster.UniqueMultiplayerID}_{spellId}";

                //Cooldowns for once per day things
                private static readonly HashSet<string> usedToday = new();

            public static bool IsOnDailyCooldown(Farmer caster, string spellId) => usedToday.Contains(MakeKey(caster,spellId));

            public static void MarkUsedToday(Farmer caster, string spellId) => usedToday.Add(MakeKey(caster,spellId));

            public static void OnDayStarted() => usedToday.Clear();
            // make sure to add a bit on the ondaystarted event in modentry to run this part TODO
        }
    }
}
