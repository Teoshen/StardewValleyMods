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
    public class Roots
    {
        public static Texture2D RootsTexture { get; set; }

        public static void SpawnGreenPulse(GameLocation location, Vector2 center, float radiusPixels)
        {
            const float baseSize = 64f;

            float targetScale = radiusPixels / baseSize;
            const int frames = 30; //half second
            float scaleStep = targetScale / frames;

            var pulse = new TemporaryAnimatedSprite(
                "TileSheets\\animations", //TODO add custom
                new Rectangle(448, 256, 64, 64),
                100f,
                1,
                2,
                center - new Vector2(baseSize / 2f, baseSize / 2f),
                flicker: false,
                flipped: false,
                layerDepth: (center.Y + 40f) / 10000f,
                alphaFade: 0.015f,
                color: Color.LightGreen * 0.6f,
                scale: 0.4f,
                scaleChange: scaleStep,
                rotation: 0f,
                rotationChange: 0f);
            Game1.Multiplayer.broadcastSprites(location, pulse);
        }

        public static class RootEffect
        {
            private class RootData
            {
                public float SecondsRemaining;
                public Vector2 LastPosition;
                public TemporaryAnimatedSprite RootSprite;
                public GameLocation Location;
            }

            private static readonly Dictionary<Monster, RootData> rooted = new();

            public static void Apply(Monster target, float durationSeconds, GameLocation location)
            {
                if (rooted.TryGetValue(target, out RootData existing))
                {
                    existing.SecondsRemaining = durationSeconds; //refresh duration without double spawning a new sprite
                    return;
                }
                var vine = new TemporaryAnimatedSprite(
                    "TileSheets\\animations", //TODO add animation
                     new Rectangle(0, 0, 64, 64),
                     100f, 4, 999, //loops for a long time because I will remove it manually
                                   target.Position,
                                   flicker: false,
                                   flipped: false)
                {
                    color = Color.White,
                    scale = 2f,
                    layerDepth = (target.Position.Y + 64f) / 10000f
                };
                Game1.Multiplayer.broadcastSprites(location, vine);

                rooted[target] = new RootData
                {
                    SecondsRemaining = durationSeconds,
                    LastPosition = target.Position,
                    RootSprite = vine,
                    Location = location
                };
            }
            public static void Tick(GameTime time)
            {
                if (rooted.Count == 0)
                    return;

                var toRemove = new List<Monster>();
                foreach (var kvp in rooted.ToList())
                {
                    Monster m = kvp.Key;
                    RootData data = kvp.Value;

                    if (m == null || m.Health <= 0 || m.currentLocation == null)
                    {
                        toRemove.Add(m);
                        continue;
                    }
                    //cancel the monster's movement
                    Vector2 movedBy = m.Position - data.LastPosition;
                    if (movedBy != Vector2.Zero)
                        m.Position -= movedBy;

                    data.LastPosition = m.Position; //pinned
                    data.RootSprite.Position = m.Position; //keep the sprite on the monster

                    data.SecondsRemaining -= (float)time.ElapsedGameTime.TotalSeconds;
                    if (data.SecondsRemaining <= 0f)
                    {
                        toRemove.Add(m); //add the monster to the list of monsters to remove
                    }
                }
                foreach (Monster m in toRemove)
                {
                    if (rooted.TryGetValue(m, out RootData data))
                        data.Location.temporarySprites.Remove(data.RootSprite);
                    rooted.Remove(m); // delete the sprite and remove the monster from the list of rooted monsters
                }
            }

        }
    }
}
