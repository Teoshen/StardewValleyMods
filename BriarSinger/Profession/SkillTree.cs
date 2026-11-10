using BriarSinger.Framework;
using SpaceCore;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriarSinger.Profession
{
    //Woodward Track

    internal class WoodwardProfession : BaseProfession
    {
            public WoodwardProfession(SpaceCore.Skills.Skill skill, string theId)
                      : base(skill, theId) { }
            //"Heal costs half as much mana and the cooldown is reduced. You can cast daily spells once more per day." //TODO update this
            public override void DoImmediateProfessionPerk()
            {
            BriarSingerBuffs.ExtraDailyCasts += 1; //TODO link this up
            BriarSingerBuffs.HealCostReduction = true; //TODO fix this for real
            }
        }

    internal class SylvanistProfession : BaseProfession
    {
            public SylvanistProfession(SpaceCore.Skills.Skill skill, string theId)
               : base(skill, theId) { }
        //"You can cast daily spells once more per day. Gain the Crop Growth spell."
            public override void DoImmediateProfessionPerk()
            {
            BriarSingerBuffs.ExtraDailyCasts += 1; //TODO link this up
            BriarSingerBuffs.AddCropGrowthSpell = true; //TODO add the crop growth spell
            }
        }

    internal class HerbalistProfession : BaseProfession
    {
            public HerbalistProfession(SpaceCore.Skills.Skill skill, string theId)
                        : base(skill, theId) { }
            //"Rituals and offerings no longer require reagents. Choose your blessing each day."
            public override void DoImmediateProfessionPerk()
            {
            BriarSingerBuffs.RitualDiscount = true; //TODO link up after making the ritual altar
            BriarSingerBuffs.BlessingChoice = true; //TODO link up after making the blessing cauldron
            }
        }


    //Herald Track

    internal class HeraldProfession : BaseProfession
    {
            public HeraldProfession(SpaceCore.Skills.Skill skill, string theId)
                    : base(skill, theId) { }
            //"Bolt costs half as much mana. Gain the Starshot spell." //TODO update this
            public override void DoImmediateProfessionPerk()
            {
            BriarSingerBuffs.BoltReduction = true; //TODO add reduction section
            //TODO figure out what I'm doing with Starshot
            }
        }

    internal class SymphonicProfession : BaseProfession
    {
            public SymphonicProfession(SpaceCore.Skills.Skill skill, string theId)
                    : base(skill, theId) { }
            //"Star Shot chains once off each target." //TODO update this
            public override void DoImmediateProfessionPerk()
            {
            //TODO figure out what to do with Starshot.
            }
        }

    internal class MelodiumProfession : BaseProfession
    {
            public MelodiumProfession(SpaceCore.Skills.Skill skill, string theId)
                  : base(skill, theId) { }
            //"The Bolt debuff is permanent and includes a damage over time effect."
            public override void DoImmediateProfessionPerk()
            {
            BriarSingerBuffs.BoltDebuff = true; //Add the debuff effect, add the dot conditional.
            }
        }
}