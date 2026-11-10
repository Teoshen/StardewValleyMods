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
    public class FruitTreeGrowth
    {
        public static bool TryGetGrowableFruitTree(GameLocation location, Vector2 tile, out FruitTree tree)
        {
            tree = null;
            if (location.terrainFeatures.TryGetValue(tile, out var feature)
                && feature is FruitTree t
                && !t.stump.Value
                && t.growthStage.Value < FruitTree.treeStage)
            {
                tree = t;
                return true;
            }
            return false;
        }
        public static void AdvanceFruitTree(FruitTree tree)
        {
            //TODO add this. Fruit tree is flat days instead of stages so maybe it takes 6 days off the maturation day
        }
    }

}