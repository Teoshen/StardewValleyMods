using BriarSinger.Framework;
using BriarSinger.Profession;
using BriarSinger.Spells.Spell_Tools;
using Microsoft.Xna.Framework;
using SpaceShared.APIs;
using StardewModdingAPI.Utilities;
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
           CooldownHelper.IsOnCooldown(spell.Id) ? CastResult.OnCooldown : CastResult.Success;

        //this is the precheck so if you try to cast a spell you don't go through the casting animation and then get refused
        public static bool CanCast<T>(Farmer caster, SpellData<T> spell)
        {
            if (SpellValidation.CheckCooldown(caster, spell) != CastResult.Success)
            {
                Game1.showRedMessage($"{spell.Name} is on cooldown.");
                return false;
            }

            if (SpellValidation.CheckMana(caster, spell) != CastResult.Success)
            {
                Game1.showRedMessage("Not enough mana.");
                return false;
            }
            if (spell.MaxCastsPerDay > 0 && CooldownHelper.IsOnDailyCooldown(spell.Id, spell.MaxCastsPerDay))
            {
                Game1.showRedMessage($"No {spell.Name} casts left today.");
                return false;
            }
            return true;
        }

        //this is the check for when you are done with the cast animation and trying to push the effect
        public static bool TryCastSpell<T>(Farmer caster, SpellData<T> spell)
        {
            if (!CanCast(caster, spell)) return false; //checks for mana and cooldown again after the cast but before resolution

            if (!spell.Targeting.TryResolve(caster, spell.Range, out T target) || spell.IsValidTarget?.Invoke(caster, target) == false)
                //checks before and after cast for invalid target for tile targets
            {
                Game1.showRedMessage("Invalid target.");
                return false;
            }
            if (spell.MaxCastsPerDay > 0)
                CooldownHelper.MarkUsedToday(spell.Id);

            //makes the player turn in the direction of the spell being cast
            Vector2 toCursor = (Game1.currentCursorTile * 64f) - caster.Position;
            caster.faceDirection(VectorToFacingDirection(toCursor));

            //spends the mana cost, starts the cooldown, and casts the spell
            // manaToSpend exists because of the percentage based mana costs, so it makes a number I can compare on
            int manaToSpend = (int)Math.Ceiling(spell.ManaCost(caster) ?? 0f);
            ManaHelper.SpendMana(caster, manaToSpend);
            CooldownHelper.StartCooldown(spell.Id, spell.CooldownSeconds);

            void ResolveCast()
            {
                spell.Effect(caster, target);
                BriarSingerSkill.AddEXP(caster, spell.XpReward);
            }

            if (spell.CastTimeSeconds > 0f)
                SpellCastAnim.Begin(caster, spell.CastTimeSeconds, ResolveCast);
            else
                ResolveCast();
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
            public sealed class Entry
            {
                public string spellId;
                public float Remaining;
            }

            private static readonly PerScreen<List<Entry>> active = new(() => new List<Entry>());
            public static IReadOnlyList<Entry> Active => active.Value;

            public static void Tick(GameTime gameTime)
            {
                var list = active.Value;
                if (list.Count == 0) return;

                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

                for (int i = list.Count - 1; i >= 0; i--)
                {
                    list[i].Remaining -= elapsed;
                    if (list[i].Remaining <= 0f)
                        list.RemoveAt(i);
                }
            }

            public static void StartCooldown(string spellId, float seconds)
            {
                var list = active.Value;
                foreach (var e in list)
                {
                    if (e.spellId == spellId)
                    {
                        e.Remaining = seconds;
                        return;
                    }
                }
                list.Add(new Entry { spellId = spellId, Remaining = seconds });
                Monitor.Log($"Cooldown started: {spellId}, active count = {active.Value.Count}", LogLevel.Debug); //TODO remove after testing
            }

            public static bool IsOnCooldown(string spellId)
            {
                foreach (var e in active.Value)
                    if (e.spellId == spellId) return true;
                return false;
            }
            public static float GetRemainingSeconds(string spellId)
            {
                foreach (var e in active.Value)
                    if (e.spellId == spellId) return e.Remaining;
                return 0f;
            }

            //Cooldowns for X casts per day things
            private static readonly PerScreen<Dictionary<string, int>> dailyUses = new(() => new ());

            public static int DailyUses(string spellId) =>
                dailyUses.TryGetValue(spellId, out int n) ? n : 0;

            public static int DailyCastsRemaining(string spellId, int maxPerDay) =>
                Math.Max(0, maxPerDay - DailyUses(spellId));

            public static bool IsOnDailyCooldown(string spellId, int maxPerDay) =>
                DailyUses(spellId) >= maxPerDay;

            public static void MarkUsedToday(string spellId) =>
                dailyUses.Value[spellId] = DailyUses(spellId) + 1;
           
            public static void OnDayStarted() => dailyUses.Value.Clear();
        }

        public static int VectorToFacingDirection(Vector2 direction)
        {
            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
                return direction.X > 0 ? 1 : 3;
            else return direction.Y > 0 ? 2 : 0;
        }
    }
}
