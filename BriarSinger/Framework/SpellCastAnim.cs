using BriarSinger;
using BriarSinger.Framework;
using BriarSinger.Spells;
using BriarSinger.Spells.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;

public static class SpellCastAnim
{
	private class ActiveCast
	{
		public Farmer Caster;
		public float RemainingMs;
		public float StrumTimerMs;
		public Action OnComplete;
	}

	private static ActiveCast active;

	//casting via the harp
	public static void Begin(Farmer caster, float castTimeSeconds, Action onComplete)
	{
		active = new ActiveCast
		{
			Caster = caster,
			RemainingMs = castTimeSeconds * 1000f,
			StrumTimerMs = 0f,
			OnComplete = onComplete,

		};
		caster.Halt();
		caster.UsingTool = true; //lock in place while casting //TODO find a better way to hold the player still
        PlayStrumAnimation(caster);
	}

    public static class StrumLock
    {
        //keeps the player from moving while they are casting
        private static float secondsRemaining;
        private static bool previousCanMove;

        public static void Lock(Farmer who, float durationSeconds)
        {
            previousCanMove = who.CanMove;
            secondsRemaining = durationSeconds;

            who.CanMove = false;
            who.Halt();
        }
    }

    //Here is the cast animation
    private static readonly int[] StrumFrames = { 98, 99, 100 };

    public static void PlayStrumAnimation(Farmer who)
    {
        var frames = new FarmerSprite.AnimationFrame[StrumFrames.Length];

        for (int i = 0; i < StrumFrames.Length; i++)
        {
            frames[i] = new FarmerSprite.AnimationFrame(
                frame: StrumFrames[i],
                milliseconds: 120,
                secondaryArm: false,
                flip: false);
        }

        who.FarmerSprite.animateOnce(frames);
    }

    public static void Tick(GameTime time)
	{
		if (active == null || time == null) return;

		float elapsed = (float)time.ElapsedGameTime.TotalMilliseconds;
		active.RemainingMs -= elapsed;
		active.StrumTimerMs += elapsed;

		//loop every 240ms while casting is active
		if (active.StrumTimerMs >= 240f && active.RemainingMs > 0f)
		{
			active.StrumTimerMs = 0f;
			SpellCastAnim.PlayStrumAnimation(active.Caster);
		}
		if (active.RemainingMs <= 0f)
		{
			active.Caster.UsingTool = false; //TODO find a better way to hold the player still
			active.Caster.canMove = true;
			var onComplete = active.OnComplete;
			active = null;
			onComplete?.Invoke();
		}
	}
    //"hand casting" for LBarrage
    //LONG version
    /*
	 public static void HandCastAnimation(Farmer who, int direction, Action onRelease)
	{
		List<FarmerSprite.AnimationFrame> frames;
		who.Halt(); //quick lock in place

        switch (direction)
		{
			case 0: //up
				frames = new List<FarmerSprite.AnimationFrame>
				{
					new FarmerSprite.AnimationFrame(76, 100),
					new FarmerSprite.AnimationFrame(38, 40),
					new FarmerSprite.AnimationFrame(63, 40),
					new FarmerSprite.AnimationFrame(62, 80),
					new FarmerSprite.AnimationFrame(63, 200, false, false, _ => onRelease(), true),
				};
				break;
			case 1: //right
				frames = new List<FarmerSprite.AnimationFrame>
				{
					new FarmerSprite.AnimationFrame(48, 100),
					new FarmerSprite.AnimationFrame(49, 40),
					new FarmerSprite.AnimationFrame(50, 40),
					new FarmerSprite.AnimationFrame(51, 80),
					new FarmerSprite.AnimationFrame(52, 200, false, false, _ => onRelease(), true),
				};
				break;
			case 2: //down
			default:
				frames = new List<FarmerSprite.AnimationFrame>
				{
					new FarmerSprite.AnimationFrame(66, 100),
					new FarmerSprite.AnimationFrame(67, 40),
					new FarmerSprite.AnimationFrame(68, 40),
					new FarmerSprite.AnimationFrame(69, 80),
					new FarmerSprite.AnimationFrame(70, 200, false, false, _ => onRelease(), true),
				};
				break;
			case 3: // left, same as right but mirrored
				frames = new List<FarmerSprite.AnimationFrame>
				{
					new FarmerSprite.AnimationFrame(48, 100, false, true),
					new FarmerSprite.AnimationFrame(49, 40, false, true),
					new FarmerSprite.AnimationFrame(50, 40, false, true),
					new FarmerSprite.AnimationFrame(51, 80, false, true),
					new FarmerSprite.AnimationFrame(52, 200, false, true, _ => onRelease(), true),
				};
				break;
		}
		who.FarmerSprite.animateOnce(frames.ToArray());
    }
	*/

    //"hand" casting for LBarrage
    //FAST version
    public static void HandCastAnimation(Farmer who, int direction, Action onRelease)
	{
		List<FarmerSprite.AnimationFrame> frames;
		who.Halt(); //quick lock in place

		switch (direction)
		{
			case 0: //up
				frames = new List<FarmerSprite.AnimationFrame>
				{
					new FarmerSprite.AnimationFrame(76, 100),
					new FarmerSprite.AnimationFrame(38, 40, false, false, _ => onRelease(), true),
                //    new FarmerSprite.AnimationFrame(63, 40),
                //    new FarmerSprite.AnimationFrame(62, 80),
                //    new FarmerSprite.AnimationFrame(63, 200, false, false, _ => onRelease(), true),
                };
				break;
			case 1: //right
				frames = new List<FarmerSprite.AnimationFrame>
				{
					new FarmerSprite.AnimationFrame(48, 100),
					new FarmerSprite.AnimationFrame(49, 40, false, false, _ => onRelease(), true),
                 //   new FarmerSprite.AnimationFrame(50, 40),
                 //   new FarmerSprite.AnimationFrame(51, 80),
                 //   new FarmerSprite.AnimationFrame(52, 200, false, false, _ => onRelease(), true),
                };
				break;
			case 2: //down
			default:
				frames = new List<FarmerSprite.AnimationFrame>
				{
					new FarmerSprite.AnimationFrame(66, 100),
					new FarmerSprite.AnimationFrame(67, 40, false, false, _ => onRelease(), true),
                 //   new FarmerSprite.AnimationFrame(68, 40),
                 //   new FarmerSprite.AnimationFrame(69, 80),
                  //  new FarmerSprite.AnimationFrame(70, 200, false, false, _ => onRelease(), true),
                };
				break;
			case 3: // left, same as right but mirrored
				frames = new List<FarmerSprite.AnimationFrame>
				{
					new FarmerSprite.AnimationFrame(48, 100, false, true),
					new FarmerSprite.AnimationFrame(49, 40, false, true, _ => onRelease(), true),
                 //   new FarmerSprite.AnimationFrame(50, 40, false, true),
                 //   new FarmerSprite.AnimationFrame(51, 80, false, true),
                 //   new FarmerSprite.AnimationFrame(52, 200, false, true, _ => onRelease(), true),
                };
				break;
		}
			who.FarmerSprite.animateOnce(frames.ToArray());
	}
}