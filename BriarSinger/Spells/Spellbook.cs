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
using StardewValley.TerrainFeatures;

namespace BriarSinger.Spells
{
    // TODO add class level and skill choice requirements
    public static class Spellbook
    {
        // LeafBarrage
        public static SpellData<Monster> LBarrageSpell { get; } = new SpellData<Monster>
        {
            Id = "LBarrage",
            Name = "Leaf Barrage", //TODO i18n
            Description = "A flurry of leaves fly from your hand towards a target.", //TODO convert to i18n
            ManaCost = (caster) => 0.08f * caster.GetMaxMana(),
            CooldownSeconds = 2f,
            XpReward = 4,
            Range = 128,
            Targeting = new SingleMonsterTargeting(),
            Effect = (caster, target) =>
            {
                SpellCastAnim.HandCastAnimation(caster, caster.FacingDirection, () => SpawnLBarrage(caster, target));
            },
        };
            private static void SpawnLBarrage(Farmer caster, Monster target)
            {
                int damage = 15; //TODO fix damage to scale with briarsinger level
                Vector2 startingPosition = caster.getStandingPosition();
                Vector2 toTarget = target.GetBoundingBox().Center.ToVector2() - startingPosition;
                Vector2 direction = toTarget == Vector2.Zero ? new Vector2(0, -1) : Vector2.Normalize(toTarget);
                Vector2 velocity = direction * 15f;

                caster.currentLocation.projectiles.Add(new LBarrage(
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
            Name = "Power Chord", //TODO i18n
            Description = "A collection of musical notes coalesce and seek nearby enemies.", //TODO convert to i18n
            ManaCost = (caster) => 0.15f * caster.GetMaxMana(),
            CooldownSeconds = 6f, //TODO adjust and add a bool for if the player has that one combat perk
            XpReward = 8,
            CastTimeSeconds = 1f,
            Range = 960,
            Targeting = new MultiMonsterTargeting(maxTargets: 4),
            Effect = (caster, targets) =>
            {

                GameLocation location = caster.currentLocation;
                int count = 4; //TODO: replace with upgrade values (2 base, 1 from slingshot, 1 from master slingshot, 2 from Skald, 1 from galaxy slingshot)
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
                        damage: 25, //TODO scale the damage with the slingshots sacrificed to it plus briarsinger level
                        startingPosition: caster.Position + hoverOffset,
                        taillength: 3,
                        location: location,
                        owner: caster,
                        targets: targets,
                        slot: i, //the note's preferred target
                        hoverOffset: hoverOffset,
                        releaseAt: holdSeconds + i * stagger,
                        arcSide: i % 2 == 0 ? 1 : -1, //each note flips its arc direction for dramatic effect
                        homingTurnRate: 6f));
                }                
            }
        };

