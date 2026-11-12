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

namespace BriarSinger.Spells

public static class HealthRegenEffects
{
    private class ActiveRegen
    {
        public Farmer Caster;
        public int SecondsRemaining;
        public int AmountPerSecond;
    }
    private static readonly List<ActiveRegen> active = new();

    public static void StartHealthRegen(Farmer caster, int durationSeconds, int amountPerSecond)
    {
        // remove active regen effects
        active.RemoveAll(r => r.Caster == caster); //TODO take this out and add the regen as a buff instead and it will refresh duration instead
        active.Add(new ActiveRegen
        {
            Caster = caster,
            SecondsRemaining = durationSeconds,
            AmountPerSecond = amountPerSecond
        });
    }
    public static void HealthOneSecondUpdate()
    {
        for (int i = active.Count - 1; i >= 0; i--)
        {
            var r = active[i];
            r.Caster.Health = Math.Min(r.Caster.MaxHealth, r.Caster.Health + r.AmountPerSecond);
            r.SecondsRemaining--;
            if (r.SecondsRemaining <= 0)
                active.RemoveAt(i);
        }
    }
}