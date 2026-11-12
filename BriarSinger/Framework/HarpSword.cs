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

    public static void EditWeaponData(IDictionary<string, WeaponData> data)
    {
        data[WeaponId] = new WeaponData
        {
            Name = "HarpSword",
            DisplayName = "HarpSword", //TODO add i18n
            Description = "Your harp, modified with forest magic to be a weapon.", //TODO add i18n
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
    }

    public HarpSword(string itemId) : base(itemId)
    {
    }

    public new Item getOne()
    {
        return new HarpSword(); //TODO add in the stuff for upgrades when that's implemented
    }
    protected override Item GetOneNew()
    {
        return new HarpSword();
    }

    // TODO add a forge enchantment
    // TODO add mechanism for an infinity harp
    

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
   
 


    
    
    
    



