using BriarSinger.Framework;
using BriarSinger.Framework;
using BriarSinger.Spells.Spell_Tools;
using Microsoft.Xna.Framework;
using SpaceShared.APIs;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SpaceCore.Skills;

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
            bool TryResolve(Farmer caster, int range, out T target);
        }

        public delegate CastResult SpellCheck<T>(Farmer caster, SpellData<T> spell, out Monster target);

        public static CastResult CheckMana<T>(Farmer caster, SpellData<T> spell)
        {
            int cost = (int)Math.Ceiling(spell.ManaCost(caster) ?? 0f);
            return ManaHelper.HasEnoughMana(caster, cost) ? CastResult.Success : CastResult.OutOfMana;
        }
        public static CastResult CheckCooldown<T>(Farmer caster, SpellData<T> spell) =>
           CooldownHelper.IsOnCooldown(caster, spell.Id) ? CastResult.OnCooldown : CastResult.Success;

        //this is the precheck so if you try to cast a spell you don't go through the casting animation and then get refused
        public static bool CanCast<T>(Farmer caster, SpellData<T> spell)
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
            return true;
        }

        //this is the check for when you are done with the cast animation and trying to push the effect
        public static bool TryCastSpell<T>(Farmer caster, SpellData<T> spell)
        {
            if (!CanCast(caster, spell)) return false; //checks for mana and cooldown again after the cast but before resolution

            if (!spell.Targeting.TryResolve(caster, spell.Range, out T target))
            {
                Game1.showRedMessage("Invalid target.");
                return false;
            }

            //makes the player turn in the direction of the spell being cast
            Vector2 toCursor = (Game1.currentCursorTile * 64f) - caster.Position;
            caster.faceDirection(VectorToFacingDirection(toCursor));

            //spends the mana cost, starts the cooldown, and casts the spell
            // manaToSpend exists because of the percentage based mana costs, so it makes a number I can compare on
            int manaToSpend = (int)Math.Ceiling(spell.ManaCost(caster) ?? 0f);
            ManaHelper.SpendMana(caster, manaToSpend);
            CooldownHelper.StartCooldown(caster, spell.Id, spell.CooldownSeconds);
            
            if (spell.CastTimeSeconds <= 0f)
               // constructor is Farmer caster, float castTimeSeconds, Action onComplete
                SpellCastAnim.Begin(caster, spell.CastTimeSeconds, () => spell.Effect(caster, target));
            else
                spell.Effect(caster, target);
            return true;
        }

        public static class ManaHelper
        {
            public static int GetMana(Farmer caster) => caster.GetMana();
            public static bool HasEnoughMana(Farmer caster, int cost) => caster.GetMana() >= cost;
            public static void SpendMana(Farmer caster, int cost) => caster.AddMana(-cost);
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

        public static int VectorToFacingDirection(Vector2 direction)
        {
            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
                return direction.X > 0 ? 1 : 3;
            else return direction.Y > 0 ? 2 : 0;
        }
    }
}
