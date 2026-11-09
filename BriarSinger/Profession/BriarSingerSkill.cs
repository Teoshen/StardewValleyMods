using BriarSinger;
using BriarSinger.Core;
using BriarSinger.Framework;
using BriarSinger.Spells;
using BriarSinger.Spells.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;

namespace BriarSinger.Profession
{

    internal class BriarSingerSkill : SpaceCore.Skills.Skill
    {
        //Unique ID for BriarSinger skill.
        public static readonly string MagicSkillId = "Teoshen.BriarSinger";

        //Level 5 'Woodward'
        //Heal costs half as much mana and the cooldown is reduced. You can cast daily spells once more per day.
        public static BaseProfession WoodwardProfession;

        //Level 5 'Herald'
        //Bolt costs half as much mana. PowerChord has additional projectiles.
        public static BaseProfession HeraldProfession;

        //Level 10 Woodward 'Herbalist'
        //Rituals and offerings no longer require reagents, choose your blessing each day.
        public static BaseProfession HerbalistProfession;

        //Level 10 Woodward 'Sylvanist'
        //You can cast daily spells once more per day. Gain the Crop Growth spell.
        public static BaseProfession SylvanistProfession;

        //Level 10 Herald 'Symphonic'
        //PowerChord has an explosion radius.
        public static BaseProfession SymphonicProfession;

        //Level 10 Herald 'Melodium'
        //The Bolt debuff is permanent and includes a damage over time effect
        public static BaseProfession MelodiumProfession;



        public BriarSingerSkill()
            : base(BriarSingerSkill.MagicSkillId)
        {
            this.Icon = ModEntry.helper.ModContent.Load<Texture2D>(""); //TODO do art
            this.SkillsPageIcon = null; //TODO

            this.ExperienceCurve = new[] { 100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000 };
            this.ExperienceBarColor = new Microsoft.Xna.Framework.Color(106, 48, 176);

            //Level 5
            BriarSingerSkill.WoodwardProfession = new WoodwardProfession(this, "Woodward")
            {
                Icon = null, //TODO do art
                Name = "Woodward", //TODO convert to i18n
                Description = "Heal costs half as much mana and the cooldown is reduced. You can cast daily spells once more per day." //TODO update this //TODO convert to i18n
            };
            this.Professions.Add(WoodwardProfession);

            BriarSingerSkill.HeraldProfession = new HeraldProfession(this, "Herald")
            {
                Icon = null, //TODO do art
                Name = "Herald", //TODO convert to i18n
                Description = "Bolt costs half as much mana. Starshot has reduced charge time." //TODO update this //TODO convert to i18n
            };
            this.Professions.Add(HeraldProfession);

            //Level 10 - Woodward track
            BriarSingerSkill.HerbalistProfession = new HerbalistProfession(this, "Herbalist")
            {
                Icon = null, //TODO do art
                Name = "Herbalist", //TODO convert to i18n
                Description = "Rituals and offerings no longer require reagents. Choose your blessing each day." //TODO convert to i18n
            };
            this.Professions.Add(HerbalistProfession);

            BriarSingerSkill.SylvanistProfession = new SylvanistProfession(this, "Sylvanist")
            {
                Icon = null, //TODO do art
                Name = "Sylvanist", //TODO convert to i18n
                Description = "You can cast daily spells once more per day. Gain the Crop Growth spell." //TODO convert to i18n
            };
            this.Professions.Add(SylvanistProfession);

            //Level 10 - Herald track
            BriarSingerSkill.SymphonicProfession = new SymphonicProfession(this, "Symphonic")
            {
                Icon = null, //TODO do art
                Name = "Symphonic", //TODO convert to i18n
                Description = "Star Shot ricochets an additional time off each chained target." //TODO update this //TODO convert to i18n
            };
            this.Professions.Add(SymphonicProfession);

            BriarSingerSkill.MelodiumProfession = new MelodiumProfession(this, "Melodium")
            {
                Icon = null, //TODO do art
                Name = "Melodium", //TODO convert to i18n
                Description = "The Bolt debuff is permanent and includes a damage over time effect." //TODO convert to i18n
            };
            this.Professions.Add(MelodiumProfession);
        }

        public override string GetName()
        {
            return "BriarSinger";
        }

        public override List<string> GetExtraLevelUpInfo(int level)
        {
            return new()
            {
                "+20 to maximum mana. +1 to mana regeneration" //TODO convert to i18n
            };
        }

        public override string GetSkillPageHoverText(int level)
        {
            return $"+{level} maximum mana, +{level} mana regen"; //TODO convert to i18n
        }

        public override void DoLevelPerk(int level)
        {
            //add level perk
            int curMana = Game1.player.GetMaxMana();
            if (level > 1 || curMana < MagicConstants.ManaPointsPerLevel) //skip level 1 perks because it's custom
                Game1.player.SetMaxMana(curMana + MagicConstants.ManaPointsPerLevel);

            //TODO make sure this works and then add the regen too or add it in mod entry
        }

        public static void AddEXP(Farmer who, int amount)
        {
            if (amount <= 0)
                return;

            Farmer player = Game1.GetPlayer(who.UniqueMultiplayerID);
            SpaceCore.Skills.AddExperience(player, ModEntry.SkillID, amount);
        }

    public static void ApplyBriarSingerSkill(
            Farmer who,
            int EXP,
            bool bonusLoot = false,
            int xLocation = 0,
            int yLocation = 0,
            bool panning = false,
            string exactItem = ""
        )

        {
            var farmer = who ?? Game1.GetPlayer(who.UniqueMultiplayerID);
            if (farmer == null)
                return;

            if (xLocation == 0 || yLocation == 0)
            {
                // Get tile location once
                xLocation = (int)farmer.Tile.X;
                yLocation = (int)farmer.Tile.Y;
            }
        } 
    } 

    public static class MagicConstants
    {
        public const int ManaPointsPerLevel = 20;
        public const float ManaRegenPerLevel = 1.0f;
    }

    public static class BriarSingerBuffs
    {
        public static int ExtraDailyCasts = 0;
        public static bool HealCostReduction = false;
        public static bool AddCropGrowthSpell = false;
        public static bool RitualDiscount = false;
        public static bool BlessingChoice = false;
        public static bool BoltReduction = false;
        public static bool BoltDebuff = false;
    }
}
