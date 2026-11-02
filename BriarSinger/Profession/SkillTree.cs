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
    //Mender Track

    internal class MenderProfession : GenericProfession
        {
            public MenderProfession(SpaceCore.Skills.Skill skill, string theId)
                      : base(skill, theId) { }
            //"Heal costs half as much mana and the cooldown is reduced. You can cast daily spells once more per day."
            public override void DoImmediateProfessionPerk()
            {
            BriarSingerBuffs.ExtraDailyCasts += 1; //TODO link this up
            BriarSingerBuffs.HealReduction; //TODO fix this for real
            }
        }

    internal class SummonerProfession : GenericProfession
    {
            public SummonerProfession(SpaceCore.Skills.Skill skill, string theId)
               : base(skill, theId) { }
        //"You can cast daily spells once more per day. Gain the Crop Growth spell."
            public override void DoImmediateProfessionPerk()
            {
            BriarSingerBuffs.ExtraDailyCasts += 1; //TODO link this up
            BriarSingerBuffs.AddCropGrowthSpell //TODO add the crop growth spell
            }
        }

    internal class ApothecaryProfession : GenericProfession
    {
            public ApothecaryProfession(SpaceCore.Skills.Skill skill, string theId)
                        : base(skill, theId) { }
            //"Rituals and offerings no longer require reagents. Choose your blessing each day."
            public override void DoImmediateProfessionPerk()
            {
            BriarSingerBuffs.RitualDiscount; //TODO link up after making the ritual altar
            BriarSingerBuffs.BlessingChoice; //TODO link up after making the blessing cauldron
            }
        }


//Seer Track

    internal class SeerProfession : GenericProfession
    {
            public SeerProfession(SpaceCore.Skills.Skill skill, string theId)
                    : base(skill, theId) { }
            //"Bolt costs half as much mana. Starshot has reduced charge time."
            public override void DoImmediateProfessionPerk()
            {
            BriarSingerBuffs.BoltReduction; //TODO add reduction section
            //TODO figure out what I'm doing with Starshot
            }
        }

    internal class StarcallerProfession : GenericProfession
    {
            public StarcallerProfession(SpaceCore.Skills.Skill skill, string theId)
                    : base(skill, theId) { }
            //"Star Shot ricochets an additional time off each chained target."
            public override void DoImmediateProfessionPerk()
            {
            //TODO figure out what to do with Starshot.
            }
        }

    internal class HexerProfession : GenericProfession
    {
            public HexerProfession(SpaceCore.Skills.Skill skill, string theId)
                  : base(skill, theId) { }
            //"The Bolt debuff is permanent and includes a damage over time effect."
            public override void DoImmediateProfessionPerk()
            {
            BriarSingerBuffs.BoltDebuff; //Add the debuff effect, add the dot conditional.
            }
        }
}