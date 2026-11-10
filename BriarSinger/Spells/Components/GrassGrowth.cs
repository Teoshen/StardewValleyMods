using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.TerrainFeatures;
using StardewValley.Util;

using BriarSinger.Framework;
using System.Runtime.InteropServices;

namespace BriarSinger.Spells.Components
{
    public static class GrassGrowth
    {
        public static bool CanGrowGrassAt(GameLocation location, Vector2 tile)
        {
            int x = (int)tile.X, y = (int)tile.Y;

            return !location.terrainFeatures.ContainsKey(tile) //exclude crops, trees, already existing grass
                && !location.objects.ContainsKey(tile)
                && location.isTileLocationOpen(new xTile.Dimensions.Location(x, y))
                && location.doesTileHaveProperty(x, y, "Diggable", "Back") != null; //TODO double check grass decomp, I just guessed here, also maybe add "must be outside"
        }

        public static void SpawnJuminoBurst(GameLocation location, Vector2 origin, List<Vector2> tiles)
        {
            //add jumino animation here centered on the caster
        }
    }
}