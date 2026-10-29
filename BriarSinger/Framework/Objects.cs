using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriarSinger.Framework
{
    public static class Harp
    {
        public const string ObjectId = "Teoshen.BriarSinger_Harp";

        public static void EditObjectData(IDictionary<string, ObjectData> data)
        {
            data[ObjectId] = new ObjectData
            {
                Name = "Harp",
                DisplayName = "Mini Harp",
                Description = "It's your old mini harp. In good condition, but could use some tuning.",
                SpriteIndex = 0,
                Texture = "BriarSinger/Objects",
                Price = 200,
                Type = "Objects",
            };
        }

        public static void LoadTexture(AssetRequestedEventArgs e)
        {
            e.LoadFromModFile<Texture2D>("assets/farmer/harpobject.png", AssetLoadPriority.Medium);
        }   

    }
}
