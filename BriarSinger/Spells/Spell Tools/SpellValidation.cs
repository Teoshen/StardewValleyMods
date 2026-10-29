using BriarSinger.Framework;
using BriarSinger.Spells.Spell_Tools;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static CastResult CheckTarget<T>(Farmer caster, SpellData<T> spell, out Monster target)
            {
            target = MonsterHelper.GetClosestMonsterToCursor(spell.Range);
            return target == null ? CastResult.NoValidTarget : CastResult.Success;
            }



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

            ManaHelper.SpendMana(caster, spell.ManaCost);
            CooldownHelper.StartCooldown(caster, spell.Id, spell.CooldownSeconds);
            spell.Effect(caster, target);
            return true;
        }

    }
}
