using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Monsters;

namespace BriarSinger.Spells.Components
{
    public class Shockwave
    {
        private const int LeafCount = 14; // number of sprites to draw below

        public static void Damage(Farmer caster, List<Monster> monsters, int damage, float knockback)
        {
            GameLocation location = caster.currentLocation;

            foreach(Monster m in monsters)
            {
                location.damageMonster(m.GetBoundingBox(), damage, damage, false, knockback, 0, 0f, 1f, false, caster);
            }
        }

        public static void SpawnLeaves(GameLocation location, Vector2 origin, float radiusPixels)
        {
            for (int i = 0; i < LeafCount; i++)
            {
                float angle = MathHelper.TwoPi * i / LeafCount;
                Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                var leaf = new TemporaryAnimatedSprite(
                    "TileSheets\\animations", //TODO add custom
                new Rectangle(448, 256, 64, 64),
                300f, 1, 1, //0.3s lifespan
                origin - new Vector2(32f, 32f), //top-left
                false, false,
                1f, //layer depth
                0.02f, //fade
                Color.White, 4f, 0f,
                angle, //points out
                0.2f); //slow spin

                // Reach the edge of the hit radius in 18 ticks (360ms abouts) 
                leaf.motion = dir * (radiusPixels / 18f);

                Game1.Multiplayer.broadcastSprites(location, leaf);
            }
        }
    }
}