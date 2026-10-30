using BriarSinger.Spells.Components;
using BriarSinger.Spells.Spell_Tools;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BriarSinger.Spells.Spell_Tools.Targeting;

namespace BriarSinger.Spells
{
    public static class Spellbook
    {
        private static Monster target;

        public static ISpellData ActiveSpell { get; set; } = BoltSpell;


        // Bolt
        public static SpellData<Monster> BoltSpell { get; } = new SpellData<Monster>
               {
            Id = "Bolt",
            ManaCost = 10,
            CooldownSeconds = 2f,
            Range = 128,
            Targeting = new SingleMonsterTargeting(),
            Effect = (caster, target) =>
            {
                int damage = 25;
                caster.currentLocation.projectiles.Add(new Bolt(
                    damage, 
                    0, 
                    10, 
                    target.getStandingPosition() + new Vector2(-16, -256), 
                    4, 
                    target,
                    true,
                    caster.currentLocation, 
                    caster));
            }
        };

        // Starshot
        public static SpellData<Vector2> StarShotSpell { get; } = new SpellData<Vector2>
                {
            Id = "Starshot",
            ManaCost = 15,
            CooldownSeconds = 2f,
            Range = 0,
            Targeting = new CursorDirection(),
            Effect = (caster, direction) =>
            {
                int damage = 25;
                Vector2 velocity = direction * 15;
                caster.currentLocation.projectiles.Add(new StarShot(
                    damage, 
                    velocity, 
                    caster.getStandingPosition() + new Vector2(0, -64), 
                    6, 
                    target,
                    true,
                    caster.currentLocation, 
                    caster));
            }
        };

        // Roots

        // NatureBounty

        // Fishing

        // GrassGrowth

        // TreeGrowth

        // Rejuvenate

        // TreeHarvest

        // Heal
    }
}
