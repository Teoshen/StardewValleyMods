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
            Name = "Bolt",
            Description = "A flurry of leaves fly from your hand towards a target.", //TODO convert to i18n
            ManaCost = (caster) => 0.08f * caster.GetMaxMana(),
            CooldownSeconds = 2f,
            XpReward = 4,
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
            Name = "Power Chord",
            Description = "A collection of musical notes coalesce and seek nearby enemies.", //TODO convert to i18n
            ManaCost = (caster) => 0.15f * caster.GetMaxMana(),
            CooldownSeconds = 6f,
            XpReward = 8,
            CastTimeSeconds = 1f,
            Range = 960,
            Targeting = new MultiMonsterTargeting(maxTargets: 4),
            Effect = (caster, targets) =>
            {

                GameLocation location = caster.currentLocation;
                int count = 4; //TODO: replace with upgrade values (2 base, 2 from slingshot, 2 from master slingshot, 2 from Herald, 2 from galaxy slingshot)
                float holdSeconds = 0.35f; //projectiles will hold still for a second before going for their target
                float stagger = 0.08f; //notes launch in sequence for dramatic effect

                for (int i = 0; i < count; i++)
                {
                    //notes go over the casters head before launching
                    float t = count == 1 ? 0.5f : i / (float)(count - 1);
                    float angle = MathHelper.ToRadians(-60f + 120f * t);
                    Vector2 hoverOffset = new Vector2((float)Math.Sin(angle), -(float)Math.Cos(angle)) * 72f
                                            + new Vector2(32f, 0f); //TODO tweak offsets

                    location.projectiles.Add(new PowerChord(
                        Damage: 25,
                        startingPosition: caster.Position + hoverOffset,
                        location: location,
                        owner: caster,
                        targets: targets,
                        slot: i, //the note's preferred target
                        hoverOffset: hoverOffset,
                        releaseAt: holdSeconds + i * stagger,
                        arcSide: i % 2 == 0 ? 1 : -1, //each note should flip its arc direction for dramatic effect
                        homingTurnRate: 6f));
                }                
            }
        };

        // Roots
        public static SpellData<List<Monster>> RootsSpell { get; } = new SpellData<List<Monster>>
        {
        Id = "Roots",
        Name = "Roots",
            // Icon = Game1.content.Load<Texture2D>("BriarSinger/Assets/Spells/rootsicon"),
            Description = "Vines erupt from the ground and root nearby enemies in place.", //TODO convert to i18n
            ManaCost = (caster) => 0.25f * caster.GetMaxMana(),
            CooldownSeconds = 8f,
            XpReward = 8,
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
            Name = "Rejuvenate",
            Description = "Call to the forest to renew your spirit.", //TODO convert to i18n
            ManaCost = (caster) => 0.3f * caster.GetMaxMana(),
            CooldownSeconds = 180f,
            XpReward = 15,
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
            Name = "Heal",
            //     Icon = Game1.content.Load<Texture2D>("BriarSinger/Assets/Spells/healicon"),
            Description = "Forest magic heals your wounds.", //TODO convert to i18n
            ManaCost = (caster) => 0.5f * caster.GetMaxMana(),
            CooldownSeconds = 20f,
            XpReward = 10,
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
