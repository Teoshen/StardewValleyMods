using StardewValley;
using StardewValley.TerrainFeatures;

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