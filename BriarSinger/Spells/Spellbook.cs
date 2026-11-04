using BriarSinger.Framework;
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
using BriarSinger.Spells;

namespace BriarSinger.Spells
{
    public static class Spellbook
    {
        // Bolt
        public static SpellData<Monster> BoltSpell { get; } = new SpellData<Monster>
        {
            Id = "Bolt",
            Description = "A flurry of leaves fly from your hand towards a target.",
            ManaCost = (caster) => 0.08f * caster.GetMaxMana(),
            CooldownSeconds = 2f,
            Range = 128,
            Targeting = new SingleMonsterTargeting(),
            Effect = (caster, target) =>
            {
                SpellCastAnim.HandCastAnimation(caster, caster.FacingDirection, () => SpawnBolt(caster, target));
            },
        };
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
        
        // Starshot
        public static SpellData<Vector2> StarShotSpell { get; } = new SpellData<Vector2>
        {
            Id = "Starshot",
            Description = "A collection of musical notes coalesce and seek nearby enemies.",
            ManaCost = (caster) => 0.06f * caster.GetMaxMana(),
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
        Description = "Vines erupt from the ground and seek nearby enemies to root them in place.",
            ManaCost = (caster) => 0.25f * caster.GetMaxMana(),
            CooldownSeconds = 8f,
            CastTimeSeconds = 0.25f,
            Range = 192,
            Targeting = new CircleAreaTargeting(),
            Effect = (caster, monsters) =>
            {
                GameLocation location = caster.currentLocation;

                //Have to recalc the area that you clicked the cast to know the spot to spawn the roots
                Vector2 center = Game1.currentCursorTile * 64f + new Vector2(32f, 32f);

                Roots.SpawnGreenPulse(location, center, RootsSpell.Range);

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
                        owner: caster));
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
        public static SpellData<Farmer> HealSpell { get; } = new SpellData<Farmer>
        {
            Id = "Heal",
            Description = "Forest magic heals your wounds.",
            ManaCost = (caster) => 0.5f * caster.GetMaxMana(),
            CooldownSeconds = 20f,
            CastTimeSeconds = 2.5f,
            Range = 0,
            Targeting = new SelfTargeting(),
            Effect = (caster, target) =>
            {
                target.health = Math.Min(target.maxHealth, target.health + 75);
            }
        };

        // GladeTeleport


        //The static thing that collects all the spelldata
        public static List<ISpellData> AllSpells = new() { RootsSpell, HealSpell }; //TODO add the other spells here as you make them, also Starshot is not included since it's tied to the weapon and Bolt is a separate key

        public static ISpellData ActiveSpell { get; set; } = RootsSpell;
    }
}
