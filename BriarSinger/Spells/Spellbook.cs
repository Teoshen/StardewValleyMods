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
        public static List<ISpellData> AllSpells = new() { BoltSpell, StarShotSpell, RootsSpell } //TODO add the other spells here as you make them
        private static Monster target;

        public static ISpellData ActiveSpell { get; set; } = BoltSpell;


        // Bolt
        public static SpellData<Monster> BoltSpell { get; } = new SpellData<Monster>
        {
            Id = "Bolt",
            ManaCost = 0.08,
            CooldownSeconds = 2f,
            Range = 128,
            Targeting = new SingleMonsterTargeting(),
            Effect = (caster, target) =>
            {
                SpellCastAnim.HandCastAnimation(caster, caster.FacingDirection, () => SpawnBolt(caster, target));
            }
            private static void SpawnBolt(Farmer caster, Monster target)
            {
                int damage = 15;
                Vector2 startingPosition = caster.getStandingPosition();
                Vector2 toTarget = target.GetBoundingBox().Center.ToVector2() - startingPosition;
                Vector2 direction = toTarget == Vector2.Zero ? new Vector2(0, -1) : Vector2.Normalize(toTarget);
                Vector2 velocity = direction * 15f;

                caster.currentLocation.projectiles.Add(new Bolt(
                    damage,
                    velocity.X,
                    velocity.Y,
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
            ManaCost = 0.06,
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
        public static SpellData<List<Monster>> RootsSpell { get; } = new SpellData<List<Monster>>
        {
        Id = "Roots",
            ManaCost = 0.25,
            CooldownSeconds = 8f,
            Range = 192,
            Targeting = new CircleAreaTargeting(),
            Effect = (caster, monsters) =>
            {
                GameLocation location = caster.currentLocation;

                //Have to recalc the area that you clicked the cast to know the spot to spawn the roots
                Vector2 center = Game1.currentCursorTile * 64f + new Vector2(32f, 32f);

                SpawnGreenPulse(location, center, Spellbook.RootsSpell.Range);

                foreach (Monster target in monsters)
                {
                    Vector2 toTarget = target.GetBoundingBox().Center.ToVector2() - center;
                    Vector2 direction = toTarget == Vector2.Zero ? new Vector2(0, -1) : Vector2.Normalize(toTarget);
                    Vector2 velocity = direction * 8f;

                    location.projectiles.Add(new Roots(
                        damage: 1,
                        velocity: velocity,
                        startingPosition: center,
                        taillength: 5,
                        homingTarget: target,
                        isHoming: true,
                        location: location,
                        owner: caster));)
                }
            }
        };
    

        // NatureBounty

        // Fishing

        // GrassGrowth

        // TreeGrowth

        // Rejuvenate

        // TreeHarvest

        // Heal

        // GladeTeleport
    }
}
