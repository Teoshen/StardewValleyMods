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
    public class Roots : Projectile
    {
        public delegate void OnCollisionBehavior(GameLocation location, int xPosition, int yPosition, Character who);
        private bool homing;
        private Monster homingTarget;
        private readonly float homingTurnRate;
        public readonly int damage;

        public static Texture2D ProjectileTexture { get; set; }
        public static Texture2D tail;

        public Roots() { }

        public Roots(int damage, Vector2 velocity, Vector2 startingPosition, int taillength, Monster homingTarget, bool isHoming, GameLocation location, Character owner, float homingTurnRate = 25f)
            : this()

        {
            this.damage = damage;
            base.damagesMonsters.Value = damage > 0;
            this.damagesMonsters.Value = damage > 0;
            base.theOneWhoFiredMe.Set(location, owner);
            base.tailLength.Value = taillength;
            xVelocity.Value = velocity.X;
            yVelocity.Value = velocity.Y;
            base.position.Value = startingPosition;
            base.ignoreObjectCollisions.Value = false;

            this.homingTarget = homingTarget;
            this.homing = isHoming && homingTarget != null;
            this.homingTurnRate = homingTurnRate;
        }

        public override void updatePosition(GameTime time)
        {
            if (homing)
            {
                if (homingTarget == null || homingTarget.Health <= 0 || homingTarget.currentLocation == null)
                    homing = false;
                else
                {
                    Vector2 newVelocity = Homing.SteerToward(
                        new Vector2(xVelocity.Value, yVelocity.Value),
                        position.Value,
                        homingTarget.GetBoundingBox().Center.ToVector2(),
                        homingTurnRate,
                        (float)time.ElapsedGameTime.TotalSeconds);
                    xVelocity.Value = newVelocity.X;
                    yVelocity.Value = newVelocity.Y;
                }
            }
            position.X += xVelocity.Value;
            position.Y += yVelocity.Value;
        }

        public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
        {
            if (n is Monster m)
            {
                if (damage > 0)
                    location.damageMonster(m.GetBoundingBox(), damage, damage, false, 1f, 0, 0f, 0f, false, GetPlayerWhoFiredMe(location), isProjectile: true);

                RootEffect.Apply(m, 8f);
            }
            location.playSound("leafrustle"); //TODO find a good sound
            this.destroyMe = true;
        }

        public override void behaviorOnCollisionWithTerrainFeature(TerrainFeature t, Vector2 tileLocation, GameLocation location) => this.destroyMe = true;
        public override void behaviorOnCollisionWithOther(GameLocation location) => this.destroyMe = true;

        public override void draw(SpriteBatch b)
        {
            var angle = GetAngleForTexture(base.xVelocity.Value, base.yVelocity.Value);
            float projectile_scale = 2f * this.localScale;
            float tail_scale = 1.67f * this.localScale;
            var tailoffset = TailOffset(new Vector2(32f, 32f), base.xVelocity.Value, base.yVelocity.Value);

            //Draws the main projectile to the game world.
            b.Draw(
                ProjectileTexture,
                Game1.GlobalToLocal(Game1.viewport, this.position.Value + new Vector2(0f, 0f - this.height.Value) + new Vector2(32f, 32f)),
                    new Rectangle(0, 0, 24, 24),
                    this.color.Value,
                    angle,
                    new Vector2(8f, 8f),
                    projectile_scale,
                    SpriteEffects.None,
                    (this.position.Value.Y + 96f) / 100000f);

            //Draws the tail to the game world, centered on the projectile and with each tail slowly shrinking in size.
            for (int i = base.tail.Count - 1; i >= 0; i--)
            {
                b.Draw(
                    ProjectileTexture,
                    Game1.GlobalToLocal(Game1.viewport, Vector2.Lerp((i == base.tail.Count - 1) ? this.position.Value : base.tail.ElementAt(i + 1), base.tail.ElementAt(i), (float)base.tailCounter / 60f) + new Vector2(0f, 0f - this.height.Value) + tailoffset),
                    new Rectangle(0, 0, 24, 24),
                    this.color.Value,
                    angle,
                    new Vector2(8f, 8f),
                    tail_scale,
                    SpriteEffects.None,
                    (this.position.Y - (float)(base.tail.Count - i) + 96f) / 10000f);
            }
        }

        public static void SpawnGreenPulse(GameLocation location, Vector2 center, float radiusPixels)
        {
            const float baseSize = 64f;
            float targetScale = (radiusPixels * 2f);

            var pulse = new TemporaryAnimatedSprite(
                "TileSheets\\animations", //TODO add animation
                new Rectangle(0, 0, 64, 64),
                80f, 1, 0,
                center - new Vector2(baseSize / 2f, baseSize / 2f),
                flicker: false,
                flipped: false)
            {
                color = Color.LightGreen * 0.6f,
                scale = 1f,
                scaleChange = (targetScale - 1f) / 20f, //should make this grow to full radius over 20 frames
                alphaFade = 0.04f
            };
            Game1.Multiplayer.broadcastSprites(location, pulse);
        }
        public static float GetAngleForTexture(float xVelocity, float yVelocity)
        {
            return (float)Math.Atan2(xVelocity, yVelocity);
        }

        private void explosionAnimation(GameLocation location)
        {
            Multiplayer multiplayer = Game1.Multiplayer;
            multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(64, 640, 64, 64), 60, 7, 1, base.position.Value, flicker: false, flipped: false)); //TODO update this to match the roots
            this.destroyMe = true;
            location.projectiles.RemoveWhere((Func<Projectile, bool>)(projectile =>
            {
                return projectile.destroyMe;
            }));
        }
        public virtual Farmer GetPlayerWhoFiredMe(GameLocation location)
        {
            return (base.theOneWhoFiredMe.Get(location) as Farmer) ?? Game1.player;
        }
        public override void behaviorOnCollisionWithPlayer(GameLocation location, Farmer player)
        {
            if ((bool)base.damagesMonsters.Value)
            {
                return;
            }
            this.explosionAnimation(location);
        }

        public static Vector2 TailOffset(Vector2 basevector, float xVelocity, float yVelocity)
        {
            if (yVelocity < 0f)
            {
                basevector.Y += 19f;
            }
            else if (yVelocity > 0f)
            {
                basevector.Y -= 19f;
            }

            if (xVelocity < 0f)
            {
                basevector.X += 19f;
                basevector.Y += 4f;
            }
            else if (xVelocity > 0f)
            {
                basevector.X -= 19f;
                basevector.Y += 4f;
            }
            return basevector;
        }
        public static class RootEffect
        {
            private static readonly Dictionary<Monster, float> secondsRemaining = new();
            private static readonly Dictionary<Monster, int> originalSpeed = new();

            public static void Apply(Monster target, float durationSeconds)
            {
                if (!originalSpeed.ContainsKey(target))
                    originalSpeed[target] = target.speed;

                target.speed = 0;
                secondsRemaining[target] = durationSeconds; //allows for refreshing if they are already rooted
            }

            public static void Tick(GameTime time)
            {
                if (secondsRemaining.Count == 0) return;

                var toRemove = new List<Monster>();
                foreach (var kvp in secondsRemaining.ToList())
                {
                    Monster m = kvp.Key;
                    float remaining = kvp.Value - (float)time.ElapsedGameTime.TotalSeconds;

                    if (m == null || m.Health <= 0 || remaining <= 0f)
                    {
                        if (m != null && originalSpeed.TryGetValue(m, out int speed))
                            m.speed = speed;
                        toRemove.Add(m);
                    }
                    else
                    {
                        secondsRemaining[m] = remaining;
                    }
                }
                foreach (var m in toRemove)
                {
                    secondsRemaining.Remove(m);
                    originalSpeed.Remove(m);
                }
            }
        }
    }
}
