using System;
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
using StardewValley.Tools;
using static BriarSinger.Spells.Spell_Tools.Targeting;

namespace BriarSinger.Spells
{
    public class PowerChord : Projectile
    { 
    private enum Phase { Hold, Fly }
    private Phase phase = Phase.Hold;
    private float timer; //seconds since spawning
    private float flyTimer; //seconds since launch
    private float retargetTimer; //saves compute on scanning for new target

    private Monster homingTarget;
    private readonly List<Monster> targets; //target list
    private readonly int slot; //the note's preferred target, as a slot fills the other notes will prefer other targets until none are available
    private readonly Vector2 hoverOffset;
    private readonly float releaseAt;
    private readonly nint arcSide; //this is just +1 -1 so the notes curl in opposite directions when launched
    private readonly float speed; //launch speed, not proj speed
    private readonly float homingTurnRate;
    private readonly Farmer caster;
    private readonly GameLocation loc;

    private const float CurlDegrees = 70f; //off-axis launch start
    private const int ReScanRange = 960; //how far the notes looks for a new target if their original one is gone

        private Texture2D TailTexture;
        private Texture2D ProjectileTexture;

    public readonly int damage;

    public PowerChord() { }

    public PowerChord(int damage, Vector2 startingPosition, int taillength, GameLocation location, 
        Farmer owner, List<Monster> targets, int slot, Vector2 hoverOffset, float releaseAt, int arcSide, 
        float speed = 10f, float homingTurnRate = 6f)
        : this()
    {
        this.damage = damage;
        this.damagesMonsters.Value = true;
        base.theOneWhoFiredMe.Set(location, owner);
        base.tailLength.Value = taillength;
        base.position.Value = startingPosition;
        base.ignoreObjectCollisions.Value = true; //TODO think about this

        this.loc = location;
        this.caster = owner;
        this.targets = targets;
        this.slot = slot;
        this.hoverOffset = hoverOffset;
        this.releaseAt = releaseAt;
        this.arcSide = arcSide;
        this.speed = speed;
        this.homingTurnRate = homingTurnRate;
    }

    //homing logic
    public override void updatePosition(GameTime time)
    {
        if (caster == null)
        {
            position.X += xVelocity.Value;
            position.Y += yVelocity.Value;
            return; //hopefully will sync on multiplayer?
        }

        float dt = (float)time.ElapsedGameTime.TotalSeconds;
        timer += dt;

        if (phase == Phase.Hold) //proj sticks to the caster until releaseAt
        {
            //hover next to caster with a bob
            float bob = (float)Math.Sin(timer * 8f + slot) * 3f;
            position.Value = caster.Position + hoverOffset + new Vector2(0f, bob);

            if (timer >= releaseAt)
                Launch();
            return;
        }

        flyTimer += dt;
        {
            retargetTimer -= dt;
            bool rescan = retargetTimer <= 0f;
            homingTarget = PickTarget(allowRescan: rescan);
            if (rescan) retargetTimer = 0.25f; //scan for new target every 0.25 seconds
            }

        if (homingTarget != null)
        {
            //turn rate ramps as the note gets closer
            float turn = homingTurnRate * (1f + flyTimer * 4f);
            Vector2 v = Homing.SteerToward(
                new Vector2(xVelocity.Value, yVelocity.Value),
                position.Value,
                homingTarget.GetBoundingBox().Center.ToVector2(),
                turn, dt);
            xVelocity.Value = v.X;
            yVelocity.Value = v.Y;
        }

        position.X += xVelocity.Value;
        position.Y += yVelocity.Value;
    }

    private void Launch()
    {
        phase = Phase.Fly;
        homingTarget = PickTarget(allowRescan: true);

        Vector2 aim = homingTarget != null
            ? homingTarget.GetBoundingBox().Center.ToVector2() - position.Value //homes towards a target
            : DirectionHelper.FacingDirectionToVector(caster.FacingDirection); //fallback to shoot at the caster's direction
        aim.Normalize();

        //starts rotated away from target and steers back towards
        float curl = homingTarget != null ? MathHelper.ToRadians(CurlDegrees) * arcSide : 0f;
        float c = (float)Math.Cos(curl), s = (float)Math.Sin(curl);
        Vector2 v = new Vector2(aim.X * c - aim.Y * s, aim.X * s + aim.Y * c) * speed; //2D rotation matrix
        xVelocity.Value = v.X;
        yVelocity.Value = v.Y;
    }

    private bool IsLive(Monster m) =>
        m != null && m.Health > 0 && m.currentLocation == loc; //maybe overkill

