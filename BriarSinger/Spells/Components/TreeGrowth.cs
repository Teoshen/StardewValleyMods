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
        puglic static bool TryGetGrowableTree(GameLocation location, Vector2 tile, out Tree tree)
        {
            tree = null;
            if (location.terrainFeatures.TryGetValue(tile, out var feature)
                && feature is tree t
                && !t.stump.Value
                && t.growthStage.Value < tree.treeStage)
            {
                tree = t;
                return true;
            }
            return false;
        }

        public static void SpawnJuminoDance(GameLocation location, Vector2 tile)
        {
            const float baseSize = 64f;

            var pulse = new TemporaryAnimatedSprite(
                "TileSheets\\animations", //TODO add animation
                new Rectangle(448, 256, 64, 64),
                80f, 1, 0,
                center - new Vector2(baseSize / 2f, baseSize / 2f),
                flicker: false,
                flipped: false)
            {
                color = Color.LightGreen * 0.6f,
                scale = 1f,
                scaleChange = (targetScale - 1f) / 20f, //should make this grow to full radius over 20 frames
                alphaFade = 0.04f
            }; //TODO remove this but it should work for placeholder stuff
            Game1.Multiplayer.broadcastSprites(location, pulse);
        }
    }
}