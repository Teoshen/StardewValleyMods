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

        //Level 5 'Skald'
        //PowerChord has 2 additional projectiles.
        public static BaseProfession SkaldProfession;

        //Level 10 Woodward 'Herbalist'
        //You can cast daily spells once more per day. Gain the Crop Growth spell.
        public static BaseProfession HerbalistProfession;

        //Level 10 Woodward 'Arborist'
        //Offerings no longer require reagents. Gain the Fruit Tree Growth spell.
        public static BaseProfession ArboristProfession;

        //Level 10 Skald 'Symphonic'
        //PowerChord projectiles chain.
        public static BaseProfession SymphonicProfession;

        //Level 10 Skald 'Melodium'
        //Gain thorns. Regenerate health per second. Stored healing turns into additional thorns damage.
        public static BaseProfession MelodiumProfession;



        public BriarSingerSkill()
            : base(BriarSingerSkill.MagicSkillId)
        {
            this.Icon = ModEntry.helper.ModContent.Load<Texture2D>("assets/interface/professionicons/briarsingerproficon.png"); //TODO do art but good
            this.SkillsPageIcon = null; //TODO do art

            this.ExperienceCurve = new[] { 100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000 };
            this.ExperienceBarColor = new Microsoft.Xna.Framework.Color(106, 48, 176);

            //Level 5
            BriarSingerSkill.WoodwardProfession = new WoodwardProfession(this, "Woodward")
            {
                Icon = ModEntry.helper.ModContent.Load<Texture2D>("assets/interface/professionicons/woodwardicon"), //TODO do art but good
                Name = "Woodward", //TODO convert to i18n
                Description = "Heal costs half as much mana and the cooldown is reduced. You can cast daily spells once more per day." //TODO convert to i18n
            };
            this.Professions.Add(WoodwardProfession);

            BriarSingerSkill.SkaldProfession = new SkaldProfession(this, "Skald")
            {
                Icon = ModEntry.helper.ModContent.Load<Texture2D>("assets/interface/professionicons/skaldicon"), //TODO do art but better
                Name = "Skald", //TODO convert to i18n
                Description = "PowerChord has 2 additional projectiles." //TODO convert to i18n
            };
            this.Professions.Add(SkaldProfession);

            //Level 10 - Woodward track
            BriarSingerSkill.HerbalistProfession = new HerbalistProfession(this, "Herbalist")
            {
                Icon = ModEntry.helper.ModContent.Load<Texture2D>("assets/interface/professionicons/herbalisticon"), //TODO do art but better
                Name = "Herbalist", //TODO convert to i18n
                Description = "You can cast daily spells once more per day. Gain the Crop Growth spell." //TODO convert to i18n
            };
            this.Professions.Add(HerbalistProfession);

            BriarSingerSkill.ArboristProfession = new SylvanistProfession(this, "Arborist")
            {
                Icon = ModEntry.helper.ModContent.Load<Texture2D>("assets/interface/professionicons/arboristicon"), //TODO do art but better
                Name = "Arborist", //TODO convert to i18n
                Description = "Offerings no longer require reagents. Gain the Fruit Tree Growth spell" //TODO convert to i18n
            };
            this.Professions.Add(SylvanistProfession);

            //Level 10 - Skald track
            BriarSingerSkill.SymphonicProfession = new SymphonicProfession(this, "Symphonic")
            {
                Icon = ModEntry.helper.ModContent.Load<Texture2D>("assets/interface/professionicons/symphonicicon"), //TODO do art but better
                Name = "Symphonic", //TODO convert to i18n
                Description = "PowerChord projectiles chain." //TODO convert to i18n
            };
            this.Professions.Add(SymphonicProfession);

            BriarSingerSkill.MelodiumProfession = new MelodiumProfession(this, "Melodium")
            {
                Icon = ModEntry.helper.ModContent.Load<Texture2D>("assets/interface/professionicons/symphonicicon"), //TODO do art but better
                Name = "Melodium", //TODO convert to i18n
                Description = "Gain thorns. Regenerate health per second. Stored healing turns into additional thorns damage." //TODO convert to i18n
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

        public static class XpValues
        {
            public const int CauldronQuest = 150;
            public const int Blessing = 15;
            //TODO point to here when you make the machines and quests
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
}
