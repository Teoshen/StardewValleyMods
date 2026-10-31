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
                Vector2 startingPosition = caster.getStandingPosition();
                Vector2 toTarget = target.GetBoundingBox().Center.ToVector2() - startingPosition;
                Vector2 direction = toTarget == Vector2.Zero ? new Vector2(0, -1) : Vector2.Normalize(toTarget);
                Vector2 velocity = direction * 15f;

                caster.currentLocation.projectiles.Add(new Bolt(
                    damage, 
                    10, 
                    10, 
                    caster.getStandingPosition(), 
                    4, 
                    target,
                    true,
                    caster.currentLocation, 
                    caster,
                    homingTurnRate: 20f));
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

                const int homingAquireRadius = 320; // 5 tiles
                bool isHoming = new SingleMonsterTargeting().TryResolve(caster, homingAquireRadius, out Monster homingTarget);
                caster.currentLocation.projectiles.Add(new StarShot(
                    damage, 
                    velocity, 
                    caster.getStandingPosition() + new Vector2(0, -64), 
                    6, 
                    isHoming ? homingTarget : null,
                    isHoming,
                    caster.currentLocation, 
                    caster,
                    homingTurnRate: 8f));
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
