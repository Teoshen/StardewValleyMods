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
            //"Heal costs half as much mana and the cooldown is reduced. You can cast daily spells once more per day."
            public override void DoImmediateProfessionPerk()
            {
            BriarSingerBuffs.LevelupBuffs.ExtraDailyCasts += 1; //TODO link this up
            BriarSingerBuffs.LevelupBuffs.HealCostReduction = true; //TODO fix this for real
            }
        }

    internal class ArboristProfession : BaseProfession
    {
            public ArboristProfession(SpaceCore.Skills.Skill skill, string theId)
               : base(skill, theId) { }
        //"Offerings no longer require reagents. Gain the Fruit Tree Growth spell"
        public override void DoImmediateProfessionPerk()
            {
            BriarSingerBuffs.LevelupBuffs.OfferingDiscount = true; //TODO link this up
            BriarSingerBuffs.LevelupBuffs.AddFruitTreeGrowthSpell = true; //TODO finish the fruit tree growth spell
            }
        }

    internal class HerbalistProfession : BaseProfession
    {
            public HerbalistProfession(SpaceCore.Skills.Skill skill, string theId)
                        : base(skill, theId) { }
        //"You can cast daily spells once more per day. Gain the Crop Growth spell."
        public override void DoImmediateProfessionPerk()
            {
            BriarSingerBuffs.LevelupBuffs.ExtraDailyCasts =+ 1; //TODO link up 
            BriarSingerBuffs.LevelupBuffs.AddCropGrowthSpell = true; //TODO add the crop growth spell
        }
    }


    //Skald Track

    internal class SkaldProfession : BaseProfession
    {
            public SkaldProfession(SpaceCore.Skills.Skill skill, string theId)
                    : base(skill, theId) { }
        //"PowerChord has 2 additional projectiles."
        public override void DoImmediateProfessionPerk()
            {
            BriarSingerBuffs.LevelupBuffs.PowerChordProj = true; //TODO add powerchord proj math
            }
        }

    internal class SymphonicProfession : BaseProfession
    {
            public SymphonicProfession(SpaceCore.Skills.Skill skill, string theId)
                    : base(skill, theId) { }
        //"PowerChord projectiles chain." //TODO find something cooler than this
        public override void DoImmediateProfessionPerk()
            {
            BriarSingerBuffs.LevelupBuffs.PowerChordChain = true; //TODO add this
            }
        }

    internal class MelodiumProfession : BaseProfession
    {
            public MelodiumProfession(SpaceCore.Skills.Skill skill, string theId)
                  : base(skill, theId) { }
        //"Gain thorns. Regenerate health per second. Gain the Shockwave spell.
        public override void DoImmediateProfessionPerk()
            {
            BriarSingerBuffs.LevelupBuffs.ThornsHeal = true; //TODO add this
            BriarSingerBuffs.LevelupBuffs.AddShockwaveSpell = true; //TODO add this
            }
        }
}