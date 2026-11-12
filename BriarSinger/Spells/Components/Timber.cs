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
using BriarSinger.Core;

namespace BriarSinger.Spells.Components
{
    public class Timber
    {
        private static readonly string LastHarvestKey = $"{ModEntry.UniqueID}/LastTimberDay";
        private const int RegrowDays = 3;

        public static bool TryGetHarvestableTree(GameLocation location, Vector2 tile, out Tree tree)
        {
            tree = null;

            if (!location.terrainFeatures.TryGetValue(tile, out var feature) || feature is not Tree t)
                return false;

            if (t.stump.Value || t.growthStage.Value < Tree.treeStage)
                return false;

            if (t.modData.TryGetValue(LastHarvestKey, out string last)
                && int.TryParse(last, out int lastDay)
                && Game1.Date.TotalDays - lastDay < RegrowDays)
                return false;

            tree = t;
            return true;
        }

        public static void Harvest(GameLocation location, Tree tree, Vector2 tile)
        {
            tree.modData[LastHarvestKey] = Game1.Date.TotalDays.ToString();

            bool mahogany = tree.treeType.Value == Tree.mahoganyTree;
            float rate = mahogany ? 0.5f : 1f; //hardwood gets reduced drops for balance

            Vector2 origin = tile * 64f + new Vector2(32f, 0f);

            Drop(location, origin, "(O)388", Roll(6, 10, rate)); //wood
            Drop(location, origin, "(O)92", Roll(1, 2, rate)); //sap
            if (mahogany)
                Drop(location, origin, "(O)709", Roll(1, 2, rate)); //hardwood
        }

        private static int Roll(int min, int max, float rate) =>
            (int)Math.Round(Game1.random.Next(min, max + 1) * rate);

        private static void Drop(GameLocation location, Vector2 origin, string itemId, int count)
        {
            if (count <= 0) return;
            Item item = ItemRegistry.Create(itemId, count);
            Game1.createMultipleItemDebris(item, origin, -1, location);
        }
    }
}