        // Roots
        public static SpellData<List<Monster>> RootsSpell { get; } = new SpellData<List<Monster>>
        {
        Id = "Roots",
        Name = "Roots", //TODO i18n
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
            //TODO add this

        // Fishing
            //TODO add this

        // Shockwave
            //TODO finish this
            {
            Id = "Shockwave",
            Name = "Shockwave", // TODO i18n
            Description = "Razor sharp leaves explode outwards from you.", //TODO i18n
            ManaCost = (caster) => 0.25f * caster.GetMaxMana(),
            CooldownSeconds = 15f,
            XpReward = 10,
            CastTimeSeconds = 1f,
            Range = 128,
            Targeting = new PlayerCenteredArea(),            
            Effect = (caster, monsters) =>
                {

                }
            }

        // GrassGrowth
        public static SpellData<List<Vector2>> GrassGrowthSpell { get; } = new SpellData<List<Vector2>>
        {
            Id = "GrassGrowth",
            Name = "Grass Growth", // TODO i18n
            Description = "Carpet the ground around you in grass.", //TODO i18n
            ManaCost = (caster) => 0.2f * caster.GetMaxMana(),
            CooldownSeconds = 30f,
            XpReward = 8,
            CastTimeSeconds = 1.5f,
            Range = 128,
            Targeting = new PlayerCenteredTiles(),
            InvalidTargetMessage = "There's nowhere to grow grass here.", //TODO i18n
            IsValidTarget = (caster, tiles) =>
                tiles.Any(t => GrassGrowth.CanGrowGrassAt(caster.currentLocation, t)),
            Effect = (caster, tiles) =>
            {
                GameLocation location = caster.currentLocation;

                Vector2 center = new Vector2(tiles.Average(t => t.X), tiles.Average(t => t.Y));

                GrassGrowth.SpawnJuminoBurst(location, caster.Position, tiles);
                GrassGrowth.SpawnGrass(location, center, tiles);
            }
        };

        // TreeGrowth
        public static SpellData<Vector2> TreeGrowthSpell { get; } = new SpellData<Vector2>
        {
            Id = "TreeGrowth",
            Name = "Tree Growth", //TODO i18n
            Description = "Juminos bring a forest tree one step closer to maturity", //TODO i18n
            ManaCost = (caster) => 0.4f * caster.GetMaxMana(),
            CooldownSeconds = 8f,
            MaxCastsPerDay = 3, //TODO change this to match the profession +casts per day
            XpReward = 12,
            CastTimeSeconds = 1.5f,
            Range = 320,
            Targeting = new GroundTileTargeting(requireOpen: false),
            InvalidTargetMessage = "Target a tree that can still grow.", //don't know if I should or can add switch cases for other types of invalid targets //TODO i18n
            IsValidTarget = (caster, tile) => TreeGrowth.TryGetGrowableTree(caster.currentLocation, tile, out _),
            Effect = (caster, tile) =>
            {
                GameLocation location = caster.currentLocation;
                if (!TreeGrowth.TryGetGrowableTree(location, tile, out Tree tree))
                    return;

                tree.growthStage.Value++;
                TreeGrowth.SpawnJuminoDance(location, tile);
            }
        };

        // FruitTreeGrowth
        public static SpellData<Vector2> FruitTreeGrowthSpell { get; } = new SpellData<Vector2>
        {
            Id = "FruitTreeGrowth",
            Name = "Fruit Tree Growth",
            Description = "Juminos bring a fruit tree closer to maturation", //TODO i18n
            ManaCost = (caster) => 0.4f * caster.GetMaxMana(),
            CooldownSeconds = 8f,
            MaxCastsPerDay = 2, //TODO change this to match the profession +casts per day
            XpReward = 12,
            CastTimeSeconds = 1.5f,
            Range = 320,
            Targeting = new GroundTileTargeting(requireOpen: false),
            InvalidTargetMessage = "Target a tree that can still grow.", //TODO i18n
            IsValidTarget = (caster, tile) => FruitTreeGrowth.TryGetGrowableFruitTree(caster.currentLocation, tile, out _),
            Effect = (caster, tile) =>
            {
                GameLocation location = caster.currentLocation;
                if (!FruitTreeGrowth.TryGetGrowableFruitTree(location, tile, out FruitTree tree))
                    return;
                FruitTreeGrowth.AdvanceFruitTree(tree);
                TreeGrowth.SpawnJuminoDance(location, tile);
            }
        };

        // Rejuvenate
        public static SpellData<Farmer> RejuvenateSpell { get; } = new SpellData<Farmer>
        {
            Id = "Rejuvenate",
            Name = "Rejuvenate", //TODO convert to i18n
            Description = "Call to the forest to renew your spirit.", //TODO convert to i18n
            ManaCost = (caster) => 0.3f * caster.GetMaxMana(),
            CooldownSeconds = 180f,
            XpReward = 15,
            CastTimeSeconds = 2.5f,
            Range = 0,
            Targeting = new SelfTargeting(),
            Effect = (caster, target) =>
            {
                StaminaRegenEffects.StartStaminaRegen(target, amountPerSecond: 2, durationSeconds: 25);
                //TODO: add a little sprite on the player like a yellow fizzing for the duration
            }
        };

        // Timber
        public static SpellData<Vector2> TimberSpell { get; } = new SpellData<Vector2>
        {
            Id = "Timber",
            Name = "Timber", //TODO convert to i18n
            Description = "Harvest a tree without felling it.", //TODO i18n
            ManaCost = (caster) => 0.4f * caster.GetMaxMana(),
            CooldownSeconds = 15f,
            MaxCastsPerDay = 5,
            XpReward = 6,
            CastTimeSeconds = 1.5f,
            Range = 128,
            Targeting = new GroundTileTargeting(requireOpen: false),
            InvalidTargetMessage = "Target a mature tree that hasn't been harvested recently.", //TODO i18n
            IsValidTarget = (caster, tile) =>
            Timber.TryGetHarvestableTree(caster.currentLocation, tile, out _),
            Effect = (caster, tile) =>
            {
                GameLocation location = caster.currentLocation;

                //check if the tree is no longer a valid target
                if (!Timber.TryGetHarvestableTree(location, tile, out Tree tree))
                    return;

                Timber.Harvest(location, tree, tile);
            }
        };

        // Heal
        public static SpellData<Farmer> HealSpell { get; } = new SpellData<Farmer>
        {
            Id = "Heal",
            Name = "Heal", //TODO convert to i18n
            Description = "Forest magic heals your wounds.", //TODO convert to i18n
            ManaCost = (caster) => 0.5f * caster.GetMaxMana(),
            CooldownSeconds = 20f,
            XpReward = 10,
            CastTimeSeconds = 2f,
            Range = 0,
            Targeting = new SelfTargeting(),
            Effect = (caster, target) =>
            {
            target.health = Math.Min(target.maxHealth, target.health + 35);     
            HealthRegenEffects.StartHealthRegen(target, amountPerSecond: 2, durationSeconds: 10);                
            //TODO add a little sprite on the player that has the green fizzle during the duration
            }
        };

        // GladeTeleport
            // TODO add this


        //The static thing that collects all the spelldata
        public static List<ISpellData> AllSpells = new() { RootsSpell, HealSpell, TimberSpell, RejuvenateSpell, GrassGrowthSpell, FruitTreeGrowthSpell, TreeGrowthSpell }; //TODO add the other spells here as I make them. PowerChordSpell is on the weapon and LeafBarrage is its own key so they are intentionally not included.

        public static ISpellData ActiveSpell { get; set; } = RootsSpell; //default active spell
    }
}
