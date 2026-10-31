using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore;
using SpaceCore.VanillaAssetExpansion;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Weapons;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BriarSinger;
using System.Diagnostics;
using static BriarSinger.Spells.Spell_Tools.Targeting;
using static BriarSinger.Spells.SpellValidation;
using StardewValley.Monsters;


namespace BriarSinger.Framework;

[XmlType("Mods_Teoshen_Harpsword")]

public class HarpSword : MeleeWeapon
{
    public const string WeaponId = "Teoshen.BriarSinger_HarpSword";
    public static Texture2D Texture { get; private set; }
    public static void LoadTexture(IModHelper helper)
    {
        Texture = helper.ModContent.Load<Texture2D>("assets/farmer/harpsword.png");
    }
    public virtual Texture2D GetTexture() => Texture;

    private static readonly FacingArcArea SwingTargeting = new();
    private static readonly PlayerCenteredArea PowerChordTargeting = new();
    public static void EditWeaponData(IDictionary<string, WeaponData> data)
         {
            data[WeaponId] = new WeaponData
            {
                           Name = "HarpSword",
                           DisplayName = "HarpSword",
                           Description = "Your harp, modified with forest magic to be a weapon.",
                           Type = 3,
                           SpriteIndex = 0,
                           Texture = "BriarSinger/weapons",
                           MinDamage = 15,
                           MaxDamage = 25,
                           Speed = 2,
                           CanBeLostOnDeath = false,
                           AreaOfEffect = 2,
                           CritChance = 0.04f,
                           CritMultiplier = 3.5f,
                           Precision = 10
            };
         }

    public static void LoadTexture(AssetRequestedEventArgs e)
     {
          e.LoadFromModFile<Texture2D>("assets/farmer/harpsword.png", AssetLoadPriority.Medium);
      }



    public HarpSword() : base(WeaponId)
    {
        Debug.WriteLine("HarpSword() constructed");
    }

    public HarpSword(string itemId) : base(itemId)
    {
        Debug.WriteLine($"HarpSword(itemId) constructed: {itemId}");
    }

    public new Item getOne()
    {
        return new HarpSword(); //add in the stuff for upgrades when that's implemented
    }
    protected override Item GetOneNew()
    {
        return new HarpSword();
    }


    public static class StrumLock
    {
        private static float secondsRemaining;
        private static bool previousCanMove;

        public static void Lock(Farmer who, float durationSeconds)
        {
            previousCanMove = who.CanMove;
            secondsRemaining = durationSeconds;

            who.CanMove = false;
            who.UsingTool = true;
            who.Halt();
        }

        public static void Tick(Farmer who, GameTime gameTime)
        {
            if (secondsRemaining <= 0f)
                return;

            secondsRemaining -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            who.CanMove = false;
            who.UsingTool = true;
            who.Halt();

            if (secondsRemaining <= 0f)
            {
                secondsRemaining = 0f;
                who.CanMove = previousCanMove;
                who.UsingTool = false;
            }
        }
    }
    //regular attack swing
    public static void PerformDirectionalSwing(Farmer who)
    {
        if (CooldownHelper.IsOnCooldown(who, DirectionalSwingCooldownId))
            return;

        CooldownHelper.StartCooldown(
            who,
            DirectionalSwingCooldownId,
            DirectionalSwingCooldownSeconds);

        PlayStrumAnimation(who);
        PlayStrumSound();
        SpawnSoundWave(who, who.currentLocation);

        int swingRangePixels = (int)(Game1.tileSize * 2.5f);
        if (SwingTargeting.TryResolve(who, swingRangePixels, out List<Monster> targets))
        {
            foreach (var monster in targets)
            {
                who.currentLocation.damageMonster(
                    monster.GetBoundingBox(),
                    minDamage: 15,
                    maxDamage: 25,
                    isBomb: false,
                    who: who);
            }
        }
    }

