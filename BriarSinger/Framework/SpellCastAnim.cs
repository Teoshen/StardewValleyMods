using BriarSinger;
using BriarSinger.Spells;
using BriarSinger.Spells.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;

public static class SpellCastAnim
{
	private class ActiveCast
	{
		public Farmer Caster;
		public fload RemainingMs;
		public float StrumTimerMs;
		public Action OnComplete;
	}

	private static ActiveCast active;

	public static void Begin(Farmer caster, float castTimeSeconds, Action onComplete)
	{
		active = new ActiveCast
		{
			Caster = caster,
			RemainingMs = castTimeSeconds * 1000f,
			StrumTimerMs = 0f,
			OnComplete = onComplete
		};
		caster.UsingTool = true; //lock in place while casting
		Harpsword.PlayStrumAnimation(caster);
	}

	public static void Tick(GameTime time)
	{
		if (active == null || time == null) return;

		float elapsed = time.ElapsedGameTime.Milliseconds;
		active.RemainingMs -= elapsed;
		active.StrumTimerMs += elapsed;

		//loop every 240ms while casting is active
		if (active.StrumTimerMs >= 240f && active.RemainingMs > 0f)
		{
			active.StrumTimerMs = 0f;
			HarpSword.PlayStrumAnimation(active.Caster);
		}
		if (active.RemainingMs <= 0f)
		{
			active.Caster.UsingTool = false;
			var onComplete = active.OnComplete;
			active = null;
			onComplete?.Invoke();
		}
	}
}
