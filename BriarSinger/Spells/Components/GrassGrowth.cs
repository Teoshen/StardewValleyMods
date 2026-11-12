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
        private class PendingGrass
        {
            public GameLocation location;
            public Vector2 tile;
            public float Delay;
        }
        
        private static readonly PerScreen<List<PendingGrass>> pending = new(() => new());
        
        public static bool CanGrowGrassAt(GameLocation location, Vector2 tile)
            {
                int x = (int)tile.X, y = (int)tile.Y;

                return !location.terrainFeatures.ContainsKey(tile) //exclude crops, trees, already existing grass
                    && !location.objects.ContainsKey(tile)
                    && location.isTileLocationOpen(new xTile.Dimensions.Location(x, y))
                    && location.doesTileHaveProperty(x, y, "Diggable", "Back") != null;
            }
        

        public static void SpawnGrass(GameLocation location, Vector2 center, IEnumerable<Vector2> tiles, float secondsPerTile = 0.25f)
        {
            foreach (Vector2 tile in tiles)
            {
                if (!CanGrowGrassAt(location, tile)) continue;                
                    pending.Value.Add(new PendingGrass
                    {
                        location = location,
                        tile = tile,
                        Delay = Vector2.Distance(tile, center) * secondsPerTile
                    });               
            }
        }

        public static void Tick(GameTime gameTime)
        {
            var list = pending.Value;
            if (list.Count == 0) return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //go backwards instead of forwards
            for (int i = list.Count -1; i >= 0; i--)
            {
                var p = list[i];
                p.Delay -= dt;
                if (p.Delay > 0f) continue;
                {
                    if (CanGrowGrassAt(p.location, p.tile))
                    {
                        p.location.terrainFeatures.Add(p.tile, new Grass(Grass.springGrass, 4));
                    }
                    list.RemoveAt(i);
                }
            }
        }

        public static void SpawnJuminoBurst(GameLocation location, Vector2 origin, List<Vector2> tiles)
        {
            //add jumino animation here centered on the caster
        }
    }
}