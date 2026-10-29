using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BriarSinger.Spells.SpellValidation;

namespace BriarSinger.Spells.Spell_Tools
{
    public interface ISpellData
        {
            string Id { get; }
            bool TryCast(Farmer caster);
        }
     
        public class SpellData<T> : ISpellData
        {
            public string Id { get; set; }
            public int ManaCost { get; set; }
            public float CooldownSeconds { get; set; }
        public float CastTimeSeconds { get; set; } = 0f; //0 is instant
            public int Range { get; set; }
            public ITargetingStrategy<T> Targeting { get; set; }
            public Action<Farmer, T> Effect { get; set; }

            public bool TryCast(Farmer caster) => SpellValidation.TryCastSpell(caster, this);
        }


    }