    private Monster PickTarget(bool allowRescan)
    {
        //start with original picks and cycle to find one
        if (targets != null)
        {
            for (int k = 0; k < targets.Count; k++)
            {
                var m = targets[(slot + k) % targets.Count];
                if (IsLive(m)) return m;
            }
        }
        //if all original picks are gone, note looks for closest target to it
        if (!allowRescan) return null;
        return MultiMonsterTargeting.FindTargets(loc, position.Value, ReScanRange, 1).FirstOrDefault();
    }

        public override void draw(SpriteBatch b)
        {
            float projectile_scale = 2f * this.localScale;

            var angle = GetAngleForTexture(base.xVelocity.Value, base.yVelocity.Value);

            ProjectileTexture = Game1.content.Load<Texture2D>("BriarSinger/MyProjectile2"); //TODO clean this up when spritesheet is finalized
            TailTexture = Game1.content.Load<Texture2D>("BriarSinger/MyProjectile3");

            float tail_scale = 1.67f * this.localScale;
            var tailoffset = TailOffset(new Vector2(32f, 32f), base.xVelocity.Value, base.yVelocity.Value);

            b.Draw(
                ProjectileTexture,
                Game1.GlobalToLocal(Game1.viewport, this.position.Value + new Vector2(0f, 0f - this.height.Value) + new Vector2(32f, 32f)),
                new Rectangle(0, 0, 24, 24),
                this.color.Value,
                angle,
                new Vector2(8f, 8f),
                projectile_scale,
                SpriteEffects.None,
                (this.position.Value.Y + 96f) / 100000f
                );

            //Draws the tail to the game world, centered on the projectile and with each tail slowly shrinking in size.
            for (int i = base.tail.Count - 1; i >= 0; i--)
            {
                b.Draw(
                    TailTexture,
                    Game1.GlobalToLocal(Game1.viewport, Vector2.Lerp((i == base.tail.Count - 1) ? this.position.Value : base.tail.ElementAt(i + 1), base.tail.ElementAt(i), (float)base.tailCounter / 60f) + new Vector2(0f, 0f - this.height.Value) + tailoffset),
                    new Rectangle(0, 0, 24, 24),
                    this.color.Value,
                    angle,
                    new Vector2(8f, 8f),
                    tail_scale,
                    SpriteEffects.None,
                    (this.position.Y - (float)(base.tail.Count - i) + 96f) / 10000f);
            }

            if (this.height.Value > 0f)
            {
                b.Draw(Game1.shadowTexture,
                    Game1.GlobalToLocal(Game1.viewport, this.position.Value + new Vector2(32f, 32f)),
                    Game1.shadowTexture.Bounds,
                    Color.White * 0.75f, 0f,
                    new Vector2(Game1.shadowTexture.Bounds.Center.X,
                    Game1.shadowTexture.Bounds.Center.Y), 2f,
                    SpriteEffects.None,
                    (this.position.Y - 1f) / 10000f);
            }
        }

        public override void behaviorOnCollisionWithTerrainFeature(TerrainFeature t, Vector2 tileLocation, GameLocation location)
        {
            t.performUseAction(tileLocation);
            this.explosionAnimation(location);
        }

        public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
        {
            location.playSound("hitEnemy");
            Farmer player = this.GetPlayerWhoFiredMe(location);
            this.explosionAnimation(location);
            if (n is Monster)
            {
                location.damageMonster(n.GetBoundingBox(), this.damage, this.damage, isBomb: false, player, isProjectile: true);
            }
            this.destroyMe = true;
        }
        private void explosionAnimation(GameLocation location)
        {
            Multiplayer multiplayer = Game1.Multiplayer;
            multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(128, 256, 64, 64), 60, 6, 1, base.position.Value, flicker: true, flipped: false));
            base.destroyMe = true; //TODO add an animation that fits a music note
            this.destroyMe = true;
            location.projectiles.RemoveWhere((Func<Projectile, bool>)(projectile =>
            {
                return projectile.destroyMe;
            }));
        }

        public override void behaviorOnCollisionWithPlayer(GameLocation location, Farmer player)
        {
            if ((bool)base.damagesMonsters.Value)
            {
                return;
            }
            this.explosionAnimation(location);
        }
        public override void behaviorOnCollisionWithOther(GameLocation location)
        {
            if (base.ignoreObjectCollisions.Value == false)
            {
                this.explosionAnimation(location);
            }
        }
       
        public static float GetAngleForTexture(float xVelocity, float yVelocity)
        {
            return (float)Math.Atan2(xVelocity, yVelocity);
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

        public virtual Farmer GetPlayerWhoFiredMe(GameLocation location)
        {
            return (base.theOneWhoFiredMe.Get(location) as Farmer) ?? Game1.player;
        }
    }
}
    