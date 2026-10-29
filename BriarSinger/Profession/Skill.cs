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

namespace BriarSinger.Profession.Skills
{
	internal class Skill : SpaceCore.Skills.Skill
	{
		//Unique ID for BriarSinger skill.
		public static readonly string MagicSkillId = "Teoshen.BriarSinger";

        //Level 5 'Mender'
        //Heal costs half as much mana and the cooldown is reduced. You can cast daily spells once more per day.
        public static GenericProfession MenderProfession;

        //Level 5 'Seer'
        //Bolt costs half as much mana. Starshot has reduced charge time.
        public static GenericProfession SeerProfession;

        //Level 10 Mender 'Apothecary'
        //Rituals and offerings no longer require reagents, choose your blessing each day.
        public static GenericProfession ApothecaryProfession;

        //Level 10 Mender 'Summoner'
        //You can cast daily spells once more per day. Gain the Crop Growth spell.
        public static GenericProfession SummonerProfession;

        //Level 10 Seer 'Starcaller'
        //Star Shot ricochets an additional time off each chained target
        public static GenericProfession StarcallerProfession;

        //Level 10 Seer 'Hexer'
        //The Bolt debuff is permanent and includes a damage over time effect
        public static GenericProfession HexerProfession;



        public Skill() : base(Skill.MagicSkillId)
        {
            this.Icon = Mod.Instance.Helper.ModContent.Load<Texture2D>("assets/interface/BriarSingerMagicExpIcon.png"); //TODO
            this.SkillsPageIcon = null; //TODO

            this.ExperienceCurve = new[] { 100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000 };
            this.ExperienceBarColor = new Microsoft.Xna.Framework.Color(106, 48, 176);

            //Level 5
            Skill.MenderProfession = new MenderProfession(this, "Mender")
            {
                Icon = null, //TODO
                Name = "Mender",
                Description = "Heal costs half as much mana and the cooldown is reduced. You can cast daily spells once more per day."
            };
            this.Professions.Add(Skill.MenderProfession);

            Skill.SeerProfession = new SeerProfession(this, "Seer")
            {
                Icon = null, //TODO
                Name = "Seer",
                Description = "Bolt costs half as much mana. Starshot has reduced charge time."
            };
            this.Professions.Add(Skill.SeerProfession);

            //Level 10 - Mender track
            Skill.ApothecaryProfession = new ApothecaryProfession(this, "Apothecary")
            {
                Icon = null, //TODO
                Name = "Apothecary",
                Description = "Rituals and offerings no longer require reagents. Choose your blessing each day."
            };
            this.Professions.Add(Skill.ApothecaryProfession);

            Skill.SummonerProfession = new SummonerProfession(this, "Summoner")
            {
                Icon = null, //TODO
                Name = "Summoner",
                Description = "You can cast daily spells once more per day. Gain the Crop Growth spell."
            };
            this.Professions.Add(Skill.SummonerProfession);


            //Level 10 - Seer track
            Skill.StarcallerProfession = new StarcallerProfession(this, "Starcaller")
            {
                Icon = null, //TODO
                Name = "Starcaller",
                Description = "Star Shot ricochets an additional time off each chained target."
            };
            this.Professions.Add(Skill.StarcallerProfession);

            Skill.HexerProfession = new HexerProfession(this, "Hexer")
            {
                Icon = null, //TODO
                Name = "Hexer",
                Description = "The Bolt debuff is permanent and includes a damage over time effect."
            };
            this.Professions.Add(Skill.HexerProfession);


            public override string GetName()
             {
                 return "BriarSinger";
             }

        public override List<string> GetExtraLevelUpInfo(int level)
        {
            return new()
            {
                "+20 to maximum mana. +1 to mana regeneration"
            }
        }

        public override string GetSkillPageHoverText(int level)
        {
            return "+" + level + "maximum mana", "+" + level + "mana regen"
        }

        public override void DoLevelPerk(int level)
        {
            //add level perk
            int curMana = Game1.player.GetMaxMana();
            if (level > 1 || curMana < MagicConstants.ManaPointsPerLevel) //skip level 1 perks because it's custom
                Game1.player.SetMaxMana(curMana + MagicConstants.ManaPointsPerLevel);

            //make sure this works and then add the regen too or add it in mod entry
        }
        }
    }
}