       //this is the power chord part
    public static void PerformPowerChord(Farmer who)
    {
        if (CooldownHelper.IsOnCooldown(who, PowerChordCooldownId))
            return;

        CooldownHelper.StartCooldown(
            who,
            PowerChordCooldownId,
            PowerChordCooldownSeconds);

        PlayStrumAnimation(who);
        PlayPowerChordSounds();

        foreach (int direction in new[] { 0, 1, 2, 3 }) // up, right, down, left
        {
            SpawnSoundWave(who, who.currentLocation, direction);
        }

        float radiusTiles = 2f;
        int radiusPixels = (int)(radiusTiles * Game1.tileSize);

        if (PowerChordTargeting.TryResolve(who, radiusPixels, out List<Monster> targets))
        {
            foreach (var monster in targets)
            {
                who.currentLocation.damageMonster(
                    monster.GetBoundingBox(),
                    minDamage: 15,
                    maxDamage: 25,
                    isBomb: false,
                    knockBackModifier: 2.5f,
                    addedPrecision: 0,
                    critChance: 0f,
                    critMultiplier: 0f,
                    triggerMonsterInvincibleTimer: true,
                    who: who);
            }
        }
    }


    public override bool onRelease(GameLocation location, int x, int y, Farmer who) => false;

   
    //Here is the custom attack animation
    private static readonly int[] StrumFrames = { 98, 99, 100 }; 
    private const float StrumAnimationDurationSeconds = 0.24f;
    private const float DirectionalSwingCooldownSeconds = 0.24f;
    private const float PowerChordCooldownSeconds = 2f;

    private const string DirectionalSwingCooldownId =
        "HarpSword_DirectionalSwingCooldown";

    private const string PowerChordCooldownId =
        "HarpSword_PowerChordCooldown";

public static void PlayStrumAnimation(Farmer who)
{
    StrumLock.Lock(who, StrumAnimationDurationSeconds);

    var frames = new FarmerSprite.AnimationFrame[StrumFrames.Length];

    for (int i = 0; i < StrumFrames.Length; i++)
    {
        frames[i] = new FarmerSprite.AnimationFrame(
            frame: StrumFrames[i],
            milliseconds: 80,
            secondaryArm: false,
            flip: false);
    }

    who.FarmerSprite.animateOnce(frames);
}

    public static void SpawnSoundWave(Farmer who, GameLocation location)
    {
        SpawnSoundWave(who, location, who.FacingDirection);
    }
    
    private static void SpawnSoundWave(Farmer who, GameLocation location, int direction)
    {
        Vector2 offset = DirectionToOffset(direction) * 64f;
        Vector2 spawnPosition = who.Position + offset;
        float rotation = DirectionToRotation(direction);

        //TODO swap the source rectangle and animation length with the real animation when I make it
        var soundWave = new TemporaryAnimatedSprite(
            textureName: "Tilesheets/animations",
            sourceRect: new Rectangle(0, 0, 64, 64),
            animationInterval: 70f,
            animationLength: 6,
            numberOfLoops: 0,
            position: spawnPosition,
            flicker: false,
            flipped: false)
        {
            rotation = rotation,    
            layerDepth = 1f
        };

        Game1.Multiplayer.broadcastSprites(location, soundWave);
    }
    private static Vector2 DirectionToOffset(int facingDirection) => facingDirection switch
    {
        0 => new Vector2(0, -1),
        1 => new Vector2(1, 0),
        2 => new Vector2(0, 1),
        3 => new Vector2(-1, 0),
        _ => Vector2.Zero
    };

    private static float DirectionToRotation(int facingDirection) => facingDirection switch
    {
        0 => -(float)Math.PI / 2,
        1 => 0f,
        2 => (float)Math.PI / 2,
        3 => (float)Math.PI,
        _ => 0f
    };

    private static void PlayStrumSound()
    {
        Game1.playSound("miniharp_note");
    }

    private static void PlayPowerChordSounds()
    {
        const int soundOffsetMilliseconds = 75;

        for (int index = 0; index < 4; index++)
        {
            int delayMilliseconds = index * soundOffsetMilliseconds;

            DelayedAction.functionAfterDelay(
                () => Game1.playSound("miniharp_note"),
                delayMilliseconds);
        }
    }


}
