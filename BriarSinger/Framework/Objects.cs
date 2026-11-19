using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Objects;
using StardewValley.Menus;

namespace BriarSinger.Framework
{
    public class Objects
    {
        //TODO add to CP and remove from here
        public const string HarpId = "Teoshen.BriarSinger_Harp";
        public const string SeedId = "Teoshen.BriarSinger_JuniperSeed";
        public const string QualifiedSeedId = "(O)" + SeedId;

        public static void EditObjectData(IDictionary<string, ObjectData> data)
        {
            data[HarpId] = new ObjectData
            {
                Name = HarpId,
                DisplayName = "Mini Harp",
                Description = "It's your old mini harp. In good condition, but could use some tuning.",
                SpriteIndex = 0,
                Texture = "BriarSinger/Objects",
                Price = 200,
                Type = "Objects",
            };

            data[SeedId] = new ObjectData
            {
                Name = SeedId,
                DisplayName = "Juniper-Moss Seed",
                Description = "This large seed yearns to be planted.",
                SpriteIndex = 0, //TODO fix when this is done
                Texture = "BriarSinger/Objects",
                Price = 0,
                Type = "Objects",
            };
        }
        // TODO make it so you can play the harp before it becomes the harpsword
        // TODO add a mechanism for the galaxy slingshot (probably a quest)

        public static void LoadTexture(AssetRequestedEventArgs e)
        {
            e.LoadFromModFile<Texture2D>("assets/farmer/harpobject.png", AssetLoadPriority.Medium);
        }
    }
}
