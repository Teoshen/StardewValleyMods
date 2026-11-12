using System;

/// <summary>
/// Summary description for Class1
/// </summary>
public class Class1
{
	public Class1()
	{
        //Template
        /*  Buff buff = new Buff(
      id: "Example.ModId_ZoomZoom",
      displayName: "Zoom Zoom", // can optionally specify description text too
      iconTexture: this.Helper.ModContent.Load<Texture2D>("assets/zoom.png"),
      iconSheetIndex: 0,
      duration: 30_000, // 30 seconds
      effects: new BuffEffects()
      {
          Speed = { 10 } // shortcut for buff.Speed.Value = 10
      }
  );
          Game1.player.applyBuff(buff); */



        ///  Tree blessings.
        public static class BriarSingerOfferings
    {
        // Food buffs last longer
        // TODO add this

        // +2 immunity?
        // TODO add this

        // Death ward
        // TODO add this

        // Exhaustion ward
        // TODO add this

        // Increased mana regeneration
        // TODO add this
    }

    ///Equipment buffs
    public static class BriarSingerEquipment
    {
        // Skald armor
        // TODO add this

        // Moon robes
        // TODO add this

        // Druid ring
        // TODO add this

        // Bard boots
        // TODO add this
    }

        // Skill unlocks
         public static class BriarSingerLevelupBuffs
    {
        public static int ExtraDailyCasts = 0;
        public static bool HealCostReduction = false;
        public static bool AddCropGrowthSpell = false;
        public static bool AddFruitTreeGrowthSpell = false;
        public static bool OfferingDiscount = false;
        public static bool PowerChordProj = false;
        public static bool ThornsHeal = false;
        public static bool PowerChordChain = false;
        public static bool AddShockwaveSpell = false;
    }
}
}
