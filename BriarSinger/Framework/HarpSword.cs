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
            who.Halt();
        }
    }

    //Here is the custom attack animation
    private static readonly int[] StrumFrames = { 98, 99, 100 };

    public static void PlayStrumAnimation(Farmer who)
    {
        var frames = new FarmerSprite.AnimationFrame[StrumFrames.Length];

        for (int i = 0; i < StrumFrames.Length; i++)
        {
            frames[i] = new FarmerSprite.AnimationFrame(
                frame: StrumFrames[i],
                milliseconds: 120,
                secondaryArm: false,
                flip: false);
        }

        who.FarmerSprite.animateOnce(frames);
    }

   /* const int soundOffsetMilliseconds = 75;

    private static void PlayStrumSound() //TODO: replace with the regular harp sounds when I got them.
    {
        for (int index = 0; index < 2; index++)
        {
            int delayMilliseconds = index * soundOffsetMilliseconds;
            DelayedAction.functionAfterDelay(
                () => Game1.playSound("miniharp_note"),
                delayMilliseconds);
        }
    }

    private static void PlayPowerChordSounds()
    {

        for (int index = 0; index < 4; index++)
        {
            int delayMilliseconds = index * soundOffsetMilliseconds;

            DelayedAction.functionAfterDelay(
                () => Game1.playSound("miniharp_note"),
                delayMilliseconds);
        }
    }
   */
}
   
 


    
    
    
    



