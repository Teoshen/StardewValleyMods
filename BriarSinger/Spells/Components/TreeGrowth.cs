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
    public class TreeGrowth
    {
        public static bool TryGetGrowableTree(GameLocation location, Vector2 tile, out Tree growable)
        {
            growable = null;
            if (location.terrainFeatures.TryGetValue(tile, out var feature)
                && feature is Tree t
                && !t.stump.Value
                && t.growthStage.Value < Tree.treeStage)
            {
                growable = t;
                return true;
            }
            return false;
        }

        public static void SpawnJuminoDance(GameLocation location, Vector2 tile)
        {
            
        }
    }
}