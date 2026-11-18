using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Weapons;
using StardewValley.Tools;
using System.Xml.Serialization;


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

    internal class HarpUpgrade
    {
        public string Id;
        public string Name;
        public string Description;
        public string FuseItemId; //qualified item id of weapons to fuse
        public Texture2D Icon;
    }
    internal static class HarpUpgrades
    {
        public static readonly List<HarpUpgrade> All = new()
        {
            //remixed rewards could work?
            //Rusty Sword tier: Rusty Sword only
            //Steel Smallsword tier: 1-2, Femur, Steel Smallsword, Wood Club, Elf Blade
            //Crystal Dagger tier: 3-4, Crystal Dagger, Cutlass, Iron Edge, Burglar's Shank, Wood Mallet
            //Obsidian Edge tier: 5-8, Obsidian Edge, Tempered Broadsword, Wicked Kris, Bone Sword, Ossified Blade
            //Galaxy Sword tier: Galaxy sword only
            new HarpUpgrade
            {
                Id = "Rusty Sword",
                Name = "Create HarpSword",
                Description = "Fusing the sword to the harp creates a new weapon", //TODO add this to the getonenew or whatever
                FuseItemId = "(W)0"
            }, //2-5dam, crit 2%
            new HarpUpgrade
            {
                Id = "Steel Smallsword",
                Name = "Adds damage",
                Description = "The HarpSword gains 8-10 damage", //TODO add this to the weapon stats
                FuseItemId = "(W)11"
            }, //4-8 dam, crit 2%, +2 speed
            new HarpUpgrade
            {
                Id = "Crystal Dagger",
                Name = "Adds crit power",
                Description = "The HarpSword gains +3 weight and +40 crit power", //TODO add this to the weapon stats
                FuseItemId = "(W)21"
            }, //4-10 dam, crit 3%, +5 weight, +50 crit power
            new HarpUpgrade
            {
                Id = "Obsidian Edge",
                Name = "Adds damage",
                Description = "The HarpSword gains 10-18 additional damage.", //TODO add this to the weapon stats
                FuseItemId = "(W)8"
            }, //30-45 dam, crit 2%, -1 speed, +10 crit power
            new HarpUpgrade
            {
                Id = "Galaxy Sword",
                Name = "Adds damage",
                Description = "The Harpsword gains 45-56 additional damage.", //TODO add this to the weapon stats
                FuseItemId = "(W)4"
            }, //60-80 dam, 2% crit, +4 speed

            new HarpUpgrade
            {
                Id = "Slingshot1",
                Name = "Unlock PowerChord",
                Description = "The HarpSword gains the PowerChord spell.", //TODO add the requirement for this
                FuseItemId = "(W)32"
            },
            new HarpUpgrade
            {
                Id = "Slingshot2",
                Name = "PowerChord Upgrade 1",
                Description = "PowerChord gains an additional projectile.", //TODO add the +proj for this
                FuseItemId = "(W)33"
            },
            new HarpUpgrade
            {
                Id = "Slingshot3",
                Name = "PowerChord Upgrade 2",
                Description = "PowerChord gains an additional projectile.", //TODO add the +proj for this
                FuseItemId = "(W)34"
            },
        };

        private static string Key(string id) => "${Modentry.UniqueId}/Upgrade/{id}"; //stores in the save data

        public static bool Has(Farmer who, string id) => who.modData.ContainsKey(Key(id));
        public static void Apply(Farmer who, string id) => who.modData[Key(id)] = "1";

        public static HarpUpgrade FindFor(Item fuseItem) =>
            fuseItem == null ? null : All.FirstOrDefault(u => u.FuseItemId == fuseItem.QualifiedItemId);

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
}
   
 


    
    
    
    



