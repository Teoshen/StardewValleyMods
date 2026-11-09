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
using Microsoft.Xna.Framework.Graphics;
using static BriarSinger.Framework.Extensions;
using System.Diagnostics;

namespace BriarSinger.Spells
{
    public static class Spellbook
    {
        // Bolt
        public static SpellData<Monster> BoltSpell { get; } = new SpellData<Monster>
        {
            Id = "Bolt",
            Description = "A flurry of leaves fly from your hand towards a target.", //TODO convert to i18n
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
        
        // PowerChord
        public static SpellData<List<Monster>> PowerChordSpell { get; } = new SpellData<List<Monster>>
        {
            Id = "PowerChord",
            Description = "A collection of musical notes coalesce and seek nearby enemies.", //TODO convert to i18n
            ManaCost = (caster) => 0.15f * caster.GetMaxMana(),
            CooldownSeconds = 6f,
            CastTimeSeconds = 1f,
            Range = 960,
            Targeting = new MultiMonsterTargeting(maxTargets: 4),
            Effect = (caster, targets) =>
            {

                GameLocation location = caster.currentLocation;
                Vector2 origin = caster.Position; //TODO tweak offsets

                for (int i = 0; i < 4; i++) //TODO adjust this to account for later upgrades
                {
                    bool hasTarget = targets.Count > 0;
                    Monster target = hasTarget ? targets[i % targets.Count] : null;

                    Vector2 dir = hasTarget
                    ? Utility.getVelocityTowardPoint(origin, target.Position, 10f)
                    : Utility.getVelocityTowardPoint(origin, origin + DirectionHelper.FacingDirectionToVector(caster.FacingDirection) * 64f, 10f);

                    //TODO make the projectiles do fancy stuff like hold for a second, then arc towards the targets instead of going straight
                    location.projectiles.Add(new PowerChord(
                        Damage: 25,
                        velocity: dir,
                        startingPosition: origin,
                        taillength: 8,
                        homingTarget: target,
                        isHoming: true,
                        location: location,
                        owner: caster,
                        homingTurnRate: 6f));
                }
            }
        };

        // Roots
        public static SpellData<List<Monster>> RootsSpell { get; } = new SpellData<List<Monster>>
        {
        Id = "Roots",
        Description = "Vines erupt from the ground and root nearby enemies in place.", //TODO convert to i18n
            ManaCost = (caster) => 0.25f * caster.GetMaxMana(),
            CooldownSeconds = 8f,
            CastTimeSeconds = 0.5f,
            Range = 192,
            Targeting = new CircleAreaTargeting(),
            Effect = (caster, monsters) =>
            {
                GameLocation location = caster.currentLocation;
                Vector2 center = Game1.currentCursorTile * 64f + new Vector2(32f, 32f);

                Roots.SpawnGreenPulse(location, center, RootsSpell.Range);

                foreach (Monster target in monsters)
                    Roots.RootEffect.Apply(target, 8f, location);
            }
        };

        // NatureBounty

        // Fishing

        // GrassGrowth

        // TreeGrowth

        // Rejuvenate
        public static SpellData<Farmer> RejuvenateSpell { get; } = new SpellData<Farmer>
        {
            Id = "Rejuvenate",
            Description = "Call to the forest to renew your spirit.", //TODO convert to i18n
            ManaCost = (caster) => 0.3f * caster.GetMaxMana(),
            CooldownSeconds = 180f,
            CastTimeSeconds = 2.5f,
            Range = 0,
            Targeting = new SelfTargeting(),
            Effect = (caster, target) =>
            {
                RegenEffects.StartStaminaRegen(target, amountPerSecond: 2, durationSeconds: 25);
            }
        };

        // TreeHarvest

        // Heal
        public static SpellData<Farmer> HealSpell { get; } = new SpellData<Farmer>
        {
            Id = "Heal",
            Description = "Forest magic heals your wounds.", //TODO convert to i18n
            ManaCost = (caster) => 0.5f * caster.GetMaxMana(),
            CooldownSeconds = 20f,
            CastTimeSeconds = 2.5f,
            Range = 0,
            Targeting = new SelfTargeting(),
            Effect = (caster, target) =>
            {
            target.health = Math.Min(target.maxHealth, target.health + 45);
                
            }
        };

        // GladeTeleport


        //The static thing that collects all the spelldata
        public static List<ISpellData> AllSpells = new() { RootsSpell, HealSpell, RejuvenateSpell }; //TODO add the other spells here as you make them, also PowerChord is not included since it's tied to the weapon and Bolt is a separate key

        public static ISpellData ActiveSpell { get; set; } = RootsSpell; //default active spell
    }
}
