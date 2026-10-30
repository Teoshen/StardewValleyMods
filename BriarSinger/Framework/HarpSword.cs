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
                           DisplayName = "HarpSword",
                           Description = "Your harp, modified with forest magic to be a weapon.",
                           Type = 3,
                           SpriteIndex = 0,
                           Texture = "BriarSinger/weapons",
                           MinDamage = 15,
                           MaxDamage = 25,
                           Speed = 4,
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
    { }
       
    public new Item getOne()
    {
        return new HarpSword(); //add in the stuff for upgrades when that's implemented
    }
    protected override Item GetOneNew()
    {
        return new HarpSword();
    }

    public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
    {
        try
        {
            base.DoFunction(location, x, y, power, who);

            location.performToolAction(this, x / Game1.tileSize, y / Game1.tileSize); //figure out how far out this needs to ogo

        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Unexpected error:\n\n{ex}", StardewModdingAPI.LogLevel.Error);
        }
    }
    public override bool onRelease(GameLocation location, int x, int y, Farmer who) => false;

    public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Microsoft.Xna.Framework.Color color, bool drawShadow)
    {
        spriteBatch.Draw(
            texture: this.GetTexture(),
            position: location + new Vector2(32f, 32f),
            new Microsoft.Xna.Framework.Rectangle(96, 16, 16, 16),
            color: color * transparency,
            rotation: 0f,
            new Vector2(8f, 8f),
            scale: Game1.pixelZoom * scaleSize,
            effects: SpriteEffects.None,
            layerDepth);
    }

    //Here is the custom attack animation
    private static readonly int[] StrumFrames = { 98, 99, 100 }; 
    public static void PlayStrumAnimation(Farmer who)
    {
        Game1.playSound("miniharp_note");

        var frames = new FarmerSprite.AnimationFrame[StrumFrames.Length];
        for (int i = 0; i < StrumFrames.Length; i++)
        {
            frames[i] = new FarmerSprite.AnimationFrame(
                frame: StrumFrames[i],
                milliseconds: 80, //240 ms total, tweak this to make the swing faster/slower
                secondaryArm: false,
                flip: false);
        }
        who.FarmerSprite.animateOnce(frames);
    }

    public static void SpawnSoundWave(Farmer who, GameLocation location)
    {
        int direction = who.FacingDirection; // 0 up, 1 right, 2 down, 3 left
        Vector2 offset = DirectionToOffset(direction) * 64f;
        Vector2 spawnPosition = who.Position + offset;
        float rotation = DirectionToRotation(direction);

        //TODO swap the source rectangle and animation length with the real animation when I make it
        var soundWave = new TemporaryAnimatedSprite(
            textureName: "Tilesheets/animations",
            sourceRect: new Rectangle(0, 0, 64, 64), //placeholder
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


}
